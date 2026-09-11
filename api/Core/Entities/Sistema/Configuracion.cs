namespace CartaNoAdeudoApi.Core.Entities.Sistema
{
    /// <summary>
    /// Parámetro de configuración del sistema (tabla <c>sist.Configuracion</c>):
    /// reintentos de firma, intervalo entre intentos, correos de administradores
    /// para notificaciones, etc.
    /// </summary>
    public class Configuracion : BaseEntity
    {
        public required string Clave { get; set; }
        public required string Valor { get; set; }
        public string? Descripcion { get; set; }
        public DateTimeOffset FechaModificacion { get; set; } = DateTimeOffset.UtcNow;
    }
}
