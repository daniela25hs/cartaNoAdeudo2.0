using System.Net;
using System.Reflection;

namespace CartaNoAdeudoApi.Core.Utils
{
    /// <summary>
    /// Cuerpo HTML del correo que recibe el contribuyente cuando su carta queda Firmada
    /// (<c>CartaService.GenerarDocumentoYCorreoAsync</c>). Maqueta tomada del proyecto
    /// api-fideicomisos (EmailTemplates.BodyUserNew) para mantener el mismo estilo visual
    /// entre los correos de gobierno del estado.
    /// </summary>
    public static class EmailTemplates
    {
        private const string PortalUrl = "https://cuentaunica.siiafhacienda.gob.mx/Account/Login";
        private const string ValidarUrl = "https://hacienda.sonora.gob.mx/validar-carta-de-no-adeudo";

        /// <summary>Content-ID con el que el header de <see cref="BodyConstanciaEmitida"/> referencia
        /// el escudo vía <c>cid:</c>; quien llama a <c>IEmailService.EnviarAsync</c> debe adjuntarlo
        /// como <c>ImagenEnlazada(EscudoContentId, EscudoBytes, EscudoContentType)</c>
        /// (ver <c>CartaService.GenerarDocumentoYCorreoAsync</c>).</summary>
        public const string EscudoContentId = "escudo-sonora-blanco";
        public const string EscudoContentType = "image/png";

        /// <summary>PNG (no SVG) porque varios clientes de correo, sobre todo Outlook de
        /// escritorio, no renderizan SVG dentro del cuerpo del mensaje.</summary>
        public static byte[] EscudoBytes { get; } = LeerRecursoEmbebido("EscudoSonoraBlanco.png");

        private static byte[] LeerRecursoEmbebido(string nombreLogico)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(nombreLogico)
                ?? throw new InvalidOperationException($"No se encontró el recurso embebido '{nombreLogico}'.");
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }

        public static string BodyConstanciaEmitida(DateTimeOffset fechaSolicitud, bool tieneAdjunto)
        {
            var fecha = WebUtility.HtmlEncode(fechaSolicitud.ToString("d 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("es-MX")));

            var notaAdjunto = tieneAdjunto
                ? "le informo que su constancia ha sido expedida y se anexa al presente correo."
                : "le informo que su constancia ha sido expedida; en breve recibirá el PDF por este mismo medio.";

            return $@"
        <body style=""margin: 0; padding: 0; font-family: Helvetica, Arial, sans-serif; background-color: #f4f4f4;"">
            <div style=""max-width: 680px; margin: 0 auto; background-color: #f4f4f4; border-radius: 24px; box-shadow: 0 0 2rem rgba(0,0,0,0.1); overflow: hidden;"">

                <div style=""padding: 32px;"">
                    <h1 style=""font-size: 24px; font-weight: bold; color: #410123; text-align: center;"">Estimado Contribuyente</h1>

                    <div style=""background-color: #410123; padding: 32px; text-align: center; border-radius: 12px; margin-bottom: 24px;"">
                        <img src=""cid:{EscudoContentId}"" alt=""Escudo del Estado de Sonora"" width=""55"" height=""64"" style=""display: inline-block; width: 55px; height: 64px;"" />
                    </div>

                    <p style=""font-size: 14px; color: #333;"">
                        Respecto a su solicitud de Constancia de No Adeudo de Contribuciones Estatales y Federales Coordinadas
                        efectuada en Cuenta Única el día <strong>{fecha}</strong>, {notaAdjunto}
                    </p>
                    <p style=""font-size: 14px; color: #333;"">
                        De igual forma podrá descargar su Constancia desde nuestro portal:
                    </p>
                    <ol style=""font-size: 14px; color: #333; padding-left: 20px;"">
                        <li>Ingresar a Cuenta Única <a href=""{PortalUrl}"" style=""color: #960E53;"">{PortalUrl}</a></li>
                        <li>Entrar al trámite CARTA DE NO ADEUDO</li>
                        <li>Ingresar a CARTAS TRAMITADAS</li>
                        <li>Seleccionar Obtener Constancia</li>
                    </ol>
                    <p style=""font-size: 14px; color: #333;"">
                        Podrá validarse la autenticidad y vigencia de las Constancias de No Adeudo mediante la lectura del
                        Código QR o bien, ingresando a <a href=""{ValidarUrl}"" style=""color: #960E53;"">{ValidarUrl}</a>
                    </p>
                    <p style=""font-size: 14px; color: #333;"">
                        ¡Fue un placer atenderle, que tenga excelente día!
                    </p>
                    <p style=""font-size: 14px; color: #333;"">
                        Atentamente
                    </p>
                    <p style=""font-size: 14px; color: #333; margin: 0;"">
                        Dirección General de Recaudación<br/>
                        Subsecretaría de Ingresos<br/>
                        Secretaría de Hacienda
                    </p>
                </div>

                <div style=""padding: 16px 32px;"">
                    <hr style=""border: none; border-top: 1px solid #ddd;"" />
                    <p style=""font-size: 12px; color: #777;"">
                        Le recordamos nuestros canales de comunicación para brindarle mayor atención:<br/>
                        Dirección de Orientación y Asistencia al Contribuyente<br/>
                        Teléfono 800 3127011<br/>
                        Correo electrónico: infocontribuyente@sonora.gob.mx<br/>
                        Página: https://haciendasonora.gob.mx
                    </p>
                </div>

                <div style=""background-color: #410123; padding: 24px; text-align: center; color: #ffffff;"">
                    <p style=""margin: 0; font-size: 12px;"">
                        <a href=""https://www.sonora.gob.mx/gobierno/politicas-de-uso-y-privacidad"" style=""color: #ffffff; margin: 0 8px;"">Política de Uso y Privacidad</a> |
                        <a href=""https://denunciapp.sonora.gob.mx/"" style=""color: #ffffff; margin: 0 8px;"" target=""_blank"" rel=""noopener noreferrer"">Denuncia Ciudadana</a> |
                        <a href=""https://www.sonora.gob.mx/contacto"" style=""color: #ffffff; margin: 0 8px;"" target=""_blank"" rel=""noopener noreferrer"">Contacto</a>
                    </p>
                    <p style=""margin-top: 8px; font-size: 12px;"">{DateTime.Now.Year}. Gobierno del Estado de Sonora. Todos los derechos reservados.</p>
                </div>
            </div>
        </body>
    ";
        }
    }
}
