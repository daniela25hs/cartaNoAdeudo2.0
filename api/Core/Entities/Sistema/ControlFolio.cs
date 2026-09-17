namespace CartaNoAdeudoApi.Core.Entities.Sistema
{
    /// <summary>
    /// Foliador genérico por año (tabla <c>sist.control_folios</c>): lleva el último
    /// folio emitido para un año determinado, independiente del tipo de carta.
    /// </summary>
    public class ControlFolio : BaseEntity
    {
        public required int Anio { get; set; }
        public int UltimoFolio { get; set; }
    }
}
