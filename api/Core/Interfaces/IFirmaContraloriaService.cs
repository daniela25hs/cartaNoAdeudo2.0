namespace CartaNoAdeudoApi.Core.Interfaces
{
    /// <summary>Resultado de intentar firmar una carta contra el servicio externo de firma electrónica.</summary>
    public record FirmaElectronicaResultado(
        bool Exitoso,
        string? Firma,
        string? Descripcion,
        string? MensajeError
    );

    /// <summary>
    /// Cliente hacia el servicio externo (wsLicAlcoholes / Contraloría) que resguarda el
    /// certificado PFX y firma con el web service FEA. Esta API nunca maneja el
    /// certificado ni la llave privada directamente.
    /// </summary>
    public interface IFirmaContraloriaService
    {
        Task<FirmaElectronicaResultado> FirmarAsync(
            string rfc,
            string nombre,
            string ro,
            string tipoCartaClave,
            DateOnly inicioVigencia,
            string? licAlcoholes,
            CancellationToken ct = default);
    }
}
