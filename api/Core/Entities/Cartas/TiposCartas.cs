namespace CartaNoAdeudoApi.Core.Entities.Cartas
{
    public class TiposCartas
    {
        public int Id { get; set; }
        public required string Descripcion { get; set; }
        public required string Clave { get; set; }
        public bool Activo { get; set; } = true;
        public ICollection<Datos> Datos { get; set; } = new List<Datos>();
    }
}
