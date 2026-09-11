using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CartaNoAdeudoApi.Core.Entities.Cartas;

namespace CartaNoAdeudoApi.Infrastructure.Data.Configurations
{
    public class FolioConsecutivoConfiguration : IEntityTypeConfiguration<FolioConsecutivo>
    {
        public void Configure(EntityTypeBuilder<FolioConsecutivo> builder)
        {
            builder.ToTable("folios_consecutivos", "carta");

            builder.Property(f => f.TipoCarta).HasMaxLength(2).IsRequired();
            builder.Property(f => f.Descripcion).HasMaxLength(255);

            builder.HasIndex(f => f.TipoCarta).IsUnique();

            builder.Property<uint>("xmin").IsRowVersion();
        }
    }
}
