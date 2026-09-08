using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CartaNoAdeudoApi.Core.Interfaces;

namespace CartaNoAdeudoApi.API.Controllers
{
    /// <summary>
    /// Ejemplo de endpoint protegido: devuelve los claims del JWT de SIGA del
    /// usuario actual. Funciona una vez que <c>AddSigaAuth</c> esté activo en
    /// Program.cs; con SIGA comentado, <c>[Authorize]</c> no tiene esquema y la
    /// ruta falla — es el comportamiento esperado hasta activar la integración.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EjemploProtegidoController(ICurrentUserService currentUser) : ControllerBase
    {
        [HttpGet("yo")]
        public IActionResult Yo() => Ok(new
        {
            usuarioId = currentUser.UserId,
            email = currentUser.Email,
            grupoId = currentUser.GrupoId,
            appId = currentUser.AppId,
            rolesSiga = currentUser.RolesSiga
        });
    }
}
