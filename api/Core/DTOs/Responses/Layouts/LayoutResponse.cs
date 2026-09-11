namespace CartaNoAdeudoApi.Core.DTOs.Responses.Layouts
{
    public record LayoutResponse(
        Guid Id,
        string? Descripcion,

        /// <summary>Nombre / ruta del archivo .docx almacenado (Layout.Archivo).</summary>
        string Archivo,
        DateTimeOffset FechaHora,
        DateOnly VigenciaInicio,
        DateOnly VigenciaFin,
        bool Activo,
        IReadOnlyList<string> Marcadores
    );

    public record LayoutListItemResponse(
        Guid Id,
        string? Descripcion,
        DateOnly VigenciaInicio,
        DateOnly VigenciaFin,
        bool Activo
    );

    public record LayoutMarcadoresResponse(
        IReadOnlyList<string> MarcadoresReconocidos,
        IReadOnlyList<string> MarcadoresNoReconocidos,
        IReadOnlyList<string> MarcadoresObligatoriosFaltantes,
        bool AptoParaActivar
    );
}
