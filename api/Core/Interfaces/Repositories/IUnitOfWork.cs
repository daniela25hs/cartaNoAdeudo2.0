using CartaNoAdeudoApi.Core.Entities;

namespace CartaNoAdeudoApi.Core.Interfaces.Repositories
{
    /// <summary>
    /// A medida que agregues entidades con consultas propias, expón aquí su
    /// repositorio específico (ej. <c>IUsuarioRepository Usuarios { get; }</c>)
    /// con lazy-init, siguiendo el mismo patrón que <c>Repository&lt;T&gt;()</c>.
    /// Ver docs/GUIA_DESARROLLO.md §1 para el criterio.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : BaseEntity;
        ITipoCartaRepository TiposCartas { get; }
        ILayoutRepository Layouts { get; }
        IEstadoTareaRepository EstadosTarea { get; }
        Task<int> SaveAsync(CancellationToken ct = default);
        Task<ITransaction> BeginTransactionAsync(CancellationToken ct = default);
    }
}
