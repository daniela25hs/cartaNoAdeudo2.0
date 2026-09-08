using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using CartaNoAdeudoApi.Core.Entities.Cartas;
using CartaNoAdeudoApi.Core.Entities.Certificados;
using CartaNoAdeudoApi.Core.Entities.Layouts;

namespace CartaNoAdeudoApi.Infrastructure.Data
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<SolicitudCarta> SolicitudesCarta { get; set; }
        public DbSet<IntentoFirma> IntentosFirma { get; set; }
        public DbSet<CertificadoDigital> CertificadosDigitales { get; set; }
        public DbSet<LayoutDocumento> LayoutsDocumento { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Toma todas las IEntityTypeConfiguration<T> de este ensamblado.
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // Nombres en snake_case automáticos (tablas y columnas) salvo que
            // una configuración los fije explícitamente.
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
