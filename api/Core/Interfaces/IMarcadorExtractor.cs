namespace CartaNoAdeudoApi.Core.Interfaces
{
    /// <summary>
    /// Lee un .docx y regresa los marcadores <c>{{Campo}}</c> que contiene, sin
    /// evaluarlos contra la whitelist (eso lo hace <c>ILayoutService</c>). Depende de
    /// una librería externa de OpenXml, por eso va detrás de interfaz.
    /// </summary>
    public interface IMarcadorExtractor
    {
        Task<IReadOnlyList<string>> ExtraerAsync(Stream docx, CancellationToken ct = default);
    }
}
