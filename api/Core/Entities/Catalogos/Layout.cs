namespace CartaNoAdeudoApi.Core.Entities.Catalogos
{
    public class Layout : ActivableEntity
    {
        public required string Archivo { get; set; }

        public string? Descripcion { get; set; }
        public DateTimeOffset FechaHora { get; set; } = DateTimeOffset.UtcNow;
        public DateOnly FechaInicioAutorizada { get; set; }
        public DateOnly FechaFinAutorizada { get; set; }
    }
}
