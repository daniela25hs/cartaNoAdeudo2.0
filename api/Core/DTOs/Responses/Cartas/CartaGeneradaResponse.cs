namespace CartaNoAdeudoApi.Core.DTOs.Responses.Cartas
{
    public record CartaGeneradaResponse(
        Guid IdentificadorSolicitud,
        string FolioCarta,
        string Carta,
        string InicioVigencia
    );
}
