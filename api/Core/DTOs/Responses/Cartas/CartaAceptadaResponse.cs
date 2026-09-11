using CartaNoAdeudoApi.Core.Entities.Cartas;

namespace CartaNoAdeudoApi.Core.DTOs.Responses.Cartas
{
    public record CartaAceptadaResponse(
        Guid IdentificadorSolicitud,
        EstatusSolicitud Estatus
    );
}
