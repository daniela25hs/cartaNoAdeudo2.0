using System.ComponentModel.DataAnnotations;
using CartaNoAdeudoApi.Core.Entities.Catalogos;

namespace CartaNoAdeudoApi.Core.DTOs.Requests.Firmantes
{
    // El nombre del firmante no se captura: se extrae del certificado del PFX (CN del subject).
    public record RegistrarFirmanteRequest(
        [Required(ErrorMessage = "La contraseña del certificado es obligatoria")]
        string Contrasena,

        string? Puesto,

        Genero Genero,

        [Required(ErrorMessage = "La fecha de inicio de vigencia operativa es obligatoria")]
        DateOnly VigenciaOperativaInicio,

        [Required(ErrorMessage = "La fecha de fin de vigencia operativa es obligatoria")]
        DateOnly VigenciaOperativaFin,
        bool Activo
    );

    public record ActualizarFirmanteRequest(
        string? Puesto,

        Genero Genero,

        [Required(ErrorMessage = "La fecha de inicio de vigencia operativa es obligatoria")]
        DateOnly VigenciaOperativaInicio,

        [Required(ErrorMessage = "La fecha de fin de vigencia operativa es obligatoria")]
        DateOnly VigenciaOperativaFin,

        string? Contrasena
    );
}
