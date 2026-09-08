using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CartaNoAdeudoApi.Core.Entities.Certificados;

namespace CartaNoAdeudoApi.Infrastructure.Data.Configurations
{
    public class CertificadoDigitalConfiguration : IEntityTypeConfiguration<CertificadoDigital>
    {
        public void Configure(EntityTypeBuilder<CertificadoDigital> builder)
        {
            builder.Property(c => c.NombreServidorPublico).HasMaxLength(255).IsRequired();
            builder.Property(c => c.Cargo).HasMaxLength(255);
            builder.Property(c => c.RutaArchivoCifrado).HasMaxLength(500).IsRequired();
            builder.Property(c => c.Thumbprint).HasMaxLength(64).IsRequired();

            builder.HasIndex(c => c.Thumbprint).IsUnique();

            // RN-005 (RF-001) — que no existan dos certificados activos con
            // vigencias operativas traslapadas para el mismo funcionario — es
            // una regla de rango de fechas que EF Core no expresa como
            // constraint declarativo; se valida en el servicio de aplicación
            // (Fase 3) antes de guardar.
        }
    }
}
