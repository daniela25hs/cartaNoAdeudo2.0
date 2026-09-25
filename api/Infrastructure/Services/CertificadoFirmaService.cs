using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CartaNoAdeudoApi.Core.Exceptions;
using CartaNoAdeudoApi.Core.Interfaces;

namespace CartaNoAdeudoApi.Infrastructure.Services
{
    public class CertificadoFirmaService : ICertificadoFirmaService
    {
        public CertificadoInfo LeerCertificado(byte[] pfx, string contrasena)
        {
            using var cert = AbrirPfx(pfx, contrasena);
            return ExtraerInfo(cert);
        }

        public CertificadoInfo LeerCertificadoPublico(string certificadoBase64)
        {
            try
            {
                using var cert = X509CertificateLoader.LoadCertificate(Convert.FromBase64String(certificadoBase64));
                return ExtraerInfo(cert);
            }
            catch (Exception ex) when (ex is FormatException or CryptographicException)
            {
                throw new ConflictException("El certificado almacenado no es válido.");
            }
        }

        public FirmaGenerada Firmar(byte[] pfx, string contrasena, string cadena)
        {
            ArgumentException.ThrowIfNullOrEmpty(cadena);

            using var cert = AbrirPfx(pfx, contrasena);
            var info = ExtraerInfo(cert);

            var ahora = DateTimeOffset.UtcNow;
            if (ahora < info.VigenteDesde || ahora > info.VigenteHasta)
                throw new ConflictException(
                    $"El certificado no está vigente (vigencia {info.VigenteDesde:yyyy-MM-dd} a {info.VigenteHasta:yyyy-MM-dd}).");

            using var rsa = cert.GetRSAPrivateKey()
                ?? throw new ConflictException("El certificado del PFX no tiene una llave privada RSA.");

            var firma = rsa.SignData(Encoding.UTF8.GetBytes(cadena), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return new FirmaGenerada(Convert.ToBase64String(firma), info);
        }

        public bool Verificar(string certificadoBase64, string cadena, string firmaBase64)
        {
            byte[] certBytes, firma;
            try
            {
                certBytes = Convert.FromBase64String(certificadoBase64);
                firma = Convert.FromBase64String(firmaBase64);
            }
            catch (FormatException)
            {
                return false;
            }

            try
            {
                using var cert = X509CertificateLoader.LoadCertificate(certBytes);
                using var rsa = cert.GetRSAPublicKey();
                return rsa is not null &&
                       rsa.VerifyData(Encoding.UTF8.GetBytes(cadena), firma, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
            catch (CryptographicException)
            {
                return false;
            }
        }

        /// <summary>
        /// Abre el PFX y devuelve el certificado que trae llave privada (un PFX puede
        /// incluir además la cadena de certificados intermedios/raíz, que se descartan).
        /// </summary>
        private static X509Certificate2 AbrirPfx(byte[] pfx, string contrasena)
        {
            X509Certificate2Collection coleccion;
            try
            {
                coleccion = X509CertificateLoader.LoadPkcs12Collection(
                    pfx, contrasena, X509KeyStorageFlags.EphemeralKeySet);
            }
            catch (CryptographicException)
            {
                // Contraseña incorrecta y archivo corrupto llegan como la misma excepción.
                throw new ConflictException("No se pudo abrir el PFX: contraseña incorrecta o archivo inválido.");
            }

            var conLlave = coleccion.FirstOrDefault(c => c.HasPrivateKey)
                ?? throw new ConflictException("El PFX no contiene un certificado con llave privada.");

            foreach (var c in coleccion.Where(c => !ReferenceEquals(c, conLlave)))
                c.Dispose();

            return conLlave;
        }

        private static CertificadoInfo ExtraerInfo(X509Certificate2 cert) => new(
            Nombre: cert.GetNameInfo(X509NameType.SimpleName, forIssuer: false),
            Subject: cert.Subject,
            Emisor: cert.GetNameInfo(X509NameType.SimpleName, forIssuer: true),
            NumeroSerieHex: cert.SerialNumber,
            Huella: cert.Thumbprint,
            VigenteDesde: new DateTimeOffset(cert.NotBefore.ToUniversalTime(), TimeSpan.Zero),
            VigenteHasta: new DateTimeOffset(cert.NotAfter.ToUniversalTime(), TimeSpan.Zero),
            CertificadoBase64: Convert.ToBase64String(cert.RawData));
    }
}
