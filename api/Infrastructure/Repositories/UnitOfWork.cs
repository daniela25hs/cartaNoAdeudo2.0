using Microsoft.EntityFrameworkCore.Storage;
using CartaNoAdeudoApi.Core.Entities;
using CartaNoAdeudoApi.Core.Interfaces.Repositories;
using CartaNoAdeudoApi.Infrastructure.Data;

namespace CartaNoAdeudoApi.Infrastructure.Repositories
{
    public class UnitOfWork(AppDbContext db) : IUnitOfWork
    {
        private readonly AppDbContext _db = db;
        private readonly Dictionary<Type, object> _repos = [];
        private ITipoCartaRepository? _tiposCartas;
        private ILayoutRepository? _layouts;
        private IEstadoTareaRepository? _estadosTarea;

        public IGenericRepository<T> Repository<T>() where T : BaseEntity
        {
            if (!_repos.TryGetValue(typeof(T), out var repo))
            {
                repo = new GenericRepository<T>(_db);
                _repos[typeof(T)] = repo;
            }
            return (IGenericRepository<T>)repo;
        }

        public ITipoCartaRepository TiposCartas => _tiposCartas ??= new TipoCartaRepository(_db);
        public ILayoutRepository Layouts => _layouts ??= new LayoutRepository(_db);
        public IEstadoTareaRepository EstadosTarea => _estadosTarea ??= new EstadoTareaRepository(_db);

        public Task<int> SaveAsync(CancellationToken ct = default) =>
            _db.SaveChangesAsync(ct);

        public async Task<ITransaction> BeginTransactionAsync(CancellationToken ct = default) =>
            new EfTransaction(await _db.Database.BeginTransactionAsync(ct));

        public void Dispose() => _db.Dispose();

        private sealed class EfTransaction(IDbContextTransaction tx) : ITransaction
        {
            public Task CommitAsync(CancellationToken ct = default) => tx.CommitAsync(ct);
            public Task RollbackAsync(CancellationToken ct = default) => tx.RollbackAsync(ct);
            public ValueTask DisposeAsync() => tx.DisposeAsync();
        }
    }
}
