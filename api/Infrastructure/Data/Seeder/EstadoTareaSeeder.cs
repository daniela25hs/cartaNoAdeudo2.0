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
                if (await context.EstadosTarea.AnyAsync())
                    return;

                var estados = new List<EstadoTarea>
                {
                    new() { Descripcion = EstadoTarea.Pendiente },
                    new() { Descripcion = EstadoTarea.EnProceso },
                    new() { Descripcion = EstadoTarea.Completada },
                    new() { Descripcion = EstadoTarea.Error },
                };

                context.EstadosTarea.AddRange(estados);
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
