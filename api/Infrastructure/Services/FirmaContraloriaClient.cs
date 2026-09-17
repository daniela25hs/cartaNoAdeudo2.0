using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CartaNoAdeudoApi.Core.Interfaces;
using CartaNoAdeudoApi.Core.Options;

namespace CartaNoAdeudoApi.Infrastructure.Services
{
    /// <summary>
    /// Llama al servicio externo "wsLicAlcoholes" (el que resguarda el PFX y firma contra
    /// el web service FEA de la Contraloría). Esa API responde siempre 200 aun cuando la
    /// firma remota falló (<c>Firma == 0</c> en su respuesta) — solo <c>Firma > 0</c> con
    /// un <c>Mensaje</c> no vacío cuenta como éxito real.
    /// </summary>
    public class FirmaContraloriaClient(
        HttpClient http, IOptions<FirmaContraloriaOptions> options, ILogger<FirmaContraloriaClient> logger)
        : IFirmaContraloriaService
    {
        private readonly FirmaContraloriaOptions _options = options.Value;

        private record LicAlcoholesRequest(string Rfc, string Nombre, string Ro, string TipoCarta, string InicioVigencia, string? LicAlcoholes);
        private record LicAlcoholesResponse(int Codigo, string? Mensaje, int Firma);

        public async Task<FirmaElectronicaResultado> FirmarAsync(
            string rfc, string nombre, string ro, string tipoCartaClave, DateOnly inicioVigencia, string? licAlcoholes,
            CancellationToken ct = default)
        {
            var payload = new LicAlcoholesRequest(
                rfc, nombre, ro, tipoCartaClave, inicioVigencia.ToString("yyyy-MM-dd"), licAlcoholes);

            // Reintenta solo fallas de transporte (red/timeout/5xx), nunca una firma remota
            // rechazada (esa es una respuesta de negocio válida, no un error transitorio).
            // Con la config por default (3 intentos, 60s de espera) esto puede bloquear la
            // llamada hasta ~2 minutos en el peor caso.
            for (var intento = 1; intento <= _options.MaxReintentos; intento++)
            {
                try
                {
                    using var response = await http.PostAsJsonAsync(string.Empty, payload, ct);

                    if (response.IsSuccessStatusCode)
                    {
                        var body = await response.Content.ReadFromJsonAsync<LicAlcoholesResponse>(cancellationToken: ct);

                        if (body is { Firma: > 0 } && !string.IsNullOrWhiteSpace(body.Mensaje))
                            return new FirmaElectronicaResultado(true, body.Mensaje, $"Firma remota #{body.Firma}", null);

                        logger.LogWarning(
                            "FirmaContraloria respondió 200 sin firma válida (Firma={Firma}).", body?.Firma);
                        return new FirmaElectronicaResultado(
                            false, null, null, "El servicio de firma electrónica no pudo firmar la carta.");
                    }

                    logger.LogWarning(
                        "FirmaContraloria respondió {StatusCode} en el intento {Intento}/{MaxIntentos}.",
                        response.StatusCode, intento, _options.MaxReintentos);
                }
                catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
                {
                    logger.LogWarning(ex,
                        "Fallo de red hacia FirmaContraloria en el intento {Intento}/{MaxIntentos}.",
                        intento, _options.MaxReintentos);
                }

                if (intento < _options.MaxReintentos)
                    await Task.Delay(TimeSpan.FromSeconds(_options.IntervaloReintentoSegundos), ct);
            }

            return new FirmaElectronicaResultado(
                false, null, null, "No se pudo contactar al servicio de firma electrónica tras varios intentos.");
        }
    }
}
