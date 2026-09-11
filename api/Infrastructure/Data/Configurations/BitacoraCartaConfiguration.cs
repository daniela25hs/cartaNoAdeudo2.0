using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CartaNoAdeudoApi.Core.Entities.Bitacora;

namespace CartaNoAdeudoApi.Infrastructure.Data.Configurations
{
    public class BitacoraCartaConfiguration : IEntityTypeConfiguration<Carta>
    {
        public void Configure(EntityTypeBuilder<Carta> builder)
        {
            builder.ToTable("carta", "bit");

            builder.Property(b => b.Accion).HasMaxLength(100).IsRequired();
            builder.Property(b => b.Descripcion).HasMaxLength(255);

            builder.HasIndex(b => b.IdDato);
        }
    }
}
