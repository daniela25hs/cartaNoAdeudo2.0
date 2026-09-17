using CartaNoAdeudoApi.Core.Entities.Catalogos;

namespace CartaNoAdeudoApi.Core.Interfaces.Repositories
{
    /// <summary>
    /// <c>EstadoTarea</c> no hereda de <see cref="Entities.BaseEntity"/> (su PK es
    /// <c>int</c> autoincremental), así que no puede resolverse vía
    /// <c>IUnitOfWork.Repository&lt;T&gt;()</c>. Repo dedicado solo para eso.
    /// </summary>
    public interface IEstadoTareaRepository
    {
        Task<EstadoTarea?> GetByDescripcionAsync(string descripcion, CancellationToken ct = default);
    }
}
