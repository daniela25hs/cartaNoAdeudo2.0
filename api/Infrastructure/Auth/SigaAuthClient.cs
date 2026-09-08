using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using CartaNoAdeudoApi.Core.Options;

namespace CartaNoAdeudoApi.Infrastructure.Auth
{
    // Resultado crudo de SIGA: status code + body tal cual, sin deserializar a un DTO
    // propio. El proxy no reinterpreta ni envuelve la respuesta de SIGA — reenviar
    // bytes evita mantener sincronizados los shapes de éxito y de error.
    public record SigaProxyResult(int StatusCode, string? ContentType, string Body);

    /// <summary>
    /// Cliente HTTP hacia SIGA para login / refresh (esta API nunca emite tokens
    /// propios). Se registra en <c>AddSigaAuthProxy</c>.
    /// </summary>
    public class SigaAuthClient(HttpClient http, IOptions<SigaOptions> options)
    {
        private readonly SigaOptions _options = options.Value;

        public Task<SigaProxyResult> LoginAsync(string email, string password, CancellationToken ct) =>
            PostAsync("/auth/login", new { email, password, aplicacion = _options.AppKey }, ct);

        public Task<SigaProxyResult> RefreshAsync(string refreshToken, CancellationToken ct) =>
            PostAsync("/auth/refresh", new { refreshToken }, ct);

        private async Task<SigaProxyResult> PostAsync(string path, object payload, CancellationToken ct)
        {
            using var response = await http.PostAsJsonAsync(path, payload, ct);
            var body = await response.Content.ReadAsStringAsync(ct);
            var contentType = response.Content.Headers.ContentType?.ToString();
            return new SigaProxyResult((int)response.StatusCode, contentType, body);
        }
    }
}
