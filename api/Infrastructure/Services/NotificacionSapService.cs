using Microsoft.Extensions.Logging;
using CartaNoAdeudoApi.Core.Interfaces;

namespace CartaNoAdeudoApi.Infrastructure.Services
{
    /// <summary>
    /// TODO: SAP aún no entrega su endpoint real de recepción. Esta implementación es un stub
    /// (solo log, sin llamada HTTP) para que el resto del flujo (RF-003) ya tenga el punto de
    /// notificación cableado; cuando exista el contrato de SAP, reemplazar el cuerpo de
    /// <see cref="NotificarAsync"/> por un <c>HttpClient</c> real y mover la URL/credenciales a
    /// opciones (mismo criterio que <c>EmailOptions</c>/Siga: nunca hardcodeadas ni en el
    /// appsettings versionado).
    /// </summary>
    public class NotificacionSapService(ILogger<NotificacionSapService> logger) : INotificacionSapService
    {
        public Task NotificarAsync(NotificacionSapPayload payload, CancellationToken ct = default)
        {
            logger.LogInformation(
                "[STUB SAP] Notificando solicitud {IdentificadorSolicitud} (RFC {Rfc}, folio {Folio}): {Estatus}",
                payload.IdentificadorSolicitud, payload.Rfc, payload.Folio, payload.Estatus);

            return Task.CompletedTask;
        }
    }
}
