namespace CartaNoAdeudoApi.Core.Options
{
    /// <summary>
    /// Configuración de la integración con SIGA (sección "Siga" de appsettings).
    /// Se enlaza en <c>AddSigaAuth</c>. Ver docs/INTEGRACION_SIGA.md.
    /// </summary>
    public class SigaOptions
    {
        public required string BaseUrl { get; set; }
        public required string Issuer { get; set; }
        public required string Audience { get; set; }

        /// <summary>GUID de esta aplicación en SIGA para el ambiente actual. Cárgalo por variable de entorno / secret manager.</summary>
        public required string AppKey { get; set; }

        /// <summary>
        /// Llaves públicas de SIGA usadas para validar la firma de los JWT de forma local.
        /// Se configuran una vez (appsettings / variables de entorno / secret manager) y no
        /// se descargan por HTTP en tiempo de ejecución. Cuando SIGA rota su clave de firma,
        /// el desarrollador responsable debe agregar/actualizar la entrada correspondiente aquí.
        /// </summary>
        public List<SigaSigningKeyOptions> SigningKeys { get; set; } = [];
    }

    /// <summary>
    /// Una llave pública RSA de SIGA en el mismo formato que expone su JWKS
    /// (kty: RSA, n/e en Base64Url), pero cargada de forma estática desde configuración.
    /// </summary>
    public class SigaSigningKeyOptions
    {
        public required string Kid { get; set; }
        public required string Alg { get; set; }

        /// <summary>Módulo RSA (n), Base64Url, tal como lo publica el JWKS de SIGA.</summary>
        public required string Modulus { get; set; }

        /// <summary>Exponente RSA (e), Base64Url, tal como lo publica el JWKS de SIGA.</summary>
        public required string Exponent { get; set; }
    }
}
