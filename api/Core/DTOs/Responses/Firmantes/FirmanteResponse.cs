using CartaNoAdeudoApi.Core.Entities.Catalogos;

namespace CartaNoAdeudoApi.Core.DTOs.Responses.Firmantes
{
    public record FirmanteResponse(
        Guid Id,
        string Nombre,
        string? Puesto,
        Genero Genero,

        string? Certificado,
        string? EstatusCertificado,

        DateOnly FechaInicioCertificado,
        DateOnly FechaFinCertificado,

        DateOnly VigenciaOperativaInicio,
        DateOnly VigenciaOperativaFin,

        bool Activo,

        bool ProximoAVencer
    );

    public record FirmanteListItemResponse(
        Guid Id,
        string Nombre,
        string? Puesto,
        string? EstatusCertificado,
        DateOnly VigenciaOperativaInicio,
        DateOnly VigenciaOperativaFin,
        bool Activo,
        bool ProximoAVencer
    );
}
