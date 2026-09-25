using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CartaNoAdeudoApi.Core.Entities.Catalogos;

namespace CartaNoAdeudoApi.Infrastructure.Data.Seeder
{
    public static class EstadoTareaSeeder
    {
        public static async Task SeedAsync(AppDbContext context, ILoggerFactory loggerFactory)
        {
            try
            {
                var descripciones = new[]
                {
                    EstadoTarea.Pendiente,
                    EstadoTarea.EnProceso,
                    EstadoTarea.Completada,
                    EstadoTarea.Error,
                    EstadoTarea.ErrorDefinitivo,
                };

                // Por descripción (no AnyAsync) para que un estado agregado después
                // (como ErrorDefinitivo) también se siembre en una BD ya sembrada antes.
                var existentes = await context.EstadosTarea.Select(e => e.Descripcion).ToListAsync();

                var faltantes = descripciones.Except(existentes)
                    .Select(d => new EstadoTarea { Descripcion = d })
                    .ToList();

                if (faltantes.Count == 0)
                    return;

                context.EstadosTarea.AddRange(faltantes);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger(nameof(EstadoTareaSeeder));
                logger.LogError(ex, "Error al sembrar el catálogo de estados de tarea.");
            }
        }
    }
}
