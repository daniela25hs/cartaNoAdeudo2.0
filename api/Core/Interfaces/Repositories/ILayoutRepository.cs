using System.Linq.Expressions;
using CartaNoAdeudoApi.Core.Entities.Catalogos;

namespace CartaNoAdeudoApi.Core.Interfaces.Repositories
{
    /// <summary>
    /// <c>Layout</c> no hereda de <see cref="Entities.BaseEntity"/> (su PK es <c>int</c>
    /// autoincremental), así que no puede resolverse vía <c>IUnitOfWork.Repository&lt;T&gt;()</c>.
    /// "El layout activo y vigente a una fecha" se repite en <c>LayoutService</c> (para
    /// desactivar solapados al activar uno nuevo) y en la generación de la carta (RF-003),
    /// no es un <c>Where</c> trivial de un solo lugar — repo dedicado (ver
    /// docs/GUIA_DESARROLLO.md §1).
    /// </summary>
    public interface ILayoutRepository
    {
        Task<Layout?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<Layout>> GetAllAsync(CancellationToken ct = default);
        Task AddAsync(Layout entity, CancellationToken ct = default);
        void Update(Layout entity);
        Task<IReadOnlyList<Layout>> FindAsync(Expression<Func<Layout, bool>> predicate, CancellationToken ct = default);
        Task<Layout?> GetActivoVigenteAsync(DateOnly fecha, CancellationToken ct = default);
    }
}
