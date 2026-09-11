using System.ComponentModel.DataAnnotations;

namespace CartaNoAdeudoApi.Core.DTOs.Requests.Cartas
{
    public record ValidarCartaRequest(
        [property: Required(ErrorMessage = "El valor RFC es obligatorio")]
        string Rfc,

        [property: Required(ErrorMessage = "La entidad federativa (ef) es obligatoria")]
        int Ef,

        [property: Required(ErrorMessage = "El folio es obligatorio")]
        int Folio
    );
}
