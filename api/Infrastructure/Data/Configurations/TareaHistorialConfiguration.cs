using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CartaNoAdeudoApi.Core.Entities.Bitacora;

namespace CartaNoAdeudoApi.Infrastructure.Data.Configurations
{
    public class TareaHistorialConfiguration : IEntityTypeConfiguration<TareaHistorial>
    {
        public void Configure(EntityTypeBuilder<TareaHistorial> builder)
        {
            builder.ToTable("tarea_historial", "bit");

            builder.HasKey(h => h.Id);

            builder.Property(h => h.Estado).HasMaxLength(50).IsRequired();
            builder.Property(h => h.Mensaje).HasMaxLength(500);

            builder.HasIndex(h => h.IdTarea);
        }
    }
}
