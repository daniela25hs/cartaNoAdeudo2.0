namespace CartaNoAdeudoApi.Core.Options
{
    /// <summary>
    /// Generación del PDF final de la carta (RF-002/RF-003): combina el layout .docx activo
    /// con los datos de la solicitud ya firmada y lo convierte a PDF con LibreOffice headless.
    /// Sección "Pdf" de appsettings.
    /// </summary>
    public class PdfOptions
    {
        /// <summary>Ruta al ejecutable de LibreOffice dentro del contenedor/host.</summary>
        public string LibreOfficePath { get; set; } = "soffice";

        public int TimeoutSegundos { get; set; } = 60;

        /// <summary>
        /// Base de la URL pública de validación que se codifica en el QR de la carta
        /// (se arma como "{ValidarUrlBase}?rfc={Rfc}&amp;folio={Folio}"). TODO: confirmar con
        /// negocio la URL real una vez exista la pantalla pública de validación en el front.
        /// </summary>
        public string ValidarUrlBase { get; set; } = string.Empty;
    }
}
