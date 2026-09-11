using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CartaNoAdeudoApi.Core.Entities.Cartas;

namespace CartaNoAdeudoApi.Infrastructure.Data.Configurations
{
    public class ArchivosConfiguration : IEntityTypeConfiguration<Archivos>
    {
        public void Configure(EntityTypeBuilder<Archivos> builder)
        {
            builder.ToTable("archivos", "carta");

            builder.Property(a => a.Carta).IsRequired();

            builder.HasIndex(a => a.IdDato).IsUnique();
        }
    }
}
