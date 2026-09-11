using System.ComponentModel.DataAnnotations;

namespace CartaNoAdeudoApi.Core.DTOs.Requests.Cartas
{
    public record ReprocesarSolicitudesRequest(
        [property: Required(ErrorMessage = "Debe indicar al menos una solicitud a reprocesar")]
        [property: MinLength(1, ErrorMessage = "Debe indicar al menos una solicitud a reprocesar")]
        IReadOnlyList<Guid> SolicitudIds
    );
}
