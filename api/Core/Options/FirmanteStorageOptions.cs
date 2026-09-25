namespace CartaNoAdeudoApi.Core.Options
{
    /// <summary>
    /// Dónde se guardan los .pfx de <c>Firmante</c> y las llaves de Data Protection con las
    /// que se cifra <c>Firmante.Contrasena</c>. Sección "FirmanteStorage" de appsettings;
    /// rutas relativas se resuelven contra <c>AppContext.BaseDirectory</c>.
    /// Ambas carpetas deben persistir entre reinicios/despliegues: si se pierde
    /// <see cref="LlavesProteccionPath"/>, las contraseñas guardadas ya no se pueden descifrar.
    /// </summary>
    public class FirmanteStorageOptions
    {
        public string PfxPath { get; set; } = "App_Data/pfx";
        public string LlavesProteccionPath { get; set; } = "App_Data/dp-keys";
    }
}
