using FluentValidation;
using CartaNoAdeudoApi.Core.Common.Layouts;
using CartaNoAdeudoApi.Core.DTOs.Requests.Layouts;
using CartaNoAdeudoApi.Core.DTOs.Responses.Layouts;
using CartaNoAdeudoApi.Core.Entities.Catalogos;
using CartaNoAdeudoApi.Core.Exceptions;
using CartaNoAdeudoApi.Core.Interfaces;
using CartaNoAdeudoApi.Core.Interfaces.Repositories;

namespace CartaNoAdeudoApi.Infrastructure.Services
{
    public class LayoutService(
        IUnitOfWork uow,
        IValidator<Layout> validator,
        IMarcadorExtractor marcadorExtractor,
        IDocxLayoutStorage storage) : ILayoutService
    {
        public async Task<IReadOnlyList<LayoutListItemResponse>> ListarAsync(CancellationToken ct = default)
        {
            var layouts = await uow.Layouts.GetAllAsync(ct);
            return layouts
                .OrderByDescending(l => l.FechaHora)
                .Select(l => new LayoutListItemResponse(l.Id, l.Descripcion, l.FechaInicioAutorizada, l.FechaFinAutorizada, l.Activo))
                .ToList();
        }

        public async Task<LayoutResponse> ObtenerAsync(int id, CancellationToken ct = default)
        {
            var layout = await uow.Layouts.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(Layout), id);

            var marcadores = await ExtraerMarcadoresAsync(layout.Archivo, ct);
            return ToResponse(layout, marcadores);
        }

        public async Task<LayoutMarcadoresResponse> PrevisualizarMarcadoresAsync(Stream archivo, CancellationToken ct = default)
        {
            var contenido = await LeerContenidoAsync(archivo, ct);
            var marcadores = await marcadorExtractor.ExtraerAsync(new MemoryStream(contenido), ct);
            return EvaluarMarcadores(marcadores);
        }  

        public async Task<LayoutResponse> RegistrarAsync(RegistrarLayoutRequest request, Stream archivo, CancellationToken ct = default)
        {
            var contenido = await LeerContenidoAsync(archivo, ct);
            var marcadores = await marcadorExtractor.ExtraerAsync(new MemoryStream(contenido), ct);
            var evaluacion = EvaluarMarcadores(marcadores);

            if (request.Activo && !evaluacion.AptoParaActivar)
                throw new ConflictException(
                    $"El layout no puede activarse: faltan los marcadores obligatorios {string.Join(", ", evaluacion.MarcadoresObligatoriosFaltantes)}.");

            var layout = new Layout
            {
                Archivo = await storage.GuardarAsync(new MemoryStream(contenido), ct),
                Descripcion = request.Descripcion,
                FechaInicioAutorizada = request.VigenciaInicio,
                FechaFinAutorizada = request.VigenciaFin,
                Activo = request.Activo
            };

            await ValidarEntidadAsync(layout, ct);

            if (layout.Activo)
                await DesactivarLayoutsSolapadosAsync(layout, ct);

            await uow.Layouts.AddAsync(layout, ct);
            await uow.SaveAsync(ct);

            return ToResponse(layout, marcadores);
        }

        public async Task<LayoutResponse> ActualizarAsync(int id, ActualizarLayoutRequest request, Stream? archivo, CancellationToken ct = default)
        {
            var layout = await uow.Layouts.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(Layout), id);

            layout.Descripcion = request.Descripcion;
            layout.FechaInicioAutorizada = request.VigenciaInicio;
            layout.FechaFinAutorizada = request.VigenciaFin;

            IReadOnlyList<string> marcadores;

