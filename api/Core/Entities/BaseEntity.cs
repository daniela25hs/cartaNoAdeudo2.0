namespace CartaNoAdeudoApi.Core.Entities
{
    /// <summary>
    /// Campos de auditoría que llena <c>AuditoriaSaveChangesInterceptor</c>. Separado de
    /// <see cref="AuditableEntity"/> para que entidades con PK <c>int</c> (que no heredan
    /// <see cref="BaseEntity"/>, ver <c>Layout</c>) también reciban auditoría automática.
    /// </summary>
    public interface IAuditable
    {
        Guid? IdCreo { get; set; }
        DateTimeOffset FechaCreo { get; set; }
        Guid? IdEdito { get; set; }
        DateTimeOffset? FechaEdito { get; set; }
    }

    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    public abstract class AuditableEntity : BaseEntity, IAuditable
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
