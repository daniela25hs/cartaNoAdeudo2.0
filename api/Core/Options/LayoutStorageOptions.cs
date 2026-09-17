namespace CartaNoAdeudoApi.Core.Options
{
    /// <summary>
    /// Carpeta donde se guardan los .docx de <c>Layout</c> (RF-002). A diferencia de
    /// <c>Archivos.Carta</c> (PDF final, base64 en la BD), el layout se guarda en disco:
    /// <c>Layout.Archivo</c> solo referencia el nombre generado. Sección "LayoutStorage"
    /// de appsettings; ruta relativa se resuelve contra <c>AppContext.BaseDirectory</c>.
    /// </summary>
    public class LayoutStorageOptions
    {
        public required string BasePath { get; set; }
    }
}
