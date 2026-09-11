namespace CartaNoAdeudoApi.Core.DTOs.Responses.Cartas
{
    public record ReprocesoResultadoResponse(
        int Solicitados,
        int Encolados,
        IReadOnlyList<ReprocesoItemResultado> Resultados
    );
    public record ReprocesoItemResultado(
        Guid SolicitudId,
        bool Aceptado,
        string? Motivo
    );
}
