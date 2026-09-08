namespace CartaNoAdeudoApi.Core.Entities
{
    /// <summary>
    /// Raíz de la jerarquía de entidades. Id es Guid y se genera en la app
    /// (no depende de una secuencia de la BD).
    /// </summary>
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    /// <summary>
    /// Entidades con rastro de auditoría. El
    /// <see cref="Infrastructure.Data.Interceptors.AuditoriaSaveChangesInterceptor"/>
    /// rellena estos campos automáticamente en cada SaveChanges.
    /// </summary>
    public abstract class AuditableEntity : BaseEntity
    {
        public Guid? CreadoPor { get; set; }
        public string? CreadoPorEmail { get; set; }
        public DateTimeOffset FechaCreacion { get; set; } = DateTimeOffset.UtcNow;
        public Guid? EditadoPor { get; set; }
        public string? EditadoPorEmail { get; set; }
        public DateTimeOffset? FechaEdicion { get; set; }
    }

    /// <summary>
    /// Entidades que se habilitan o deshabilitan sin borrarse físicamente
    /// (soft delete / activación). Se togglea vía PATCH .../toggle.
    /// </summary>
    public abstract class ActivableEntity : AuditableEntity
    {
        public bool Activo { get; set; } = true;
    }
}
