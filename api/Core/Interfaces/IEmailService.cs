namespace CartaNoAdeudoApi.Core.Interfaces
{
    public record ArchivoAdjunto(string NombreArchivo, byte[] Contenido, string ContentType);

    /// <summary>Imagen referenciada desde el HTML del correo con <c>cid:ContentId</c> (p. ej. el
    /// escudo del encabezado en <c>EmailTemplates</c>), en vez de un adjunto que el destinatario
    /// ve como archivo aparte.</summary>
    public record ImagenEnlazada(string ContentId, byte[] Contenido, string ContentType);

    /// <summary>Envío de correo por SMTP (MailKit). No reintenta: quien llama decide qué hacer
    /// si <see cref="EnviarAsync"/> lanza (ver uso en <c>CartaService.IntentarFirmarAsync</c>,
    /// que solo registra el error sin afectar el estatus de la solicitud ya firmada).</summary>
    public interface IEmailService
    {
        Task EnviarAsync(
            string destinatario,
            string asunto,
            string cuerpoHtml,
            IReadOnlyList<ArchivoAdjunto>? adjuntos = null,
            IReadOnlyList<ImagenEnlazada>? imagenesEnlazadas = null,
            CancellationToken ct = default);
    }
}
