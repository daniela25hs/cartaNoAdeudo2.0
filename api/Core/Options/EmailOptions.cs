namespace CartaNoAdeudoApi.Core.Options
{
    /// <summary>
    /// Credenciales SMTP para el correo de notificación que recibe el solicitante cuando su
    /// carta queda Firmada (<c>CartaService.IntentarFirmarAsync</c>). Sección "Email" de
    /// appsettings; en desarrollo/producción los valores reales van por variables de entorno
    /// <c>CARTA_NO_ADEUDO__Email__*</c> (ver docker-compose.yml), nunca committeados en
    /// appsettings.json.
    /// </summary>
    public class EmailOptions
    {
        public required string SmtpServidor { get; set; }
        public int SmtpPuerto { get; set; } = 587;
        public required string SmtpUsuario { get; set; }
        public required string SmtpClave { get; set; }
        public bool HabilitarSsl { get; set; } = true;

        /// <summary>Remitente que ve el destinatario; si se deja vacío se usa <see cref="SmtpUsuario"/>.</summary>
        public string? From { get; set; }
        public string FromNombre { get; set; } = "Carta de No Adeudo";
    }
}
