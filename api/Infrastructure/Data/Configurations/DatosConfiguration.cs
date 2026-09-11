using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CartaNoAdeudoApi.Core.Entities.Bitacora;
using CartaNoAdeudoApi.Core.Entities.Cartas;

namespace CartaNoAdeudoApi.Infrastructure.Data.Configurations
{
    public class DatosConfiguration : IEntityTypeConfiguration<Datos>
    {
        public void Configure(EntityTypeBuilder<Datos> builder)
        {
            builder.ToTable("datos", "carta");

            builder.Property(d => d.Id).HasColumnName("id_dato");

            builder.Property(d => d.Rfc).HasMaxLength(13).IsRequired();
            builder.Property(d => d.Nombre).HasMaxLength(255).IsRequired();
            builder.Property(d => d.RO).HasColumnName("ro").HasMaxLength(25).IsRequired();
            builder.Property(d => d.Email).HasMaxLength(100).IsRequired();
            builder.Property(d => d.Alcoholes).HasMaxLength(20);
            builder.Property(d => d.Folio).HasMaxLength(100);

            builder.Property(d => d.Estatus)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.HasIndex(d => new { d.RO, d.IdTipoCarta });

            builder.HasOne(d => d.TipoCarta)
                .WithMany(t => t.Datos)
                .HasForeignKey(d => d.IdTipoCarta)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.Archivo)
                .WithOne(a => a.Dato)
                .HasForeignKey<Archivos>(a => a.IdDato)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(d => d.Tareas)
                .WithOne(t => t.Dato)
                .HasForeignKey(t => t.IdDato)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(d => d.Firmas)
                .WithOne(f => f.Dato)
                .HasForeignKey(f => f.IdDato)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(d => d.Bitacora)
                .WithOne(b => b.Dato)
                .HasForeignKey(b => b.IdDato)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
