using Microsoft.EntityFrameworkCore;
using CartaNoAdeudoApi.Core.Entities.Cartas;
using CartaNoAdeudoApi.Core.Interfaces.Repositories;
using CartaNoAdeudoApi.Infrastructure.Data;

namespace CartaNoAdeudoApi.Infrastructure.Repositories
{
    public class TipoCartaRepository(AppDbContext db) : ITipoCartaRepository
    {
        public async Task<TiposCartas?> GetByClaveAsync(string clave, CancellationToken ct = default) =>
            await db.TiposCartas.FirstOrDefaultAsync(t => t.Clave == clave && t.Activo, ct);
    }
}
