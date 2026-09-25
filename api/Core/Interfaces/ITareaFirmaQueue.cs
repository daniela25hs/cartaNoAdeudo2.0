namespace CartaNoAdeudoApi.Core.Interfaces
{
    /// <summary>Solicitud pendiente de (re)intentar firmar.</summary>
    public record TareaFirmaItem(Guid IdDato, string TipoCartaClave);

    /// <summary>
    /// Cola en memoria de solicitudes por firmar/reintentar, consumida por
    /// <c>TareaFirmaWorker</c>. Al vivir solo en memoria del proceso, se vacía en cada
    /// reinicio; <c>TareaFirmaRecovery</c> la repuebla al arrancar con lo que haya quedado
    /// en <c>EstatusSolicitud.SolicitudFirma</c>.
    /// </summary>
    public interface ITareaFirmaQueue
    {
        ValueTask EncolarAsync(TareaFirmaItem item, CancellationToken ct = default);
        IAsyncEnumerable<TareaFirmaItem> LeerTodoAsync(CancellationToken ct = default);
    }
}
