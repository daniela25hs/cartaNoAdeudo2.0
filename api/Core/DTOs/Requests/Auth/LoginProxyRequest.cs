namespace CartaNoAdeudoApi.Core.DTOs.Requests.Auth
{
    // Lo que manda el frontend. Sin "aplicacion": el AppKey lo inyecta el backend
    // desde SigaOptions, nunca viaja desde el cliente.
    public record LoginProxyRequest(string Email, string Password);

    public record RefreshProxyRequest(string RefreshToken);
}
