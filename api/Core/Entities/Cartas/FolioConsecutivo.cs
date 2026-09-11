namespace CartaNoAdeudoApi.Core.Entities.Cartas
{
    public class FolioConsecutivo : AuditableEntity
    {
        public required string TipoCarta { get; set; }
        public string? Descripcion { get; set; }
        public int UltimoFolio { get; set; }
    }
}