            if (archivo is not null)
            {
                var contenido = await LeerContenidoAsync(archivo, ct);
                marcadores = await marcadorExtractor.ExtraerAsync(new MemoryStream(contenido), ct);

                if (layout.Activo && !EvaluarMarcadores(marcadores).AptoParaActivar)
                    throw new ConflictException("El layout quedaría sin los marcadores obligatorios y está Activo; desactívalo antes de reemplazar el archivo o corrígelo.");

                // RN-007: reemplaza solo el .docx físico; el Id del Layout (y por lo
                // tanto lo ya emitido con él) no cambia.
                var archivoAnterior = layout.Archivo;
                layout.Archivo = await storage.GuardarAsync(new MemoryStream(contenido), ct);
                storage.Eliminar(archivoAnterior);
            }
            else
            {
                marcadores = await ExtraerMarcadoresAsync(layout.Archivo, ct);
            }

            await ValidarEntidadAsync(layout, ct);

            uow.Layouts.Update(layout);
            await uow.SaveAsync(ct);

            return ToResponse(layout, marcadores);
        }

        public async Task ToggleActivoAsync(int id, CancellationToken ct = default)
        {
            var layout = await uow.Layouts.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(Layout), id);

            if (!layout.Activo)
            {
                var marcadores = await ExtraerMarcadoresAsync(layout.Archivo, ct);
                var evaluacion = EvaluarMarcadores(marcadores);
                if (!evaluacion.AptoParaActivar)
                    throw new ConflictException(
                        $"El layout no puede activarse: faltan los marcadores obligatorios {string.Join(", ", evaluacion.MarcadoresObligatoriosFaltantes)}.");

                await DesactivarLayoutsSolapadosAsync(layout, ct);
            }

            layout.Activo = !layout.Activo;
            uow.Layouts.Update(layout);
            await uow.SaveAsync(ct);
        }

        /// <summary>
        /// Asunción (confirmar con negocio): un solo layout activo por rango de vigencia
        /// solapado — LayoutResponse/RF-003 asumen "único layout activo y vigente".
        /// </summary>
        private async Task DesactivarLayoutsSolapadosAsync(Layout nuevo, CancellationToken ct)
        {
            var solapados = await uow.Layouts.FindAsync(l =>
                l.Activo && l.Id != nuevo.Id &&
                l.FechaInicioAutorizada <= nuevo.FechaFinAutorizada &&
                nuevo.FechaInicioAutorizada <= l.FechaFinAutorizada, ct);

            foreach (var l in solapados)
            {
                l.Activo = false;
                uow.Layouts.Update(l);
            }
        }

        private async Task ValidarEntidadAsync(Layout layout, CancellationToken ct)
        {
            var validacion = await validator.ValidateAsync(layout, ct);
            if (!validacion.IsValid)
                throw new ConflictException(string.Join(" | ", validacion.Errors.Select(e => e.ErrorMessage)));
        }

        private async Task<IReadOnlyList<string>> ExtraerMarcadoresAsync(string archivo, CancellationToken ct)
        {
            await using var stream = await storage.AbrirAsync(archivo, ct);
            return await marcadorExtractor.ExtraerAsync(stream, ct);
        }

        private static async Task<byte[]> LeerContenidoAsync(Stream stream, CancellationToken ct)
        {
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms, ct);
            return ms.ToArray();
        }

        private static LayoutMarcadoresResponse EvaluarMarcadores(IReadOnlyList<string> marcadoresEnDocx)
        {
            var reconocidos = marcadoresEnDocx.Where(LayoutMarcadores.Reconocidos.Contains).ToList();
            var noReconocidos = marcadoresEnDocx.Except(reconocidos).ToList();
            var faltantes = LayoutMarcadores.Obligatorios.Except(marcadoresEnDocx).ToList();

            return new LayoutMarcadoresResponse(reconocidos, noReconocidos, faltantes, faltantes.Count == 0);
        }

        private static LayoutResponse ToResponse(Layout l, IReadOnlyList<string> marcadores) =>
            new(l.Id, l.Descripcion, l.Archivo, l.FechaHora, l.FechaInicioAutorizada, l.FechaFinAutorizada, l.Activo, marcadores);
    }
}
