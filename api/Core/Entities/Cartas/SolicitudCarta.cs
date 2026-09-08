using CartaNoAdeudoApi.Core.Entities.Certificados;
using CartaNoAdeudoApi.Core.Entities.Layouts;

namespace CartaNoAdeudoApi.Core.Entities.Cartas
{
    /// <summary>
    /// Solicitud de generación de una Carta de No Adeudo recibida desde SAP
    /// (RF-003). Reemplaza a los antiguos <c>RequestFirmaCarta</c> / <c>ExisteRO</c>
    /// / <c>Reporte</c> del proyecto legado, unificando en una sola entidad los
    /// datos de entrada, el resultado de la firma y la evidencia de no repudio
    /// (RF-005). El <see cref="BaseEntity.Id"/> heredado es el identificador único
    /// de trazabilidad exigido por RN-003 (RF-003).
    /// </summary>
    public class SolicitudCarta : AuditableEntity
    {
        // --- Datos recibidos de SAP (antes: RequestFirmaCarta / ExisteRO) ---
        public required string Rfc { get; set; }
        public required string Nombre { get; set; }

        /// <summary>Registro oficial. No es único por sí solo: la combinación
        /// Ro + TipoCarta es lo que identifica una solicitud repetida
        /// (ver antiguo FirmaProvider.ExisteROFirma).</summary>
        public required string Ro { get; set; }
        public required string TipoCarta { get; set; }
        public DateOnly InicioVigencia { get; set; }

        /// <summary>Obligatorio únicamente cuando TipoCarta == "03".</summary>
        public string? LicAlcoholes { get; set; }
        public required string Email { get; set; }

        // --- Ciclo de vida (RF-003 / RF-004) ---
        public EstatusSolicitud Estatus { get; set; } = EstatusSolicitud.Recibida;
        public int IntentosFirma { get; set; }
        public DateTimeOffset? FechaUltimoIntento { get; set; }
        public string? UltimoError { get; set; }

        // --- Insumos utilizados para generar/firmar (RF-001 / RF-002) ---
        public Guid? CertificadoDigitalId { get; set; }
        public CertificadoDigital? CertificadoDigital { get; set; }
        public Guid? LayoutDocumentoId { get; set; }
        public LayoutDocumento? LayoutDocumento { get; set; }

        // --- Resultado de la emisión ---
        /// <summary>Folio visible impreso en el documento (distinto del Id interno).</summary>
        public string? Folio { get; set; }

        /// <summary>Ruta o clave de almacenamiento del PDF firmado (no se persiste
        /// el binario/base64 directamente en la fila).</summary>
        public string? RutaPdfFirmado { get; set; }
        public DateTimeOffset? FechaFirmado { get; set; }

        // --- Evidencia de no repudio (RF-005) ---
        /// <summary>Hash (SHA-256) del PDF firmado, calculado al momento de la firma.</summary>
        public string? HashDocumento { get; set; }

        /// <summary>Sello de tiempo de la firma electrónica, tal como lo reporta
        /// el servicio de Firma Electrónica Estatal.</summary>
        public DateTimeOffset? SelloTiempoFirma { get; set; }

        public ICollection<IntentoFirma> Intentos { get; set; } = new List<IntentoFirma>();
    }
}
