using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using CartaNoAdeudoApi.Core.Entities.Cartas;
using CartaNoAdeudoApi.Core.Entities.Catalogos;
using CartaNoAdeudoApi.Core.Exceptions;
using CartaNoAdeudoApi.Core.Interfaces;
using CartaNoAdeudoApi.Core.Interfaces.Repositories;
using CartaNoAdeudoApi.Core.Utils;

namespace CartaNoAdeudoApi.Infrastructure.Services
{
    public class FirmaCartaService(
        IUnitOfWork uow,
        ICertificadoFirmaService certificados,
        IPfxStorage storage,
        IDataProtectionProvider dataProtection,
        ILogger<FirmaCartaService> logger) : IFirmaCartaService
    {
        private readonly IDataProtector _protector = dataProtection.CreateProtector(FirmanteService.PropositoContrasena);

        public async Task<ResultadoFirmaCarta> FirmarAsync(Datos dato, string tipoCartaClave, CancellationToken ct = default)
        {
            try
            {
                var firmante = await ObtenerFirmanteAsync(ct);

                var pfx = await storage.LeerAsync(firmante.Pfx!, ct);
                var contrasena = _protector.Unprotect(firmante.Contrasena!);
                var cadena = CadenaOriginalCarta.Construir(dato, tipoCartaClave);

                var firma = certificados.Firmar(pfx, contrasena, cadena);
                return new ResultadoFirmaCarta(true, firma, cadena, firmante.Id, firmante.Nombre, firmante.Puesto, null);
            }
            catch (BusinessException ex)
            {
                logger.LogWarning("No se pudo firmar la solicitud {IdDato}: {Motivo}", dato.Id, ex.Message);
                return new ResultadoFirmaCarta(false, null, null, null, null, null, ex.Message);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // PFX borrado del disco, llaves de Data Protection perdidas, etc.
                logger.LogError(ex, "Falla inesperada al firmar la solicitud {IdDato}.", dato.Id);
                return new ResultadoFirmaCarta(false, null, null, null, null, null,
                    "Error interno al firmar la carta; revisa el PFX y la contraseña del firmante.");
            }
        }

        /// <summary>
        /// Firmante Activo con vigencia operativa que cubra hoy. Asunción (confirmar con negocio):
        /// si hay más de uno, gana el de vigencia operativa más reciente.
        /// </summary>
        private async Task<Firmante> ObtenerFirmanteAsync(CancellationToken ct)
        {
            var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
            var candidatos = await uow.Repository<Firmante>().FindAsync(f =>
                f.Activo && f.FechaInicioAutorizada <= hoy && hoy <= f.FechaFinAutorizada, ct);

            var firmante = candidatos
                .OrderByDescending(f => f.FechaInicioAutorizada)
                .FirstOrDefault()
                ?? throw new ConflictException("No hay un firmante activo con vigencia operativa para el día de hoy.");

            if (string.IsNullOrEmpty(firmante.Pfx) || string.IsNullOrEmpty(firmante.Contrasena))
                throw new ConflictException($"El firmante '{firmante.Nombre}' no tiene PFX o contraseña cargados.");

            return firmante;
        }
    }
}
