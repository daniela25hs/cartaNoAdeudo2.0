using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CartaNoAdeudoApi.Core.Entities.Sistema;

namespace CartaNoAdeudoApi.Infrastructure.Data.Configurations
{
    public class ConfiguracionConfiguration : IEntityTypeConfiguration<Configuracion>
    {
        public void Configure(EntityTypeBuilder<Configuracion> builder)
        {
            builder.ToTable("configuracion", "sist");

            builder.Property(c => c.Clave).HasMaxLength(20).IsRequired();
            builder.Property(c => c.Valor).HasMaxLength(250).IsRequired();
            builder.Property(c => c.Descripcion).HasMaxLength(120);

            builder.HasIndex(c => c.Clave).IsUnique();
        }
    }
}
