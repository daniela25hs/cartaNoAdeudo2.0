namespace CartaNoAdeudoApi.Core.Interfaces
{
    /// <summary>
    /// Repuebla <see cref="ITareaFirmaQueue"/> con las solicitudes que se quedaron en
    /// <c>EstatusSolicitud.SolicitudFirma</c> (la cola vive solo en memoria y se vacía en cada
    /// reinicio). Corre sola al arrancar la app; <see cref="RecuperarAsync"/> permite dispararla
    /// también a mano, en caliente, sin reiniciar (ver <c>CartasController</c>).
    /// </summary>
    public interface ITareaFirmaRecovery
    {
        /// <param name="resetRetries">Si es true, además reinicia <c>Datos.Intentos</c> de cada
        /// solicitud reencolada, para que vuelva a tener sus <c>TareaFirmaOptions.MaxIntentos</c>
        /// completos.</param>
        /// <returns>Cuántas solicitudes se reencolaron.</returns>
        Task<int> RecuperarAsync(bool resetRetries = false, CancellationToken ct = default);
    }
}
