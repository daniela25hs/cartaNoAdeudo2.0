using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CartaNoAdeudoApi.Core.DTOs.Requests.Cartas
{
    /// <summary>
    /// Payload que envía SAP al endpoint de generación (RF-003). Reemplaza a
    /// <c>RequestFirmaCarta</c> del proyecto legado; se conservan las mismas
    /// validaciones de formato (RFC, tipo de carta, correo).
    /// Los nombres de propiedad JSON respetan el contrato ya definido con SAP.
    /// </summary>
    public record GenerarCartaRequest(
        [property: JsonPropertyName("RFC")]
        [property: Required(ErrorMessage = "El valor RFC es obligatorio")]
        [property: RegularExpression(
            @"^([a-zA-ZñÑ&]{3,4}([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1]))([a-zA-Z\d]{3})([A\d]){0,1}?$",
            ErrorMessage = "RFC Invalida")]
        string Rfc,

        [property: JsonPropertyName("NOMBRE")]
        [property: Required(ErrorMessage = "Nombre obligatorio")]
        string Nombre,

        [property: JsonPropertyName("RO")]
        [property: Required(ErrorMessage = "RO obligatorio")]
        string Ro,

        [property: JsonPropertyName("TipoCarta")]
        [property: Required(ErrorMessage = "El valor Tipo de carta es obligatorio")]
        [property: RegularExpression(@"^([0-9]{2})$", ErrorMessage = "Tipo Carta Invalido")]
        string TipoCarta,

        [property: JsonPropertyName("InicioVigencia")]
        [property: Required(ErrorMessage = "El valor InicioVigencia es obligatorio")]
        string InicioVigencia,

        [property: JsonPropertyName("LicAlcoholes")]
        string? LicAlcoholes,

        [property: JsonPropertyName("email")]
        [property: EmailAddress(ErrorMessage = "Email Invalido")]
        string Email
    );
}
