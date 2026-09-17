using FluentValidation;
using Microsoft.EntityFrameworkCore;
using CartaNoAdeudoApi.Core.DTOs.Requests.Cartas;
using CartaNoAdeudoApi.Core.DTOs.Responses.Cartas;
using CartaNoAdeudoApi.Core.Entities.Cartas;
using CartaNoAdeudoApi.Core.Entities.Catalogos;
using CartaNoAdeudoApi.Core.Exceptions;
using CartaNoAdeudoApi.Core.Interfaces;
using CartaNoAdeudoApi.Core.Interfaces.Repositories;

namespace CartaNoAdeudoApi.Infrastructure.Services
{
    public class CartaService(IUnitOfWork uow, IValidator<Datos> validator, IFirmaContraloriaService firmaContraloria) : ICartaService
    {
        public async Task<CartaAceptadaResponse> SolicitarCartaAsync(GenerarCartaRequest request, CancellationToken ct = default)
        {
            var tipoCarta = await uow.TiposCartas.GetByClaveAsync(request.TipoCarta, ct)
                ?? throw new NotFoundException(nameof(TiposCartas), request.TipoCarta);

            if (tipoCarta.Clave == "03" && string.IsNullOrWhiteSpace(request.LicAlcoholes))
                throw new ConflictException("El valor LicAlcoholes es obligatorio para el tipo de carta 03.");

            if (!DateOnly.TryParse(request.InicioVigencia, out var inicioVigencia))
                throw new ConflictException("InicioVigencia no tiene un formato de fecha válido.");

            if (await uow.Repository<Datos>().ExistsAsync(d =>
                    d.Rfc == request.Rfc && d.RO == request.Ro && d.IdTipoCarta == tipoCarta.Id &&
                    d.Estatus != EstatusSolicitud.Firmada, ct))
                throw new ConflictException("Ya existe una solicitud en trámite para ese RFC, RO y tipo de carta.");

            var dato = new Datos
            {
                IdTipoCarta = tipoCarta.Id,
                Nombre = request.Nombre,
                Rfc = request.Rfc,
                RO = request.Ro,
                Email = request.Email,
                InicioVigencia = inicioVigencia,
                // TODO: confirmar con negocio la vigencia real de cada tipo de carta; se asume 1 año
                // mientras no exista esa regla documentada.
                Vencimiento = inicioVigencia.AddYears(1),
                Alcoholes = request.LicAlcoholes,
                Estatus = EstatusSolicitud.SolicitudFirma
            };

            var validacion = await validator.ValidateAsync(dato, ct);
            if (!validacion.IsValid)
                throw new ConflictException(string.Join(" | ", validacion.Errors.Select(e => e.ErrorMessage)));

            // Transacción corta solo para el folio + el alta; la firma (llamada externa,
            // puede tardar/reintentar) se hace después, fuera de cualquier transacción abierta.
            await using var tx = await uow.BeginTransactionAsync(ct);
            try
            {
                dato.Folio = await AsignarFolioAsync(tipoCarta.Clave, ct);
                await uow.Repository<Datos>().AddAsync(dato, ct);
                await uow.SaveAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }

            await IntentarFirmarAsync(dato, tipoCarta.Clave, ct);

            return new CartaAceptadaResponse(dato.Id, dato.Estatus);
        }

        public async Task<ValidarCartaResponse> ValidarCartaAsync(ValidarCartaRequest request, CancellationToken ct = default)
        {
            var folio = request.Folio.ToString();
            var dato = await uow.Repository<Datos>().AsQueryable()
                .Include(d => d.Firmas)
                .FirstOrDefaultAsync(d => d.Rfc == request.Rfc && d.Folio == folio, ct);

            if (dato is null)
                return new ValidarCartaResponse(ResultadoValidacion.NoEncontrado, null, null, null, null);

            if (dato.Estatus != EstatusSolicitud.Firmada)
                return new ValidarCartaResponse(ResultadoValidacion.NoAutentico, dato.Folio, null, null, null);

            var firma = dato.Firmas.OrderByDescending(f => f.Fecha).FirstOrDefault();

            return new ValidarCartaResponse(
                ResultadoValidacion.Autentico,
                dato.Folio,
                firma?.Descripcion,
                dato.FechaFirmado,
                // TODO: requiere el servicio de firma electrónica (pendiente) para validar
                // vigencia real del certificado al momento de la firma.
                null);
        }

        public async Task<byte[]> ObtenerReportePdfAsync(ValidarCartaRequest request, CancellationToken ct = default)
        {
            var folio = request.Folio.ToString();
            var dato = await uow.Repository<Datos>().AsQueryable()
                .Include(d => d.Archivo)
                .FirstOrDefaultAsync(d => d.Rfc == request.Rfc && d.Folio == folio, ct)
                ?? throw new NotFoundException(nameof(Datos), $"{request.Rfc}/{request.Folio}");

            if (dato.Archivo is null)
                throw new ConflictException("La carta aún no ha sido generada; la solicitud sigue en proceso de firma.");

            return Convert.FromBase64String(dato.Archivo.Carta);
        }

