using Microsoft.EntityFrameworkCore;
using CartaNoAdeudoApi.Core.Entities.Catalogos;
using CartaNoAdeudoApi.Core.Interfaces.Repositories;
using CartaNoAdeudoApi.Infrastructure.Data;

namespace CartaNoAdeudoApi.Infrastructure.Repositories
{
    public class EstadoTareaRepository(AppDbContext db) : IEstadoTareaRepository
    {
        public async Task<EstadoTarea?> GetByDescripcionAsync(string descripcion, CancellationToken ct = default) =>
            await db.EstadosTarea.FirstOrDefaultAsync(e => e.Descripcion == descripcion, ct);
    }
}
