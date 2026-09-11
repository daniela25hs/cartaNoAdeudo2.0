using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CartaNoAdeudoApi.Core.Entities.Catalogos;

namespace CartaNoAdeudoApi.Infrastructure.Data.Configurations
{
    public class EstadoTareaConfiguration : IEntityTypeConfiguration<EstadoTarea>
    {
        public void Configure(EntityTypeBuilder<EstadoTarea> builder)
        {
            builder.ToTable("estado_tarea", "cat");

            builder.Property(e => e.Descripcion).HasMaxLength(100).IsRequired();
            builder.HasIndex(e => e.Descripcion).IsUnique();
        }
    }
}
