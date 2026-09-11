namespace CartaNoAdeudoApi.Core.Entities.Cartas
{
    public class Firmas : BaseEntity
    {
        public Guid IdDato { get; set; }

        public string? Descripcion { get; set; }
        public DateTimeOffset Fecha { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>Identificador devuelto por el servicio de Firma Electrónica Estatal.</summary>
        public string? Identificador { get; set; }

        public string? Certificado { get; set; }
        public string? HexSerie { get; set; }
        public string? FingerPrint { get; set; }

        /// <summary>Valor de la firma / hash del documento firmado.</summary>
        public string? Firma { get; set; }

        // Navegacion
        public Datos? Dato { get; set; }
    }
}
