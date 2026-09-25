using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CartaNoAdeudoApi.Core.DTOs.Requests.Cartas
{
    public record GenerarCartaRequest(
        [property: JsonPropertyName("RFC")]
        [Required(ErrorMessage = "El valor RFC es obligatorio")]
        [RegularExpression(
            @"^([a-zA-ZñÑ&]{3,4}([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1]))([a-zA-Z\d]{3})([A\d]){0,1}?$",
            ErrorMessage = "RFC Invalida")]
        string Rfc,

        [property: JsonPropertyName("NOMBRE")]
        [Required(ErrorMessage = "Nombre obligatorio")]
        string Nombre,

        [property: JsonPropertyName("RO")]
        [Required(ErrorMessage = "RO obligatorio")]
        string Ro,

        [property: JsonPropertyName("TipoCarta")]
        [Required(ErrorMessage = "El valor Tipo de carta es obligatorio")]
        [RegularExpression(@"^([0-9]{2})$", ErrorMessage = "Tipo Carta Invalido")]
        string TipoCarta,

        [property: JsonPropertyName("InicioVigencia")]
        [Required(ErrorMessage = "El valor InicioVigencia es obligatorio")]
        string InicioVigencia,

        [property: JsonPropertyName("LicAlcoholes")]
        string? LicAlcoholes,

        [property: JsonPropertyName("email")]
        [EmailAddress(ErrorMessage = "Email Invalido")]
        string Email
    );
}
