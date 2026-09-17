using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CartaNoAdeudoApi.Core.DTOs.Requests.Cartas;
using CartaNoAdeudoApi.Core.Interfaces;

namespace CartaNoAdeudoApi.API.Controllers
{
    /// <summary>
    /// Alta y consulta de solicitudes de carta de no adeudo. La firma electrónica y la
    /// generación del PDF todavía no están implementadas (ver <see cref="ICartaService"/>):
    /// <see cref="Solicitar"/> solo registra la solicitud y la encola.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartasController(ICartaService cartaService) : ControllerBase
    {
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

        [HttpPost("reprocesar")]
        public async Task<IActionResult> Reprocesar([FromBody] ReprocesarSolicitudesRequest request, CancellationToken ct) =>
            Ok(await cartaService.ReprocesarSolicitudesAsync(request, ct));
    }
}
