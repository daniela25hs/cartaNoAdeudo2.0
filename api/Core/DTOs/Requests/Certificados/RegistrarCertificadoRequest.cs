using System.ComponentModel.DataAnnotations;

namespace CartaNoAdeudoApi.Core.DTOs.Requests.Certificados
{
    /// <summary>
    /// Datos capturados por el administrador al cargar un certificado .pfx
    /// (RF-001). El archivo en sí viaja como IFormFile en el controller, no
    /// aquí; este record cubre los metadatos que acompañan la carga.
    /// </summary>
    public record RegistrarCertificadoRequest(
        [property: Required(ErrorMessage = "La contraseña del certificado es obligatoria")]
        string Contrasena,

        [property: Required(ErrorMessage = "El nombre del servidor público es obligatorio")]
        string NombreServidorPublico,

        string? Cargo,

        [property: Required(ErrorMessage = "La fecha de inicio de vigencia operativa es obligatoria")]
        DateOnly VigenciaOperativaInicio,

        [property: Required(ErrorMessage = "La fecha de fin de vigencia operativa es obligatoria")]
        DateOnly VigenciaOperativaFin
    );
}
