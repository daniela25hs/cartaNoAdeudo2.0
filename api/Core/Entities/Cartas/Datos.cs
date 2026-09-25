using CartaNoAdeudoApi.Core.Entities.Bitacora;

namespace CartaNoAdeudoApi.Core.Entities.Cartas
{
    public class Datos : BaseEntity
    {
        public int IdTipoCarta { get; set; }

        public required string Nombre { get; set; } 
        public required string Rfc { get; set; } 
        public required string RO { get; set; }

        public required string Email { get; set; }
        public DateOnly InicioVigencia { get; set; }

        public DateOnly Vencimiento { get; set; }
        public bool Vencida { get; set; }
        public string? Alcoholes { get; set; }

        public string? Folio { get; set; }
        public DateTimeOffset? FechaFirmado { get; set; }
        public DateTimeOffset FechaHora { get; set; } = DateTimeOffset.UtcNow;
        public EstatusSolicitud Estatus { get; set; } = EstatusSolicitud.SolicitudFirma;

        /// <summary>Cuántas veces se ha intentado firmar (ver <c>TareaFirmaOptions.MaxIntentos</c>
        /// y <c>TareaFirmaWorker</c>). Al llegar al máximo, <see cref="Estatus"/> pasa a
        /// <c>ErrorDefinitivo</c> y deja de reintentarse solo.</summary>
        public int Intentos { get; set; }
        public TiposCartas? TipoCarta { get; set; }
        public Archivos? Archivo { get; set; }
        public ICollection<Tareas> Tareas { get; set; } = new List<Tareas>();
        public ICollection<Firmas> Firmas { get; set; } = new List<Firmas>();
        public ICollection<Carta> Bitacora { get; set; } = new List<Carta>();
    }
}
