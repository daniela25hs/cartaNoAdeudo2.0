using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CartaNoAdeudoApi.Core.Entities.Cartas;

namespace CartaNoAdeudoApi.Infrastructure.Data.Configurations
{
    public class FirmasConfiguration : IEntityTypeConfiguration<Firmas>
    {
        public void Configure(EntityTypeBuilder<Firmas> builder)
        {
            builder.ToTable("firmas", "carta");

            builder.Property(f => f.Id).HasColumnName("id_firma");

            builder.Property(f => f.Descripcion).HasMaxLength(500);
            builder.Property(f => f.Identificador).HasMaxLength(255);
            builder.Property(f => f.Certificado).HasMaxLength(500);
            builder.Property(f => f.HexSerie).HasMaxLength(255);
            builder.Property(f => f.FingerPrint).HasMaxLength(255);

            builder.HasIndex(f => f.IdDato);
        }
    }
}
