namespace CartaNoAdeudoApi.Core.Entities.Certificados
{
    /// <summary>
    /// Certificado digital (.pfx) emitido por la Contraloría del Estado de
    /// Sonora, usado para la firma electrónica de Cartas de No Adeudo (RF-001).
    /// El proyecto legado no modelaba esto como entidad; vivía como archivo +
    /// configuración suelta.
    /// </summary>
    public class CertificadoDigital : ActivableEntity
    {
        public required string NombreServidorPublico { get; set; }
        public string? Cargo { get; set; }

        /// <summary>Referencia al certificado almacenado de forma cifrada
        /// (ver consideraciones funcionales RF-001). Nunca se persiste la
        /// contraseña ni el .pfx en claro.</summary>
        public required string RutaArchivoCifrado { get; set; }

        /// <summary>Huella digital (thumbprint) del certificado, útil para
        /// localizarlo sin descifrar el archivo.</summary>
        public required string Thumbprint { get; set; }

        // --- Vigencia real, tomada del propio certificado (RN-001, RN-003) ---
        public DateOnly VigenciaInicio { get; set; }
        public DateOnly VigenciaFin { get; set; }

        // --- Vigencia operativa, configurada por el administrador (RN-003) ---
        public DateOnly VigenciaOperativaInicio { get; set; }
        public DateOnly VigenciaOperativaFin { get; set; }
    }
}
