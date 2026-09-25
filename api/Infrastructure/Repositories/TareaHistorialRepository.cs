using CartaNoAdeudoApi.Core.Entities.Bitacora;
using CartaNoAdeudoApi.Core.Interfaces.Repositories;
using CartaNoAdeudoApi.Infrastructure.Data;

namespace CartaNoAdeudoApi.Infrastructure.Repositories
{
    public class TareaHistorialRepository(AppDbContext db) : ITareaHistorialRepository
    {
        public async Task AddAsync(TareaHistorial historial, CancellationToken ct = default) =>
            await db.TareaHistorial.AddAsync(historial, ct);
    }
}
