namespace CartaNoAdeudoApi.Core.Entities.Catalogos
{
    public class Firmante : ActivableEntity
    {
        public required string Nombre { get; set; }
        public string? Puesto { get; set; }
        public Genero Genero { get; set; } = Genero.NoEspecificado;
        public string? Certificado { get; set; }
        public string? Pfx { get; set; }
        public string? Contrasena { get; set; }
        public string? EstatusCertificado { get; set; }
        public DateOnly FechaInicioCertificado { get; set; }
        public DateOnly FechaFinCertificado { get; set; }
        public DateOnly FechaInicioAutorizada { get; set; }
        public DateOnly FechaFinAutorizada { get; set; }
    }
}
