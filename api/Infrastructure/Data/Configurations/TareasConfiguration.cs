using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CartaNoAdeudoApi.Core.Entities.Cartas;

namespace CartaNoAdeudoApi.Infrastructure.Data.Configurations
{
    public class TareasConfiguration : IEntityTypeConfiguration<Tareas>
    {
        public void Configure(EntityTypeBuilder<Tareas> builder)
        {
            builder.ToTable("tareas", "carta");

            builder.Property(t => t.Nota).HasMaxLength(255);
            builder.Property(t => t.MensajeError).HasMaxLength(255);

            builder.HasIndex(t => t.IdDato);

            builder.HasOne(t => t.Estado)
                .WithMany()
                .HasForeignKey(t => t.IdEstado)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
