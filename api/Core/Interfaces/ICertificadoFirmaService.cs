namespace CartaNoAdeudoApi.Core.Interfaces
{
    /// <summary>Datos públicos extraídos del certificado contenido en un PFX.</summary>
    public record CertificadoInfo(
        string Nombre,
        string Subject,
        string Emisor,
        string NumeroSerieHex,
        string Huella,
        DateTimeOffset VigenteDesde,
        DateTimeOffset VigenteHasta,
        string CertificadoBase64
    );

    /// <summary>Resultado de firmar una cadena: firma en base64 + certificado que la generó.</summary>
    public record FirmaGenerada(string FirmaBase64, CertificadoInfo Certificado);

    /// <summary>
    /// Firma electrónica local con un PFX (certificado + llave privada). La llave privada
    /// solo vive en memoria durante la llamada; nunca se exporta a disco como .key.
    /// Algoritmo: RSA PKCS#1 v1.5 con SHA-256 sobre la cadena en UTF-8, resultado en base64.
    /// </summary>
    public interface ICertificadoFirmaService
    {
        /// <summary>Abre el PFX con su contraseña y devuelve los datos del certificado (periodo activo, nombre, serie...).</summary>
        CertificadoInfo LeerCertificado(byte[] pfx, string contrasena);

        /// <summary>Datos de un certificado público (DER en base64), sin PFX ni llave privada. Lanza si el base64 o el certificado son inválidos.</summary>
        CertificadoInfo LeerCertificadoPublico(string certificadoBase64);

        /// <summary>Firma <paramref name="cadena"/> con la llave privada del PFX. Rechaza certificados no vigentes.</summary>
        FirmaGenerada Firmar(byte[] pfx, string contrasena, string cadena);

        /// <summary>Verifica una firma con la llave pública del certificado (no requiere el PFX ni la contraseña).</summary>
        bool Verificar(string certificadoBase64, string cadena, string firmaBase64);
    }
}
