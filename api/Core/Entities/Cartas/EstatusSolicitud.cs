namespace CartaNoAdeudoApi.Core.Entities.Cartas
{
    public enum EstatusSolicitud
    {
        SolicitudFirma = 1,
        Firmada        = 2,
        Pendiente      = 3,

        /// <summary>Agotó los <c>TareaFirmaOptions.MaxIntentos</c> reintentos automáticos de
        /// firma (ver <c>TareaFirmaWorker</c>); solo se reactiva a mano vía
        /// <c>ICartaService.ReprocesarSolicitudesAsync</c>.</summary>
        ErrorDefinitivo = 4
    }
}
