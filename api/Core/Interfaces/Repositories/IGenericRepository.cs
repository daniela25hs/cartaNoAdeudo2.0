using System.Linq.Expressions;
using CartaNoAdeudoApi.Core.Entities;

namespace CartaNoAdeudoApi.Core.Interfaces.Repositories
{
    /// <summary>
    /// CRUD + consultas por predicado LINQ simple. Úsalo vía
    /// <c>uow.Repository&lt;T&gt;()</c>. Crea un repositorio específico solo
    /// cuando necesites Include / proyección / SQL crudo, o encapsular una
    /// regla (ver docs/GUIA_DESARROLLO.md §1).
    /// </summary>
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
        Task AddAsync(T entity, CancellationToken ct = default);
        void Update(T entity);
        void Remove(T entity);
        Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
        IQueryable<T> AsQueryable();
    }
}
