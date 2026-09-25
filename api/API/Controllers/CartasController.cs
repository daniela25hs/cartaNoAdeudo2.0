using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CartaNoAdeudoApi.Core.DTOs.Requests.Cartas;
using CartaNoAdeudoApi.Core.Entities.Cartas;
using CartaNoAdeudoApi.Core.Interfaces;

namespace CartaNoAdeudoApi.API.Controllers
{
    /// <summary>
    /// Alta y consulta de solicitudes de carta de no adeudo. <see cref="Solicitar"/> registra la
    /// solicitud y hace el primer intento de firma de inmediato; si falla, queda reintentándose
    /// solo en background hasta agotar sus intentos (ver <see cref="ICartaService"/>). La
    /// generación del PDF todavía no está implementada.
    /// </summary>
    /// 
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize] // TODO: reactivar junto con SIGA (última fase: usuarios). Ver docs/INTEGRACION_SIGA.md
    public class CartasController(ICartaService cartaService) : ControllerBase
    {
        [HttpGet("tipos")]
        public async Task<IActionResult> ListarTipos(CancellationToken ct) =>
            Ok(await cartaService.ListarTiposCartaAsync(ct));

        /// <summary>Pantalla de Tareas: lista las solicitudes (más recientes primero), con su
        /// último error de firma si lo tuvo. <paramref name="estatus"/> es opcional, por ejemplo
        /// <c>?estatus=ErrorDefinitivo</c> para ver solo las reintentables.</summary>
        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] EstatusSolicitud? estatus, CancellationToken ct) =>
            Ok(await cartaService.ListarSolicitudesAsync(estatus, ct));

        [HttpPost]
        public async Task<IActionResult> Solicitar([FromBody] GenerarCartaRequest request, CancellationToken ct)
        {
            var resultado = await cartaService.SolicitarCartaAsync(request, ct);
            return Ok(resultado);
        }

        [HttpPost("validar")]
        public async Task<IActionResult> Validar([FromBody] ValidarCartaRequest request, CancellationToken ct) =>
            Ok(await cartaService.ValidarCartaAsync(request, ct));

        [Produces("application/pdf")]
        [HttpPost("reporte")]
        public async Task<IActionResult> Reporte([FromBody] ValidarCartaRequest request, CancellationToken ct)
        {
            var contenido = await cartaService.ObtenerReportePdfAsync(request, ct);
            return File(contenido, "application/pdf", $"carta-{request.Rfc}-{request.Folio}.pdf");
        }

        /// <summary>Reprocesa una sola solicitud puntual que quedó en ErrorDefinitivo (solo esa,
        /// no admite más de un id). Para reprocesar todas a la vez usa <see cref="Reprocesar"/>.</summary>
        [HttpPost("{id:guid}/reprocesar")]
        public async Task<IActionResult> ReprocesarUna(Guid id, CancellationToken ct)
        {
            var resultado = await cartaService.ReprocesarSolicitudesAsync(new ReprocesarSolicitudesRequest([id]), ct);
            var item = resultado.Resultados[0];
            return item.Aceptado ? Ok(item) : Conflict(item);
        }

        /// <summary>Reprocesa TODAS las solicitudes que estén en ErrorDefinitivo ahora mismo (sin
        /// indicar ids). Para una sola en particular usa <see cref="ReprocesarUna"/>.</summary>
        [HttpPost("reprocesar")]
        public async Task<IActionResult> Reprocesar(CancellationToken ct) =>
            Ok(await cartaService.ReprocesarTodosAsync(ct));

        /// <summary>Reencola a mano, sin reiniciar la app, las solicitudes que se quedaron en
        /// SolicitudFirma (intento en curso cuando se cayó el proceso, o esperando su siguiente
        /// reintento). Lo mismo que hace <c>TareaFirmaRecovery</c> solo al arrancar. No toca las
        /// que ya llegaron a ErrorDefinitivo; para esas usa <see cref="Reprocesar"/> o
        /// <see cref="ReprocesarUna"/>. <paramref name="resetRetries"/> además reinicia el
        /// contador de intentos de cada una reencolada, para que vuelva a tener sus
        /// <c>TareaFirmaOptions.MaxIntentos</c> completos.</summary>
        [HttpPost("recovery")]
        public async Task<IActionResult> Recovery(
            [FromServices] ITareaFirmaRecovery recovery,
            [FromQuery] bool resetRetries,
            CancellationToken ct)
        {
            var encoladas = await recovery.RecuperarAsync(resetRetries, ct);
            return Ok(new { Encoladas = encoladas, ResetRetries = resetRetries });
        }
    }
}
