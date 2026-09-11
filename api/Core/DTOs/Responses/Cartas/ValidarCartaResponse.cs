namespace CartaNoAdeudoApi.Core.DTOs.Responses.Cartas
{
    public enum ResultadoValidacion
    {
        Autentico,
        NoAutentico,
        NoEncontrado
    }

    public record ValidarCartaResponse(
        ResultadoValidacion Resultado,
        string? Folio,
        string? NombreServidorPublicoFirmante,
        DateTimeOffset? FechaEmision,
        bool? CertificadoVigenteAlMomentoDeFirma
    );
}
