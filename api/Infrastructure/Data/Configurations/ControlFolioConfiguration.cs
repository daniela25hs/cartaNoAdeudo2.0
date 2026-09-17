using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CartaNoAdeudoApi.Core.Entities.Sistema;

namespace CartaNoAdeudoApi.Infrastructure.Data.Configurations
{
    public class ControlFolioConfiguration : IEntityTypeConfiguration<ControlFolio>
    {
        public void Configure(EntityTypeBuilder<ControlFolio> builder)
        {
            builder.ToTable("control_folios", "sist");

            builder.HasIndex(c => c.Anio).IsUnique();

            builder.Property<uint>("xmin").IsRowVersion();
        }
    }
}
