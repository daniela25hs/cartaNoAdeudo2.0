using CartaNoAdeudoApi.Core.DTOs.Requests.Firmantes;
using CartaNoAdeudoApi.Core.DTOs.Responses.Firmantes;

namespace CartaNoAdeudoApi.Core.Interfaces
{
    /// <summary>
    /// Alta y administración de firmantes. El PFX se abre con su contraseña
    /// (<see cref="ICertificadoFirmaService"/>) y de él salen certificado (serie), vigencia y
    /// estatus; el request solo aporta la vigencia operativa. La contraseña se guarda cifrada
    /// y el .pfx en disco; ninguno de los dos sale nunca en una respuesta.
    /// </summary>
    public interface IFirmanteService
    {
        Task<IReadOnlyList<FirmanteListItemResponse>> ListarAsync(CancellationToken ct = default);
        Task<FirmanteResponse> ObtenerAsync(Guid id, CancellationToken ct = default);
        Task<FirmanteResponse> RegistrarAsync(RegistrarFirmanteRequest request, Stream pfx, CancellationToken ct = default);

        /// <summary>
        /// Con <paramref name="pfx"/> reemplaza el PFX (requiere <c>Contrasena</c>); sin él, una
        /// <c>Contrasena</c> se valida contra el PFX guardado y reemplaza la almacenada.
        /// </summary>
        Task<FirmanteResponse> ActualizarAsync(Guid id, ActualizarFirmanteRequest request, Stream? pfx, CancellationToken ct = default);
        Task ToggleActivoAsync(Guid id, CancellationToken ct = default);
    }
}
