using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using CartaNoAdeudoApi.Core.Entities;
using CartaNoAdeudoApi.Core.Interfaces.Repositories;
using CartaNoAdeudoApi.Infrastructure.Data;

namespace CartaNoAdeudoApi.Infrastructure.Repositories
{
    public class GenericRepository<T>(AppDbContext db) : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _db = db;

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            await _db.Set<T>().FindAsync([id], ct);

        public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default) =>
            await _db.Set<T>().AsNoTracking().ToListAsync(ct);

        public async Task AddAsync(T entity, CancellationToken ct = default) =>
            await _db.Set<T>().AddAsync(entity, ct);

        public void Update(T entity) => _db.Set<T>().Update(entity);

        public void Remove(T entity) => _db.Set<T>().Remove(entity);

        public async Task<IReadOnlyList<T>> FindAsync(
            Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
            await _db.Set<T>().Where(predicate).ToListAsync(ct);

        public Task<bool> ExistsAsync(
            Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
            _db.Set<T>().AnyAsync(predicate, ct);

        public IQueryable<T> AsQueryable() => _db.Set<T>();
    }
}
