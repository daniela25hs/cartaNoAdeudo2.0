namespace CartaNoAdeudoApi.Core.Interfaces
{
    /// <summary>Convierte un .docx a PDF invocando LibreOffice headless como proceso externo
    /// (no hay conversor puramente managed que preserve el layout de Word). Ver
    /// <c>PdfOptions</c> y el paquete <c>libreoffice-writer</c> del Dockerfile.</summary>
    public interface IPdfConverter
    {
        Task<byte[]> ConvertirAsync(byte[] docx, CancellationToken ct = default);
    }
}
