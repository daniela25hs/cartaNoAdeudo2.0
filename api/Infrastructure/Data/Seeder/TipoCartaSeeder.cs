using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CartaNoAdeudoApi.Core.Entities.Cartas;

namespace CartaNoAdeudoApi.Infrastructure.Data.Seeder
{
    public static class TipoCartaSeeder
    {
        public static async Task SeedAsync(AppDbContext context, ILoggerFactory loggerFactory)
        {
            try
            {
                if (await context.TiposCartas.AnyAsync())
                    return;

                var tiposCartas = new List<TiposCartas>
                {
                    new() { Clave = "01", Descripcion = "GENERAL CONTRIBUYENTE CUMPLIDO", Activo = true },
                    new() { Clave = "02", Descripcion = "GENERAL CONTRIBUYENTE INSCRITO SIN OBLIGACIONES", Activo = true },
                    new() { Clave = "03", Descripcion = "ALCOHOLES CONTRIBUYENTE CUMPLIDO", Activo = true },
                };

                context.TiposCartas.AddRange(tiposCartas);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger(nameof(TipoCartaSeeder));
                logger.LogError(ex, "Error al sembrar el catálogo de tipos de carta.");
            }
        }
    }
}
