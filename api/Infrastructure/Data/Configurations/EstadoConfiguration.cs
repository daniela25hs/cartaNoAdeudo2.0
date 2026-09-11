using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CartaNoAdeudoApi.Core.Entities.Catalogos;

namespace CartaNoAdeudoApi.Infrastructure.Data.Configurations
{
    public class EstadoConfiguration : IEntityTypeConfiguration<Estado>
    {
        public void Configure(EntityTypeBuilder<Estado> builder)
        {
            builder.ToTable("estado", "cat");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.Descripcion).HasMaxLength(50).IsRequired();
            builder.HasIndex(e => e.Descripcion).IsUnique();
        }
    }
}
