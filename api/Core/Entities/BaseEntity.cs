namespace CartaNoAdeudoApi.Core.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    public abstract class AuditableEntity : BaseEntity
    {
        public Guid? IdCreo { get; set; }
        public DateTimeOffset FechaCreo { get; set; } = DateTimeOffset.UtcNow;
        public Guid? IdEdito { get; set; }
        public DateTimeOffset? FechaEdito { get; set; }
    }

    public abstract class ActivableEntity : AuditableEntity
    {
        public bool Activo { get; set; } = true;
    }
}
