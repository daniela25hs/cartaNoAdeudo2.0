namespace CartaNoAdeudoApi.Core.Interfaces
{
    /// <summary>Datos que se le avisan a SAP sobre el resultado de una solicitud (RF-003, paso 11).</summary>
    public record NotificacionSapPayload(
        Guid IdentificadorSolicitud,
        string Rfc,
        string? Folio,
        string Estatus
    );

    /// <summary>
    /// Notifica a SAP el resultado de una solicitud de carta ya firmada. SAP todavía no entregó
    /// el contrato real de su endpoint receptor (URL, body esperado, autenticación) — mientras
    /// tanto <c>NotificacionSapService</c> solo simula el envío (log), como el prototipo
    /// <c>api-tareas-firma.EnviarSapAsync</c> del que salió el patrón de reintentos (ver
    /// <c>TareaFirmaWorker</c>). Reemplazar por un cliente HTTP real en cuanto SAP dé su endpoint;
    /// nunca debe lanzar (la carta ya quedó Firmada y persistida antes de llegar aquí).
    /// </summary>
    public interface INotificacionSapService
    {
        Task NotificarAsync(NotificacionSapPayload payload, CancellationToken ct = default);
    }
}
