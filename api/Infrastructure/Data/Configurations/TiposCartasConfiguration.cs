using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CartaNoAdeudoApi.Core.Entities.Cartas;

namespace CartaNoAdeudoApi.Infrastructure.Data.Configurations
{
    public class TiposCartasConfiguration : IEntityTypeConfiguration<TiposCartas>
    {
        public void Configure(EntityTypeBuilder<TiposCartas> builder)
        {
            builder.ToTable("tipos_cartas", "carta");

            builder.Property(t => t.Id).HasColumnName("id_tipo_carta");

            builder.Property(t => t.Descripcion).HasMaxLength(50).IsRequired();
            builder.Property(t => t.Clave).HasMaxLength(2).IsRequired();

            builder.HasIndex(t => t.Clave).IsUnique();
        }
    }
}
