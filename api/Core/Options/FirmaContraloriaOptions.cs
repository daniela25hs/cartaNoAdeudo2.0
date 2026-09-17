namespace CartaNoAdeudoApi.Core.Options
{
    /// <summary>
    /// Configuración del cliente HTTP hacia el servicio externo de firma electrónica
    /// (wsLicAlcoholes / Contraloría) que resguarda el certificado PFX y firma contra
    /// el web service FEA. Sección "FirmaContraloria" de appsettings.
    /// </summary>
    public class FirmaContraloriaOptions
    {
        public required string BaseUrl { get; set; }
        public int TimeoutSegundos { get; set; } = 30;
        public int MaxReintentos { get; set; } = 3;
        public int IntervaloReintentoSegundos { get; set; } = 60;
    }
}
