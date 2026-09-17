using CartaNoAdeudoApi.Core.Entities.Catalogos;

namespace CartaNoAdeudoApi.Core.Interfaces.Repositories
{
    /// <summary>
    /// "El layout activo y vigente a una fecha" se repite en <c>LayoutService</c> (para
    /// desactivar solapados al activar uno nuevo) y en la generación de la carta (RF-003),
    /// no es un <c>Where</c> trivial de un solo lugar — repo dedicado (ver
    /// docs/GUIA_DESARROLLO.md §1).
    /// </summary>
    public interface ILayoutRepository : IGenericRepository<Layout>
    {
        Task<Layout?> GetActivoVigenteAsync(DateOnly fecha, CancellationToken ct = default);
    }
}
