using System.ComponentModel.DataAnnotations;

namespace CartaNoAdeudoApi.Core.DTOs.Requests.Cartas
{
    /// <summary>
    /// Datos capturados por el ciudadano/dependencia en el módulo público de
    /// validación de autenticidad (RF-005). Unifica los antiguos
    /// <c>RequestValidarCarta</c> y <c>validar</c>, que eran duplicados.
    /// </summary>
    public record ValidarCartaRequest(
        [property: Required(ErrorMessage = "El valor RFC es obligatorio")]
        string Rfc,

        [property: Required(ErrorMessage = "La entidad federativa (ef) es obligatoria")]
        int Ef,

        [property: Required(ErrorMessage = "El folio es obligatorio")]
        int Folio
    );
}
