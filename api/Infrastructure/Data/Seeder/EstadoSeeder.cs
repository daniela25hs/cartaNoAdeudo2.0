using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CartaNoAdeudoApi.Core.Entities.Cartas;
using CartaNoAdeudoApi.Core.Entities.Catalogos;

namespace CartaNoAdeudoApi.Infrastructure.Data.Seeder
{
    public static class EstadoSeeder
    {
        public static async Task SeedAsync(AppDbContext context, ILoggerFactory loggerFactory)
        {
            try
            {
                if (await context.Estados.AnyAsync())
                    return;

                var estados = Enum.GetValues<EstatusSolicitud>()
                    .Select(estatus => new Estado { Id = (int)estatus, Descripcion = estatus.ToString() })
                    .ToList();

                context.Estados.AddRange(estados);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger(nameof(EstadoSeeder));
                logger.LogError(ex, "Error al sembrar el catálogo de estados.");
            }
        }
    }
}
