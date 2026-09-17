using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CartaNoAdeudoApi.Core.DTOs.Requests.Layouts;
using CartaNoAdeudoApi.Core.Exceptions;
using CartaNoAdeudoApi.Core.Interfaces;

namespace CartaNoAdeudoApi.API.Controllers
{
    /// <summary>
    /// Administración de layouts .docx (RF-002): alta, edición y activación de la
    /// plantilla que usa la firma electrónica para generar la carta. Solo la capa API
    /// conoce <see cref="IFormFile"/>; el servicio trabaja con <see cref="Stream"/>.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LayoutsController(ILayoutService layoutService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Listar(CancellationToken ct) =>
            Ok(await layoutService.ListarAsync(ct));

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> ObtenerPorId(Guid id, CancellationToken ct) =>
            Ok(await layoutService.ObtenerAsync(id, ct));

        [HttpPost("marcadores")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PrevisualizarMarcadores(IFormFile archivo, CancellationToken ct)
        {
            ValidarExtensionDocx(archivo);

            await using var stream = archivo.OpenReadStream();
            return Ok(await layoutService.PrevisualizarMarcadoresAsync(stream, ct));
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Registrar([FromForm] RegistrarLayoutRequest request, IFormFile archivo, CancellationToken ct)
        {
            ValidarExtensionDocx(archivo);

            await using var stream = archivo.OpenReadStream();
            var resultado = await layoutService.RegistrarAsync(request, stream, ct);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = resultado.Id }, resultado);
        }

        [HttpPut("{id:guid}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Actualizar(Guid id, [FromForm] ActualizarLayoutRequest request, IFormFile? archivo, CancellationToken ct)
        {
            if (archivo is not null)
                ValidarExtensionDocx(archivo);

            await using var stream = archivo?.OpenReadStream();
            return Ok(await layoutService.ActualizarAsync(id, request, stream, ct));
        }

        [HttpPatch("{id:guid}/toggle")]
        public async Task<IActionResult> ToggleActivo(Guid id, CancellationToken ct)
        {
            await layoutService.ToggleActivoAsync(id, ct);
            return NoContent();
        }

        private static void ValidarExtensionDocx(IFormFile archivo)
        {
            if (!Path.GetExtension(archivo.FileName).Equals(".docx", StringComparison.OrdinalIgnoreCase))
                throw new ConflictException("El layout debe ser un archivo .docx.");
        }
    }
}
