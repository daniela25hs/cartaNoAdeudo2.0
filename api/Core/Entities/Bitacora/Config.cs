namespace CartaNoAdeudoApi.Core.Entities.Bitacora
{
    /// <summary>
    /// Bitácora de acciones administrativas de configuración (tabla
    /// <c>bit.Config</c>): alta / modificación / activación de certificados
    /// (RF-001 RN-006) y layouts (RF-002 RN-006). Registra fecha, hora y
    /// usuario responsable.
    /// </summary>
    public class Config : BaseEntity
    {
        public required string Accion { get; set; }
        public string? Descripcion { get; set; }
        public DateTimeOffset Fecha { get; set; } = DateTimeOffset.UtcNow;

        // Id del usuario responsable (proviene de SIGA; no hay tabla local de usuarios).
        public Guid? UsuarioId { get; set; }
    }
}
