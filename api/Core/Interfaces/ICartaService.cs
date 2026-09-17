using CartaNoAdeudoApi.Core.DTOs.Requests.Cartas;
using CartaNoAdeudoApi.Core.DTOs.Responses.Cartas;

namespace CartaNoAdeudoApi.Core.Interfaces
{
    /// <summary>
    /// Alta y consulta de solicitudes de carta de no adeudo (entidad <c>carta.Datos</c>).
    ///
    /// <see cref="SolicitarCartaAsync"/> registra la solicitud, asigna folio y firma de
    /// inmediato contra <see cref="IFirmaContraloriaService"/> (wsLicAlcoholes/FEA). Si la
    /// firma falla queda una <c>Tareas</c> en Error, reintentable con
    /// <see cref="ReprocesarSolicitudesAsync"/>. La generación del PDF todavía no está
    /// implementada (ver TODO en <c>ServiceExtensions.AddPdfGeneration</c>), así que
    /// <see cref="ObtenerReportePdfAsync"/> solo funciona una vez que exista un
    /// <c>Archivos</c> para la solicitud.
    /// </summary>
    public interface ICartaService
    {
        Task<CartaAceptadaResponse> SolicitarCartaAsync(GenerarCartaRequest request, CancellationToken ct = default);
        Task<ValidarCartaResponse> ValidarCartaAsync(ValidarCartaRequest request, CancellationToken ct = default);
        Task<byte[]> ObtenerReportePdfAsync(ValidarCartaRequest request, CancellationToken ct = default);
        Task<ReprocesoResultadoResponse> ReprocesarSolicitudesAsync(ReprocesarSolicitudesRequest request, CancellationToken ct = default);
    }
}
