using CartaNoAdeudoApi.Core.Entities.Catalogos;

namespace CartaNoAdeudoApi.Core.Entities.Cartas
{
    public class Tareas : BaseEntity
    {
        public Guid IdDato { get; set; }
        public int IdEstado { get; set; }
        public DateTimeOffset FechaInicio { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? FechaFin { get; set; }
        public string? Nota { get; set; }
        public string? MensajeError { get; set; }
        public Datos? Dato { get; set; }
        public EstadoTarea? Estado { get; set; }
    }
}
