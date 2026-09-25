using FluentValidation;
using Microsoft.AspNetCore.DataProtection;
using CartaNoAdeudoApi.Core.DTOs.Requests.Firmantes;
using CartaNoAdeudoApi.Core.DTOs.Responses.Firmantes;
using CartaNoAdeudoApi.Core.Entities.Catalogos;
using CartaNoAdeudoApi.Core.Exceptions;
using CartaNoAdeudoApi.Core.Interfaces;
using CartaNoAdeudoApi.Core.Interfaces.Repositories;

namespace CartaNoAdeudoApi.Infrastructure.Services
{
    public class FirmanteService(
        IUnitOfWork uow,
        IValidator<Firmante> validator,
        ICertificadoFirmaService certificados,
        IPfxStorage storage,
        IDataProtectionProvider dataProtection) : IFirmanteService
    {
        /// <summary>Días antes del vencimiento (del certificado o de la vigencia operativa) para marcar ProximoAVencer.</summary>
        private const int DiasProximoAVencer = 30;

        /// <summary>Propósito de Data Protection con el que se cifra la contraseña.</summary>
        public const string PropositoContrasena = "Firmante.Contrasena.v1";

        private readonly IDataProtector _protector = dataProtection.CreateProtector(PropositoContrasena);

        public async Task<IReadOnlyList<FirmanteListItemResponse>> ListarAsync(CancellationToken ct = default)
        {
            var firmantes = await uow.Repository<Firmante>().GetAllAsync(ct);
            return firmantes
                .OrderBy(f => f.Nombre)
                .Select(f => new FirmanteListItemResponse(
                    f.Id, f.Nombre, f.Puesto, f.EstatusCertificado,
                    f.FechaInicioAutorizada, f.FechaFinAutorizada, f.Activo, ProximoAVencer(f)))
                .ToList();
        }

        public async Task<FirmanteResponse> ObtenerAsync(Guid id, CancellationToken ct = default) =>
            ToResponse(await ObtenerEntidadAsync(id, ct));

        public async Task<FirmanteResponse> RegistrarAsync(RegistrarFirmanteRequest request, Stream pfx, CancellationToken ct = default)
        {
            var contenido = await LeerContenidoAsync(pfx, ct);
            var certificado = certificados.LeerCertificado(contenido, request.Contrasena);

            var firmante = new Firmante
            {
                Nombre = NombreDelCertificado(certificado),
                Puesto = request.Puesto,
                Genero = request.Genero,
                FechaInicioAutorizada = request.VigenciaOperativaInicio,
                FechaFinAutorizada = request.VigenciaOperativaFin,
                Contrasena = _protector.Protect(request.Contrasena),
                Activo = request.Activo
            };
            AplicarCertificado(firmante, certificado);

            if (firmante.Activo)
                ExigirCertificadoVigente(firmante);

            firmante.Pfx = await storage.GuardarAsync(contenido, ct);
            try
            {
                await ValidarEntidadAsync(firmante, ct);
                await uow.Repository<Firmante>().AddAsync(firmante, ct);
                await uow.SaveAsync(ct);
            }
            catch
            {
                storage.Eliminar(firmante.Pfx);
                throw;
            }

            return ToResponse(firmante);
        }

        public async Task<FirmanteResponse> ActualizarAsync(Guid id, ActualizarFirmanteRequest request, Stream? pfx, CancellationToken ct = default)
        {
            var firmante = await ObtenerEntidadAsync(id, ct);

            firmante.Puesto = request.Puesto;
            firmante.Genero = request.Genero;
            firmante.FechaInicioAutorizada = request.VigenciaOperativaInicio;
            firmante.FechaFinAutorizada = request.VigenciaOperativaFin;

            var pfxAnterior = firmante.Pfx;
            string? pfxNuevo = null;

            if (pfx is not null)
            {
                if (string.IsNullOrWhiteSpace(request.Contrasena))
                    throw new ConflictException("Al reemplazar el PFX se requiere la contraseña del nuevo archivo.");

                var contenido = await LeerContenidoAsync(pfx, ct);
                var certificado = certificados.LeerCertificado(contenido, request.Contrasena);
                AplicarCertificado(firmante, certificado);
                firmante.Nombre = NombreDelCertificado(certificado);
                firmante.Contrasena = _protector.Protect(request.Contrasena);

                pfxNuevo = await storage.GuardarAsync(contenido, ct);
                firmante.Pfx = pfxNuevo;
            }
            else if (!string.IsNullOrWhiteSpace(request.Contrasena))
            {
                if (string.IsNullOrEmpty(firmante.Pfx))
                    throw new ConflictException("El firmante no tiene un PFX cargado; adjunta el archivo junto con la contraseña.");

                // Se valida contra el PFX guardado antes de reemplazar la contraseña almacenada.
                certificados.LeerCertificado(await storage.LeerAsync(firmante.Pfx, ct), request.Contrasena);
                firmante.Contrasena = _protector.Protect(request.Contrasena);
            }

            try
            {
                if (firmante.Activo)
                    ExigirCertificadoVigente(firmante);

                await ValidarEntidadAsync(firmante, ct);
                uow.Repository<Firmante>().Update(firmante);
                await uow.SaveAsync(ct);
            }
            catch
            {
                if (pfxNuevo is not null)
                    storage.Eliminar(pfxNuevo);
                throw;
            }

            if (pfxNuevo is not null && !string.IsNullOrEmpty(pfxAnterior))
                storage.Eliminar(pfxAnterior);

            return ToResponse(firmante);
        }

        public async Task ToggleActivoAsync(Guid id, CancellationToken ct = default)
        {
            var firmante = await ObtenerEntidadAsync(id, ct);

            if (!firmante.Activo)
            {
                ExigirCertificadoVigente(firmante);
                // El estatus se calculó al cargar el PFX y puede haber quedado en PorIniciar.
                firmante.EstatusCertificado = EstatusCertificadoFirmante.Vigente;
            }

            firmante.Activo = !firmante.Activo;
            uow.Repository<Firmante>().Update(firmante);
            await uow.SaveAsync(ct);
        }

        private async Task<Firmante> ObtenerEntidadAsync(Guid id, CancellationToken ct) =>
            await uow.Repository<Firmante>().GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(Firmante), id);

