using CartaNoAdeudoApi.Core.DTOs.Requests.Cartas;
using CartaNoAdeudoApi.Core.DTOs.Responses.Cartas;
using CartaNoAdeudoApi.Core.DTOs.Responses.TiposCartas;
using CartaNoAdeudoApi.Core.Entities.Cartas;

namespace CartaNoAdeudoApi.Core.Interfaces
{
    /// <summary>Resultado de un intento de firma (síncrono o desde <c>TareaFirmaWorker</c>).
    /// <see cref="RequiereReintento"/> le dice a quien llamó si debe encolarlo de nuevo.</summary>
    public record ResultadoIntentoFirma(bool Exitoso, bool RequiereReintento);

    /// <summary>
    /// Alta y consulta de solicitudes de carta de no adeudo (entidad <c>carta.Datos</c>).
    ///
    /// SolicitarCartaregistra la solicitud, asigna folio y hace el primer
    /// intento de firma de inmediato con FirmaCartaService (PFX del firmante
    /// activo). Si ese intento falla, la solicitud se encola en <c>ITareaFirmaQueue</c> y
    /// <c>TareaFirmaWorker</c> la reintenta solo en background hasta agotar
    /// <c>TareaFirmaOptions.MaxIntentos</c> (cada intento deja una <c>Tareas</c> con su
    /// resultado); al agotarlos, <c>Datos.Estatus</c> pasa a <c>EstatusSolicitud.ErrorDefinitivo</c>
    /// y ya no se reintenta solo — ahí es donde entra ReprocesarSolicitudesAsync,
    /// para forzar un reinicio manual del contador. La generación del PDF todavía no está
    /// implementada (ver TODO en <c>ServiceExtensions.AddPdfGeneration</c>), así que
    /// <see cref="ObtenerReportePdfAsync"/> solo funciona una vez que exista un
    /// <c>Archivos</c> para la solicitud.
    /// </summary>
    public interface ICartaService
    {
        /// <summary>Catálogo de tipos de carta activos (<c>cat.TiposCartas</c>), para poblar el
        /// selector de <see cref="GenerarCartaRequest.TipoCarta"/> en el front.</summary>
        Task<IReadOnlyList<TipoCartaResponse>> ListarTiposCartaAsync(CancellationToken ct = default);

        /// <summary>Lista las solicitudes (más recientes primero) para la pantalla de Tareas,
        /// con el último error de firma si lo tuvo. <paramref name="estatus"/> filtra opcionalmente
        /// (por ejemplo, solo <c>ErrorDefinitivo</c> para ver qué se puede reintentar).</summary>
        Task<IReadOnlyList<SolicitudListItemResponse>> ListarSolicitudesAsync(EstatusSolicitud? estatus, CancellationToken ct = default);

        Task<CartaAceptadaResponse> SolicitarCartaAsync(GenerarCartaRequest request, CancellationToken ct = default);
        Task<ValidarCartaResponse> ValidarCartaAsync(ValidarCartaRequest request, CancellationToken ct = default);
        Task<byte[]> ObtenerReportePdfAsync(ValidarCartaRequest request, CancellationToken ct = default);

        /// <summary>Reinicia el contador de intentos y reencola una o varias solicitudes puntuales
        /// que llegaron a <c>EstatusSolicitud.ErrorDefinitivo</c> (agotaron sus reintentos
        /// automáticos). No hace nada por solicitudes que aún están dentro de sus reintentos
        /// automáticos: esas ya las está manejando <c>TareaFirmaWorker</c> solo.</summary>
        Task<ReprocesoResultadoResponse> ReprocesarSolicitudesAsync(ReprocesarSolicitudesRequest request, CancellationToken ct = default);

        /// <summary>Igual que <see cref="ReprocesarSolicitudesAsync"/> pero sobre TODAS las
        /// solicitudes que estén en <c>EstatusSolicitud.ErrorDefinitivo</c> ahora mismo, sin tener
        /// que indicar sus ids uno por uno.</summary>
        Task<ReprocesoResultadoResponse> ReprocesarTodosAsync(CancellationToken ct = default);

        /// <summary>Ejecuta un intento de firma para una solicitud ya existente. Lo usa
        /// <c>TareaFirmaWorker</c> para los reintentos en background; idempotente si la
        /// solicitud ya quedó Firmada por otro camino.</summary>
        Task<ResultadoIntentoFirma> ProcesarIntentoFirmaAsync(Guid idDato, CancellationToken ct = default);
    }
}
