namespace CartaNoAdeudoApi.Core.Entities.Catalogos
{
    /// <summary>
    /// Frase legal autorizada para un año (tabla <c>cat.frases</c>), con vigencia
    /// propia para poder cargarse con anticipación, similar a las plantillas.
    /// </summary>
    public class Frase : ActivableEntity
    {
        public required int Anio { get; set; }
        public required string Texto { get; set; }
        public DateOnly FechaInicioAutorizada { get; set; }
        public DateOnly FechaFinAutorizada { get; set; }
    }
}
