using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using CartaNoAdeudoApi.Core.Entities.Catalogos;
using CartaNoAdeudoApi.Core.Interfaces.Repositories;
using CartaNoAdeudoApi.Infrastructure.Data;

namespace CartaNoAdeudoApi.Infrastructure.Repositories
{
    public class LayoutRepository(AppDbContext db) : ILayoutRepository
    {
        public async Task<Layout?> GetByIdAsync(int id, CancellationToken ct = default) =>
            await db.Layouts.FindAsync([id], ct);

        public async Task<IReadOnlyList<Layout>> GetAllAsync(CancellationToken ct = default) =>
            await db.Layouts.AsNoTracking().ToListAsync(ct);

        public async Task AddAsync(Layout entity, CancellationToken ct = default) =>
            await db.Layouts.AddAsync(entity, ct);

        public void Update(Layout entity) => db.Layouts.Update(entity);

        public async Task<IReadOnlyList<Layout>> FindAsync(
            Expression<Func<Layout, bool>> predicate, CancellationToken ct = default) =>
            await db.Layouts.Where(predicate).ToListAsync(ct);

        public Task<Layout?> GetActivoVigenteAsync(DateOnly fecha, CancellationToken ct = default) =>
            db.Layouts.FirstOrDefaultAsync(l =>
                l.Activo && l.FechaInicioAutorizada <= fecha && fecha <= l.FechaFinAutorizada, ct);
    }
}
