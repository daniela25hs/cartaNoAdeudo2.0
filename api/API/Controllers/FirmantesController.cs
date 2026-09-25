using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CartaNoAdeudoApi.Core.DTOs.Requests.Firmantes;
using CartaNoAdeudoApi.Core.Exceptions;
using CartaNoAdeudoApi.Core.Interfaces;

namespace CartaNoAdeudoApi.API.Controllers
{
    /// <summary>
    /// Administración de firmantes: alta con su PFX, edición y activación. Solo la capa API
    /// conoce el servicio de firmantes. Ninguna
    /// respuesta expone el PFX ni la contraseña.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize] // TODO: reactivar junto con SIGA (última fase: usuarios). Ver docs/INTEGRACION_SIGA.md
    public class FirmantesController(IFirmanteService firmanteService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Listar(CancellationToken ct) =>
            Ok(await firmanteService.ListarAsync(ct));

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> ObtenerPorId(Guid id, CancellationToken ct) =>
            Ok(await firmanteService.ObtenerAsync(id, ct));

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Registrar([FromForm] RegistrarFirmanteRequest request, IFormFile archivo, CancellationToken ct)
        {
            ValidarExtensionPfx(archivo);

            await using var stream = archivo.OpenReadStream();
            var resultado = await firmanteService.RegistrarAsync(request, stream, ct);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = resultado.Id }, resultado);
        }

        [HttpPut("{id:guid}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Actualizar(Guid id, [FromForm] ActualizarFirmanteRequest request, IFormFile? archivo, CancellationToken ct)
        {
            if (archivo is not null)
                ValidarExtensionPfx(archivo);

            await using var stream = archivo?.OpenReadStream();
            return Ok(await firmanteService.ActualizarAsync(id, request, stream, ct));
        }

        [HttpPatch("{id:guid}/toggle")]
        public async Task<IActionResult> ToggleActivo(Guid id, CancellationToken ct)
        {
            await firmanteService.ToggleActivoAsync(id, ct);
            return NoContent();
        }

        private static void ValidarExtensionPfx(IFormFile archivo)
        {
            var extension = Path.GetExtension(archivo.FileName);
            if (!extension.Equals(".pfx", StringComparison.OrdinalIgnoreCase) &&
                !extension.Equals(".p12", StringComparison.OrdinalIgnoreCase))
                throw new ConflictException("El certificado debe ser un archivo .pfx o .p12.");
        }
    }
}
