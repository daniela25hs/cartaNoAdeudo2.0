namespace CartaNoAdeudoApi.Core.Entities.Cartas
{
    /// <summary>
    /// Registro histórico de cada intento (automático o manual) de firma
    /// electrónica sobre una <see cref="SolicitudCarta"/>. Sostiene la
    /// trazabilidad exigida por RN-010 (RF-003) y RN-005/RN-010 (RF-004):
    /// fechas, reintentos, respuestas obtenidas y usuario responsable en
    /// reprocesos manuales.
    /// </summary>
    public class IntentoFirma : BaseEntity
    {
        public required Guid SolicitudCartaId { get; set; }
        public SolicitudCarta? SolicitudCarta { get; set; }

        public DateTimeOffset FechaIntento { get; set; } = DateTimeOffset.UtcNow;
        public bool Exitoso { get; set; }
        public string? MensajeError { get; set; }

        /// <summary>Null cuando el intento fue automático (reintentos de RF-003);
        /// con valor cuando se disparó desde el módulo de reproceso manual (RF-004).</summary>
        public Guid? EjecutadoPor { get; set; }
    }
}
