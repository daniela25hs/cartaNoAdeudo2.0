using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CartaNoAdeudoApi.Core.Entities.Catalogos;

namespace CartaNoAdeudoApi.Infrastructure.Data.Configurations
{
    public class FirmanteConfiguration : IEntityTypeConfiguration<Firmante>
    {
        public void Configure(EntityTypeBuilder<Firmante> builder)
        {
            builder.ToTable("firmante", "cat");

            builder.Property(f => f.Nombre).HasMaxLength(255).IsRequired();
            builder.Property(f => f.Puesto).HasMaxLength(255);

            builder.Property(f => f.Genero)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(f => f.Certificado).HasMaxLength(500);
            builder.Property(f => f.Pfx).HasMaxLength(500);
            builder.Property(f => f.Contrasena).HasMaxLength(500);
            builder.Property(f => f.EstatusCertificado).HasMaxLength(50);

            builder.HasIndex(f => new { f.Activo, f.FechaInicioAutorizada, f.FechaFinAutorizada });
        }
    }
}
