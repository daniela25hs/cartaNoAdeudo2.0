using System.ComponentModel.DataAnnotations;
using CartaNoAdeudoApi.Core.Entities.Catalogos;

namespace CartaNoAdeudoApi.Core.DTOs.Requests.Firmantes
{
    public record RegistrarFirmanteRequest(
        [property: Required(ErrorMessage = "La contraseña del certificado es obligatoria")]
        string Contrasena,

        [property: Required(ErrorMessage = "El nombre del servidor público es obligatorio")]
        string Nombre,

        string? Puesto,

        Genero Genero,

        [property: Required(ErrorMessage = "La fecha de inicio de vigencia operativa es obligatoria")]
        DateOnly VigenciaOperativaInicio,

        [property: Required(ErrorMessage = "La fecha de fin de vigencia operativa es obligatoria")]
        DateOnly VigenciaOperativaFin,
        bool Activo
    );

    public record ActualizarFirmanteRequest(
        [property: Required(ErrorMessage = "El nombre del servidor público es obligatorio")]
        string Nombre,

        string? Puesto,

        Genero Genero,

        [property: Required(ErrorMessage = "La fecha de inicio de vigencia operativa es obligatoria")]
        DateOnly VigenciaOperativaInicio,

        [property: Required(ErrorMessage = "La fecha de fin de vigencia operativa es obligatoria")]
        DateOnly VigenciaOperativaFin,

        string? Contrasena
    );
}
