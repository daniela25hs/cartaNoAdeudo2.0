using CartaNoAdeudoApi.Core.Entities.Bitacora;

namespace CartaNoAdeudoApi.Core.Interfaces.Repositories
{
    /// <summary>
    /// <c>TareaHistorial</c> no hereda de <see cref="Entities.BaseEntity"/> (su PK es
    /// <c>int</c> autoincremental), así que no puede resolverse vía
    /// <c>IUnitOfWork.Repository&lt;T&gt;()</c>. Repo dedicado solo para eso.
    /// </summary>
    public interface ITareaHistorialRepository
    {
        Task AddAsync(TareaHistorial historial, CancellationToken ct = default);
    }
}
