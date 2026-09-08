namespace CartaNoAdeudoApi.Core.DTOs.Responses.Cartas
{
    /// <summary>Resultado posible de una validación de autenticidad (RF-005, CA-003/004/005).</summary>
    public enum ResultadoValidacion
    {
        Autentico,
        NoAutentico,
        NoEncontrado
    }

    /// <summary>
    /// Respuesta del módulo público de validación de autenticidad / no repudio
    /// (RF-005). Reemplaza al par duplicado <c>RequestValidarCarta</c> / <c>validar</c>
    /// del proyecto legado, que en realidad describían la misma consulta.
    /// </summary>
    public record ValidarCartaResponse(
        ResultadoValidacion Resultado,
        string? Folio,
        string? NombreServidorPublicoFirmante,
        DateTimeOffset? FechaEmision,
        bool? CertificadoVigenteAlMomentoDeFirma
    );
}
