using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CartaNoAdeudoApi.Core.DTOs.Requests.Auth;
using CartaNoAdeudoApi.Infrastructure.Auth;

namespace CartaNoAdeudoApi.API.Controllers
{
    /// <summary>
    /// Proxy de login / refresh hacia SIGA. Esta API nunca emite tokens propios ni
    /// valida contraseñas: solo reenvía a SIGA e inyecta el AppKey desde config.
    ///
    /// Requiere <c>AddSigaAuthProxy</c> activo en Program.cs. Mientras SIGA esté
    /// comentado estos endpoints responden 500 (SigaAuthClient no está registrado).
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthController(SigaAuthClient sigaAuthClient, ILogger<AuthController> logger) : ControllerBase
    {
        [HttpPost("login")]
        public Task<IActionResult> Login([FromBody] LoginProxyRequest request, CancellationToken ct) =>
            ForwardAsync(() => sigaAuthClient.LoginAsync(request.Email, request.Password, ct), "login");

        [HttpPost("refresh")]
        public Task<IActionResult> Refresh([FromBody] RefreshProxyRequest request, CancellationToken ct) =>
            ForwardAsync(() => sigaAuthClient.RefreshAsync(request.RefreshToken, ct), "refresh");

        private async Task<IActionResult> ForwardAsync(Func<Task<SigaProxyResult>> call, string operacion)
        {
            try
            {  
                var result = await call();
                return new ContentResult
                {
                    StatusCode = result.StatusCode,
                    Content = result.Body,
                    ContentType = result.ContentType ?? "application/json"
                };
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                logger.LogError(ex, "Carta no adeudo no disponible durante {Operacion}.", operacion);
                return Problem(
                    statusCode: StatusCodes.Status503ServiceUnavailable,
                    title: "Carta no adeudo no disponible",
                    detail: "No se pudo contactar al proveedor de identidad. Intenta de nuevo en unos segundos.");
            }
        }
    }
}
