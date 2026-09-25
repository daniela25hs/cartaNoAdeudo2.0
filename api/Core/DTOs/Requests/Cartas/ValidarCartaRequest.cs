using System.ComponentModel.DataAnnotations;

namespace CartaNoAdeudoApi.Core.DTOs.Requests.Cartas
{
    public record ValidarCartaRequest(
        [Required(ErrorMessage = "El valor RFC es obligatorio")]
        string Rfc,

        [Required(ErrorMessage = "La entidad federativa (ef) es obligatoria")]
        int Ef,

        [Required(ErrorMessage = "El folio es obligatorio")]
        int Folio
    );
}
