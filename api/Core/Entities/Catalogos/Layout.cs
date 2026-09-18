using CartaNoAdeudoApi.Core.Entities;

namespace CartaNoAdeudoApi.Core.Entities.Catalogos
{
    /// <summary>
    /// No hereda de <see cref="Entities.BaseEntity"/>: su PK es <c>int</c> autoincremental,
    /// igual que <c>TiposCartas</c>/<c>Estado</c>/<c>EstadoTarea</c>, así que se resuelve vía
    /// <see cref="Interfaces.Repositories.ILayoutRepository"/> en vez de
    /// <c>IUnitOfWork.Repository&lt;T&gt;()</c>. Implementa <see cref="IAuditable"/> para
    /// seguir recibiendo auditoría automática de <c>AuditoriaSaveChangesInterceptor</c>.
    /// </summary>
    public class Layout : IAuditable
    {
        public int Id { get; set; }

        public required string Archivo { get; set; }

        public string? Descripcion { get; set; }
        public DateTimeOffset FechaHora { get; set; } = DateTimeOffset.UtcNow;
        public DateOnly FechaInicioAutorizada { get; set; }
        public DateOnly FechaFinAutorizada { get; set; }
        public bool Activo { get; set; } = true;

        public Guid? IdCreo { get; set; }
        public DateTimeOffset FechaCreo { get; set; } = DateTimeOffset.UtcNow;
        public Guid? IdEdito { get; set; }
        public DateTimeOffset? FechaEdito { get; set; }
    }
}
