using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using CartaNoAdeudoApi.Core.Interfaces;
using CartaNoAdeudoApi.Core.Options;

namespace CartaNoAdeudoApi.Infrastructure.Services
{
    public class EmailService(IOptions<EmailOptions> options, ILogger<EmailService> logger) : IEmailService
    {
        private readonly EmailOptions _options = options.Value;

        public async Task EnviarAsync(
            string destinatario,
            string asunto,
            string cuerpoHtml,
            IReadOnlyList<ArchivoAdjunto>? adjuntos = null,
            IReadOnlyList<ImagenEnlazada>? imagenesEnlazadas = null,
            CancellationToken ct = default)
        {
            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress(_options.FromNombre, _options.From ?? _options.SmtpUsuario));
            mensaje.To.Add(MailboxAddress.Parse(destinatario));
            mensaje.Subject = asunto;

            var builder = new BodyBuilder { HtmlBody = cuerpoHtml };
            foreach (var adjunto in adjuntos ?? [])
                builder.Attachments.Add(adjunto.NombreArchivo, adjunto.Contenido, ContentType.Parse(adjunto.ContentType));
            foreach (var imagen in imagenesEnlazadas ?? [])
            {
                var recurso = builder.LinkedResources.Add(imagen.ContentId, imagen.Contenido, ContentType.Parse(imagen.ContentType));
                recurso.ContentId = imagen.ContentId;
            }
            mensaje.Body = builder.ToMessageBody();

            using var cliente = new SmtpClient();
            try
            {
                await cliente.ConnectAsync(
                    _options.SmtpServidor,
                    _options.SmtpPuerto,
                    _options.HabilitarSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None,
                    ct);
                await cliente.AuthenticateAsync(_options.SmtpUsuario, _options.SmtpClave, ct);
                await cliente.SendAsync(mensaje, ct);
            }
            finally
            {
                if (cliente.IsConnected)
                    await cliente.DisconnectAsync(true, ct);
            }

            logger.LogInformation("Correo '{Asunto}' enviado a {Destinatario}.", asunto, destinatario);
        }
    }
}
