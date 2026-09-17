using Microsoft.EntityFrameworkCore;
using CartaNoAdeudoApi.Core.Entities.Catalogos;
using CartaNoAdeudoApi.Core.Interfaces.Repositories;
using CartaNoAdeudoApi.Infrastructure.Data;

namespace CartaNoAdeudoApi.Infrastructure.Repositories
{
    public class LayoutRepository(AppDbContext db) : GenericRepository<Layout>(db), ILayoutRepository
    {
        public Task<Layout?> GetActivoVigenteAsync(DateOnly fecha, CancellationToken ct = default) =>
            _db.Layouts.FirstOrDefaultAsync(l =>
                l.Activo && l.FechaInicioAutorizada <= fecha && fecha <= l.FechaFinAutorizada, ct);
    }
}
