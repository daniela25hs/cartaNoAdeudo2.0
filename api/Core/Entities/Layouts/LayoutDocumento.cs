namespace CartaNoAdeudoApi.Core.Entities.Layouts
{
    /// <summary>
    /// Plantilla Word (.docx) con marcadores dinámicos usada para generar el
    /// documento de la Carta de No Adeudo (RF-002).
    /// </summary>
    public class LayoutDocumento : ActivableEntity
    {
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; }

        /// <summary>Tipo de carta al que aplica este layout (campo "TipoCarta"
        /// de la solicitud, ej. "01", "03", "99").</summary>
        public required string TipoCarta { get; set; }

        public required string RutaArchivo { get; set; }

        public DateOnly VigenciaInicio { get; set; }
        public DateOnly VigenciaFin { get; set; }
    }
}
