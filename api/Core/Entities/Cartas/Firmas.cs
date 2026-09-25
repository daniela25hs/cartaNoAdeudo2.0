namespace CartaNoAdeudoApi.Core.Entities.Cartas
{
    public class Firmas : BaseEntity
    {
        public Guid IdDato { get; set; }

        /// <summary>Nombre del servidor público firmante (de ahí sale <c>NombreServidorPublicoFirmante</c> al validar).</summary>
        public string? Descripcion { get; set; }
        public DateTimeOffset Fecha { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>Id del <c>Firmante</c> cuyo PFX generó la firma.</summary>
        public string? Identificador { get; set; }

        /// <summary>Certificado público (DER en base64) con el que se puede verificar <see cref="Firma"/>, sin depender del PFX.</summary>
        public string? Certificado { get; set; }
        public string? HexSerie { get; set; }
        public string? FingerPrint { get; set; }

        /// <summary>Cadena original exacta que se firmó (<c>CadenaOriginalCarta</c>); se guarda para que cambiar el formato después no invalide firmas ya emitidas.</summary>
        public string? CadenaOriginal { get; set; }

        /// <summary>Firma RSA-SHA256 de <see cref="CadenaOriginal"/>, en base64.</summary>
        public string? Firma { get; set; }

        // Navegacion
        public Datos? Dato { get; set; }
    }
}
