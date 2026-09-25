namespace CartaNoAdeudoApi.Core.Entities.Bitacora
{
    /// <summary>
    /// Una fila por cada cambio de estado de una <see cref="Cartas.Tareas"/> (intento de firma):
    /// a diferencia de <c>Tareas</c>, que guarda solo el estado final de cada intento, esta
    /// tabla es el histórico append-only de esos cambios.
    /// No hereda de <see cref="Entities.BaseEntity"/>: su PK es <c>int</c> autoincremental,
    /// así que se resuelve vía <see cref="Interfaces.Repositories.ITareaHistorialRepository"/>
    /// en vez de <c>IUnitOfWork.Repository&lt;T&gt;()</c>.
    /// </summary>
    public class TareaHistorial
    {
        public int Id { get; set; }
        public Guid IdTarea { get; set; }

        /// <summary>Nombre del estado (ver <see cref="Catalogos.EstadoTarea"/>), no el id numérico.</summary>
        public required string Estado { get; set; }
        public DateTimeOffset Fecha { get; set; } = DateTimeOffset.UtcNow;
        public string? Mensaje { get; set; }

        // Navegacion
        public Cartas.Tareas? Tarea { get; set; }
    }
}
