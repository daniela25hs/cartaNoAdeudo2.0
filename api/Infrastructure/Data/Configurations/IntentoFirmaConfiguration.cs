using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CartaNoAdeudoApi.Core.Entities.Cartas;

namespace CartaNoAdeudoApi.Infrastructure.Data.Configurations
{
    public class IntentoFirmaConfiguration : IEntityTypeConfiguration<IntentoFirma>
    {
        public void Configure(EntityTypeBuilder<IntentoFirma> builder)
        {
            builder.Property(i => i.MensajeError).HasMaxLength(1000);

            // La relación (incluyendo el cascade delete) queda declarada en
            // SolicitudCartaConfiguration.HasMany para no duplicarla.
            builder.HasIndex(i => i.SolicitudCartaId);
        }
    }
}
