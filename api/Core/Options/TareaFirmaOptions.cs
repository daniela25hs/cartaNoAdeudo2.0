namespace CartaNoAdeudoApi.Core.Options
{
    /// <summary>
    /// Reintentos automáticos de firma en background. Sección "TareaFirma" de appsettings.
    /// Ver <c>ITareaFirmaQueue</c> / <c>TareaFirmaWorker</c> / <c>TareaFirmaRecovery</c>.
    /// </summary>
    public class TareaFirmaOptions
    {
        /// <summary>Intentos totales de firma por solicitud (el primero es síncrono en
        /// <c>SolicitarCartaAsync</c>; los siguientes los hace <c>TareaFirmaWorker</c> en
        /// background). Al llegar aquí, la solicitud pasa a <c>EstatusSolicitud.ErrorDefinitivo</c>.</summary>
        public int MaxIntentos { get; set; } = 3;

        /// <summary>Cuántos reintentos puede procesar <c>TareaFirmaWorker</c> al mismo tiempo.</summary>
        public int MaxConcurrencia { get; set; } = 2;

        /// <summary>Espera antes de reintentar una firma que falló, para no golpear el PFX/
        /// certificado de inmediato tras un error.</summary>
        public int EsperaReintentoSegundos { get; set; } = 30;
    }
}
