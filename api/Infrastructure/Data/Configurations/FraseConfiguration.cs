using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CartaNoAdeudoApi.Core.Entities.Catalogos;

namespace CartaNoAdeudoApi.Infrastructure.Data.Configurations
{
    public class FraseConfiguration : IEntityTypeConfiguration<Frase>
    {
        public void Configure(EntityTypeBuilder<Frase> builder)
        {
            builder.ToTable("frases", "cat");

            builder.Property(f => f.Texto).HasColumnName("frase").HasMaxLength(1000).IsRequired();

            builder.HasIndex(f => f.Anio);
        }
    }
}
