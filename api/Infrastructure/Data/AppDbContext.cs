using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using CartaNoAdeudoApi.Core.Entities.Cartas;
using CartaNoAdeudoApi.Core.Entities.Catalogos;
using CartaNoAdeudoApi.Core.Entities.Sistema;
using Bit = CartaNoAdeudoApi.Core.Entities.Bitacora;

namespace CartaNoAdeudoApi.Infrastructure.Data
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // --- Esquema carta ---
        public DbSet<Datos> Datos { get; set; }
        public DbSet<Archivos> Archivos { get; set; }
        public DbSet<Tareas> Tareas { get; set; }
        public DbSet<Firmas> Firmas { get; set; }
        public DbSet<TiposCartas> TiposCartas { get; set; }
        public DbSet<FolioConsecutivo> FoliosConsecutivos { get; set; }

        // --- Esquema cat ---
        public DbSet<Estado> Estados { get; set; }
        public DbSet<EstadoTarea> EstadosTarea { get; set; }
        public DbSet<Layout> Layouts { get; set; }
        public DbSet<Firmante> Firmantes { get; set; }

        // --- Esquema bit ---
        public DbSet<Bit.Carta> BitacoraCartas { get; set; }
        public DbSet<Bit.Config> BitacoraConfigs { get; set; }

        // --- Esquema sist ---
        public DbSet<Configuracion> Configuraciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entity.GetTableName();
                if (!string.IsNullOrEmpty(tableName) && tableName == entity.ClrType.Name)
                    entity.SetTableName(ToSnakeCase(tableName));

                foreach (var property in entity.GetProperties())
                    if (property.GetColumnName() == property.Name)
                        property.SetColumnName(ToSnakeCase(property.Name));
            }
        }

        private static string ToSnakeCase(string name) =>
            SnakeCaseRegex().Replace(name, "$1_$2").ToLowerInvariant();

        [GeneratedRegex("([a-z0-9])([A-Z])")]
        private static partial Regex SnakeCaseRegex();
    }
}
