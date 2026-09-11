using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CartaNoAdeudoApi.Core.Entities.Catalogos;

namespace CartaNoAdeudoApi.Infrastructure.Data.Configurations
{
    public class LayoutConfiguration : IEntityTypeConfiguration<Layout>
    {
        public void Configure(EntityTypeBuilder<Layout> builder)
        {
            builder.ToTable("layout", "cat");

            builder.Property(l => l.Archivo).HasMaxLength(500).IsRequired();
            builder.Property(l => l.Descripcion).HasMaxLength(500);

            builder.HasIndex(l => new { l.Activo, l.FechaInicioAutorizada, l.FechaFinAutorizada });
        }
    }
}
