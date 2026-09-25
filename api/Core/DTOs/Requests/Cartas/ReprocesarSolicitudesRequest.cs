using System.ComponentModel.DataAnnotations;

namespace CartaNoAdeudoApi.Core.DTOs.Requests.Cartas
{
    public record ReprocesarSolicitudesRequest(
        [Required(ErrorMessage = "Debe indicar al menos una solicitud a reprocesar")]
        [MinLength(1, ErrorMessage = "Debe indicar al menos una solicitud a reprocesar")]
        IReadOnlyList<Guid> SolicitudIds
    );
}
