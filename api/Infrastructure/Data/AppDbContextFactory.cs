using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CartaNoAdeudoApi.Infrastructure.Data
{
    /// <summary>
    /// Fábrica usada solo por las herramientas de EF Core (<c>dotnet ef
    /// migrations</c> / <c>database update</c>). Permite generar y aplicar
    /// migraciones apuntando directamente al proyecto Infrastructure, sin
    /// depender de que el host web (proyecto API) compile.
    ///
    /// La cadena de conexión se toma de la variable de entorno
    /// </summary>
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var connectionString =
                Environment.GetEnvironmentVariable("CARTA_NO_ADEUDO__ConnectionStrings__Postgres")
                ?? "Host=localhost;Port=5432;Database=carta_no_adeudo;Username=postgres;Password=postgres";

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(
                    connectionString,
                    npgsql => npgsql.MigrationsAssembly("CartaNoAdeudoApi.Infrastructure"))
                .Options;

            return new AppDbContext(options);
        }
    }
}
