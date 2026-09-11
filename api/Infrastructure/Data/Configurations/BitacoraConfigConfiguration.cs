using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CartaNoAdeudoApi.Core.Entities.Bitacora;

namespace CartaNoAdeudoApi.Infrastructure.Data.Configurations
{
    public class BitacoraConfigConfiguration : IEntityTypeConfiguration<Config>
    {
        public void Configure(EntityTypeBuilder<Config> builder)
        {
            builder.ToTable("config", "bit");

            builder.Property(c => c.Accion).HasMaxLength(100).IsRequired();
            builder.Property(c => c.Descripcion).HasMaxLength(255);
        }
    }
}
