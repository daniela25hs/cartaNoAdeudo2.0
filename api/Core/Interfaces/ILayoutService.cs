using CartaNoAdeudoApi.Core.DTOs.Requests.Layouts;
using CartaNoAdeudoApi.Core.DTOs.Responses.Layouts;

namespace CartaNoAdeudoApi.Core.Interfaces
{
    /// <summary>
    /// Alta y administración de layouts .docx (RF-002). Registrar/Actualizar extraen los
    /// marcadores del documento (<see cref="IMarcadorExtractor"/>) y los validan contra
    /// <c>Common.Layouts.LayoutMarcadores</c>: un layout no apto no puede quedar Activo
    /// (RN-007 además prohíbe que estos cambios afecten documentos ya emitidos, por eso
    /// solo se reemplaza el archivo físico, nunca el <c>Id</c>).
    /// </summary>
    public interface ILayoutService
    {
        Task<IReadOnlyList<LayoutListItemResponse>> ListarAsync(CancellationToken ct = default);
        Task<LayoutResponse> ObtenerAsync(Guid id, CancellationToken ct = default);

        /// <summary>Previsualiza los marcadores de un .docx sin guardar nada (para validar antes de dar de alta).</summary>
        Task<LayoutMarcadoresResponse> PrevisualizarMarcadoresAsync(Stream archivo, CancellationToken ct = default);

        Task<LayoutResponse> RegistrarAsync(RegistrarLayoutRequest request, Stream archivo, CancellationToken ct = default);
        Task<LayoutResponse> ActualizarAsync(Guid id, ActualizarLayoutRequest request, Stream? archivo, CancellationToken ct = default);
        Task ToggleActivoAsync(Guid id, CancellationToken ct = default);
    }
}
