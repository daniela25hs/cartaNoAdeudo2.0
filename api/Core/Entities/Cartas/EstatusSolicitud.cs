namespace CartaNoAdeudoApi.Core.Entities.Cartas
{
    /// <summary>
    /// Ciclo de vida de una <see cref="SolicitudCarta"/>, ver RF-003 y RF-004.
    /// </summary>
    public enum EstatusSolicitud
    {
        Recibida = 0,
        EnProceso = 1,
        Firmando = 2,
        Completada = 3,
        Pendiente = 4,
        Error = 5
    }
}