        /// <summary>
        /// Certificado = número de serie en hex (la columna mide 500, no cabe el certificado en
        /// base64); vigencia y estatus se calculan del propio certificado.
        /// </summary>
        private static void AplicarCertificado(Firmante firmante, CertificadoInfo certificado)
        {
            firmante.Certificado = certificado.NumeroSerieHex;
            firmante.FechaInicioCertificado = DateOnly.FromDateTime(certificado.VigenteDesde.UtcDateTime);
            firmante.FechaFinCertificado = DateOnly.FromDateTime(certificado.VigenteHasta.UtcDateTime);

            var ahora = DateTimeOffset.UtcNow;
            firmante.EstatusCertificado =
                ahora < certificado.VigenteDesde ? EstatusCertificadoFirmante.PorIniciar :
                ahora > certificado.VigenteHasta ? EstatusCertificadoFirmante.Vencido :
                EstatusCertificadoFirmante.Vigente;
        }

        /// <summary>Nombre del firmante = nombre simple del subject del certificado (CN; si no hay, el siguiente campo de nombre que X.509 encuentre).</summary>
        private static string NombreDelCertificado(CertificadoInfo certificado) =>
            string.IsNullOrWhiteSpace(certificado.Nombre)
                ? throw new ConflictException("El certificado no trae un nombre (CN) para identificar al firmante.")
                : certificado.Nombre.Trim();

        private static void ExigirCertificadoVigente(Firmante firmante)
        {
            var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
            if (hoy < firmante.FechaInicioCertificado || hoy > firmante.FechaFinCertificado)
                throw new ConflictException(
                    $"El certificado no está vigente (vigencia {firmante.FechaInicioCertificado:yyyy-MM-dd} a {firmante.FechaFinCertificado:yyyy-MM-dd}); un firmante con certificado no vigente no puede estar Activo.");
        }

        private async Task ValidarEntidadAsync(Firmante firmante, CancellationToken ct)
        {
            var validacion = await validator.ValidateAsync(firmante, ct);
            if (!validacion.IsValid)
                throw new ConflictException(string.Join(" | ", validacion.Errors.Select(e => e.ErrorMessage)));
        }

        private static async Task<byte[]> LeerContenidoAsync(Stream stream, CancellationToken ct)
        {
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms, ct);
            return ms.ToArray();
        }

        private static bool ProximoAVencer(Firmante f)
        {
            var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
            var limite = hoy.AddDays(DiasProximoAVencer);
            return (f.FechaFinCertificado >= hoy && f.FechaFinCertificado <= limite) ||
                   (f.FechaFinAutorizada >= hoy && f.FechaFinAutorizada <= limite);
        }

        private static FirmanteResponse ToResponse(Firmante f) => new(
            f.Id, f.Nombre, f.Puesto, f.Genero,
            f.Certificado, f.EstatusCertificado,
            f.FechaInicioCertificado, f.FechaFinCertificado,
            f.FechaInicioAutorizada, f.FechaFinAutorizada,
            f.Activo, ProximoAVencer(f));
    }
}