        public async Task<ReprocesoResultadoResponse> ReprocesarSolicitudesAsync(ReprocesarSolicitudesRequest request, CancellationToken ct = default)
        {
            var resultados = new List<ReprocesoItemResultado>();
            var encolados = 0;

            foreach (var solicitudId in request.SolicitudIds)
            {
                var dato = await uow.Repository<Datos>().AsQueryable()
                    .Include(d => d.TipoCarta)
                    .FirstOrDefaultAsync(d => d.Id == solicitudId, ct);

                if (dato is null)
                {
                    resultados.Add(new ReprocesoItemResultado(solicitudId, false, "Solicitud no encontrada"));
                    continue;
                }

                if (dato.Estatus == EstatusSolicitud.Firmada)
                {
                    resultados.Add(new ReprocesoItemResultado(solicitudId, false, "La solicitud ya fue firmada"));
                    continue;
                }

                await IntentarFirmarAsync(dato, dato.TipoCarta!.Clave, ct);

                if (dato.Estatus == EstatusSolicitud.Firmada)
                {
                    resultados.Add(new ReprocesoItemResultado(solicitudId, true, null));
                    encolados++;
                }
                else
                {
                    resultados.Add(new ReprocesoItemResultado(solicitudId, false, "La firma electrónica volvió a fallar"));
                }
            }

            return new ReprocesoResultadoResponse(request.SolicitudIds.Count, encolados, resultados);
        }

        /// <summary>
        /// Llama a <see cref="IFirmaContraloriaService"/> y registra el resultado: si firma,
        /// crea la <c>Firmas</c> y marca <c>Datos</c> como Firmada; si falla, deja una
        /// <c>Tareas</c> en Error con el motivo (reintentable vía <see cref="ReprocesarSolicitudesAsync"/>).
        /// </summary>
        private async Task IntentarFirmarAsync(Datos dato, string tipoCartaClave, CancellationToken ct)
        {
            var tarea = new Tareas { IdDato = dato.Id, FechaInicio = DateTimeOffset.UtcNow };

            var resultado = await firmaContraloria.FirmarAsync(
                dato.Rfc, dato.Nombre, dato.RO, tipoCartaClave, dato.InicioVigencia, dato.Alcoholes, ct);

            tarea.FechaFin = DateTimeOffset.UtcNow;

            if (resultado.Exitoso)
            {
                await uow.Repository<Firmas>().AddAsync(new Firmas
                {
                    IdDato = dato.Id,
                    Descripcion = resultado.Descripcion,
                    Firma = resultado.Firma
                }, ct);

                dato.Estatus = EstatusSolicitud.Firmada;
                dato.FechaFirmado = tarea.FechaFin;
                uow.Repository<Datos>().Update(dato);

                tarea.IdEstado = (await ObtenerEstadoTareaAsync(EstadoTarea.Completada, ct)).Id;
                tarea.Nota = "Carta firmada correctamente.";
            }
            else
            {
                tarea.IdEstado = (await ObtenerEstadoTareaAsync(EstadoTarea.Error, ct)).Id;
                tarea.MensajeError = resultado.MensajeError;
            }

            await uow.Repository<Tareas>().AddAsync(tarea, ct);
            await uow.SaveAsync(ct);
        }

        private async Task<EstadoTarea> ObtenerEstadoTareaAsync(string descripcion, CancellationToken ct) =>
            await uow.EstadosTarea.GetByDescripcionAsync(descripcion, ct)
                ?? throw new ServiceUnavailableException($"El catálogo de estados de tarea no tiene sembrado '{descripcion}'.");

        private async Task<string> AsignarFolioAsync(string tipoCartaClave, CancellationToken ct)
        {
            var folioConsecutivo = (await uow.Repository<FolioConsecutivo>()
                .FindAsync(f => f.TipoCarta == tipoCartaClave, ct)).SingleOrDefault();

            if (folioConsecutivo is null)
            {
                folioConsecutivo = new FolioConsecutivo { TipoCarta = tipoCartaClave, UltimoFolio = 0 };
                await uow.Repository<FolioConsecutivo>().AddAsync(folioConsecutivo, ct);
            }

            folioConsecutivo.UltimoFolio++;
            uow.Repository<FolioConsecutivo>().Update(folioConsecutivo);

            // Folio puramente numérico para que coincida con ValidarCartaRequest.Folio (int).
            // Nota: al ser consecutivo por tipo de carta, dos tipos distintos pueden compartir
            // el mismo número; ValidarCartaRequest no distingue por tipo (ver RF-005 legado).
            return folioConsecutivo.UltimoFolio.ToString();
        }
    }
}
