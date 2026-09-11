using CartaNoAdeudoApi.Core.Entities.Cartas;

namespace CartaNoAdeudoApi.Core.DTOs.Responses.Cartas
{
    public record SolicitudListItemResponse(
        Guid Id,
        string Rfc,
        string Nombre,
        string TipoCarta,

        string? Folio,
        EstatusSolicitud Estatus,
        bool Vencida,
        int Intentos,
        DateTimeOffset? FechaFirmado,
        DateTimeOffset FechaHora
    );

    public record SolicitudDetalleResponse(
        Guid Id,

        string Rfc,
        string Nombre,
        string RO,
        string TipoCarta,
        DateOnly InicioVigencia,
        string? Alcoholes,
        string Email,

        EstatusSolicitud Estatus,
        string? Folio,
        DateOnly Vencimiento,
        bool Vencida,
        DateTimeOffset? FechaFirmado,

        IReadOnlyList<TareaResponse> Tareas,
        IReadOnlyList<BitacoraCartaResponse> Bitacora,
        IReadOnlyList<FirmaResponse> Firmas
    );

    public record TareaResponse(
        Guid Id,
        string Estado,
        DateTimeOffset FechaInicio,
        DateTimeOffset? FechaFin,
        string? Nota,
        string? MensajeError
    );

    public record BitacoraCartaResponse(
        string Accion,
        string? Descripcion,
        DateTimeOffset FechaHora
    );

    public record FirmaResponse(
        DateTimeOffset Fecha,
        string? Identificador,
        string? Certificado,
        string? HexSerie,
        string? FingerPrint
    );
}
