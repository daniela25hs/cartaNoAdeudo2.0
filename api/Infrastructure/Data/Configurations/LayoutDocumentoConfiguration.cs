using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CartaNoAdeudoApi.Core.Entities.Layouts;

namespace CartaNoAdeudoApi.Infrastructure.Data.Configurations
{
    public class LayoutDocumentoConfiguration : IEntityTypeConfiguration<LayoutDocumento>
    {
        public void Configure(EntityTypeBuilder<LayoutDocumento> builder)
        {
            builder.Property(l => l.Nombre).HasMaxLength(255).IsRequired();
            builder.Property(l => l.Descripcion).HasMaxLength(1000);
            builder.Property(l => l.TipoCarta).HasMaxLength(2).IsRequired();
            builder.Property(l => l.RutaArchivo).HasMaxLength(500).IsRequired();

            // Ayuda a localizar rápido "el layout vigente para este TipoCarta"
            // (RF-002, CA-005). RN-005 (no dos layouts activos traslapados
            // para el mismo TipoCarta) se valida en el servicio de aplicación.
            builder.HasIndex(l => l.TipoCarta);
        }
    }
}
