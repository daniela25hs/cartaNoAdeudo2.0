using Microsoft.EntityFrameworkCore;
using CartaNoAdeudoApi.Infrastructure.Data;
using CartaNoAdeudoApi.Infrastructure.Data.Seeder;

namespace CartaNoAdeudoApi.API.Extensions
{
    public static class StartupExtensions
    {
        /// <summary>
        /// Aplica migraciones pendientes y siembra catálogos al arrancar. Si falla,
        /// la API no levanta (mejor caer temprano que servir con esquema inconsistente).
        /// </summary>
        public static async Task MigrateAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<AppDbContext>();

            try
            {
                logger.LogInformation("Aplicando migraciones...");
                await db.Database.MigrateAsync();

                logger.LogInformation("Sembrando catálogos...");
                await EstadoSeeder.SeedAsync(db, loggerFactory);
                await TipoCartaSeeder.SeedAsync(db, loggerFactory);
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Error al aplicar migraciones. La API no puede arrancar.");
                throw;
            }
        }
    }
}
