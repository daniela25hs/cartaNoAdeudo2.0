namespace CartaNoAdeudoApi.Core.DTOs.Responses.Cartas
{
    /// <summary>
    /// Respuesta tras generar y firmar una Carta de No Adeudo. Unifica los
    /// antiguos <c>CartaM</c> y <c>CartaFirma</c> (eran casi idénticos) y agrega
    /// el identificador de trazabilidad que RF-003 (RN-003) exige devolver a SAP.
    /// </summary>
    public record CartaGeneradaResponse(
        Guid IdentificadorSolicitud,
        string FolioCarta,
        /// <summary>PDF firmado codificado en Base64.</summary>
        string Carta,
        string InicioVigencia
    );
}
