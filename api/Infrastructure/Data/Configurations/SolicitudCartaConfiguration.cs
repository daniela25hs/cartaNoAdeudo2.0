using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CartaNoAdeudoApi.Core.Entities.Cartas;

namespace CartaNoAdeudoApi.Infrastructure.Data.Configurations
{
    public class SolicitudCartaConfiguration : IEntityTypeConfiguration<SolicitudCarta>
    {
        public void Configure(EntityTypeBuilder<SolicitudCarta> builder)
        {
            builder.Property(s => s.Rfc).HasMaxLength(13).IsRequired();
            builder.Property(s => s.Nombre).HasMaxLength(255).IsRequired();
            builder.Property(s => s.Ro).HasMaxLength(50).IsRequired();
            builder.Property(s => s.TipoCarta).HasMaxLength(2).IsRequired();
            builder.Property(s => s.LicAlcoholes).HasMaxLength(50);
            builder.Property(s => s.Email).HasMaxLength(255).IsRequired();

            // Se guarda como texto (no int) para que la BD sea legible sin
            // consultar el enum; ver Core.Entities.Cartas.EstatusSolicitud.
            builder.Property(s => s.Estatus)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(s => s.Folio).HasMaxLength(50);
            builder.Property(s => s.HashDocumento).HasMaxLength(64); // SHA-256 en hex

            // Acelera la búsqueda de "¿ya existe una carta para este RO y tipo?"
            // (antes: FirmaProvider.ExisteROFirma). No es único: el proyecto
            // legado sí permite reintentos que generan más de una fila para el
            // mismo par mientras la anterior no quedó Completada.
            builder.HasIndex(s => new { s.Ro, s.TipoCarta });

            builder.HasOne(s => s.CertificadoDigital)
                .WithMany()
                .HasForeignKey(s => s.CertificadoDigitalId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.LayoutDocumento)
                .WithMany()
                .HasForeignKey(s => s.LayoutDocumentoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(s => s.Intentos)
                .WithOne(i => i.SolicitudCarta)
                .HasForeignKey(i => i.SolicitudCartaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
