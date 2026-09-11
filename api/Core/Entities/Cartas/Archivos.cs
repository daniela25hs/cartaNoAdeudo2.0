namespace CartaNoAdeudoApi.Core.Entities.Cartas
{
    public class Archivos : BaseEntity
    {
        public Guid IdDato { get; set; }
        public required string Carta { get; set; }
        public Datos? Dato { get; set; }
    }
}
