namespace CartaNoAdeudoApi.Core.Entities.Catalogos
{
    /// <summary>
    /// No hereda de <see cref="Entities.BaseEntity"/>: su PK es <c>int</c> autoincremental
    /// (1, 2, 3...), igual que <c>TiposCartas</c>/<c>Estado</c>, así que se resuelve vía
    /// <see cref="Interfaces.Repositories.IEstadoTareaRepository"/> en vez de
    /// <c>IUnitOfWork.Repository&lt;T&gt;()</c>.
    /// </summary>
    public class EstadoTarea
    {
        public const string Pendiente = "Pendiente";
        public const string EnProceso = "EnProceso";
        public const string Completada = "Completada";
        public const string Error = "Error";

        public int Id { get; set; }
        public required string Descripcion { get; set; }
    }
}
