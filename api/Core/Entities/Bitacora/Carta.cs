namespace CartaNoAdeudoApi.Core.Entities.Bitacora
{
    public class Carta : BaseEntity
    {
        public Guid IdDato { get; set; }

        public required string Accion { get; set; }
        public string? Descripcion { get; set; }
        public DateTimeOffset FechaHora { get; set; } = DateTimeOffset.UtcNow;

        // Navegacion
        public Cartas.Datos? Dato { get; set; }
    }
}
