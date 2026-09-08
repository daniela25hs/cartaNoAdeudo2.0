using System.ComponentModel.DataAnnotations;

namespace CartaNoAdeudoApi.Core.DTOs.Requests.Layouts
{
    /// <summary>
    /// Datos capturados por el administrador al cargar una plantilla .docx
    /// (RF-002). El archivo viaja como IFormFile en el controller.
    /// </summary>
    public record RegistrarLayoutRequest(
        [property: Required(ErrorMessage = "El nombre del layout es obligatorio")]
        string Nombre,

        string? Descripcion,

        [property: Required(ErrorMessage = "El tipo de carta es obligatorio")]
        [property: RegularExpression(@"^([0-9]{2})$", ErrorMessage = "Tipo Carta Invalido")]
        string TipoCarta,

        [property: Required(ErrorMessage = "La fecha de inicio de vigencia es obligatoria")]
        DateOnly VigenciaInicio,

        [property: Required(ErrorMessage = "La fecha de fin de vigencia es obligatoria")]
        DateOnly VigenciaFin
    );
}
