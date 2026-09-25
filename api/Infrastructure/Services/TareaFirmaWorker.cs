using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CartaNoAdeudoApi.Core.Interfaces;
using CartaNoAdeudoApi.Core.Options;
using CartaNoAdeudoApi.Core.Utils;

namespace CartaNoAdeudoApi.Infrastructure.Services
{
    /// <summary>
    /// Consume <see cref="ITareaFirmaQueue"/> y reintenta las firmas fallidas en background,
    /// hasta <see cref="TareaFirmaOptions.MaxIntentos"/> (el conteo y la decisión de si hace
    /// falta reintentar viven en <c>ICartaService.ProcesarIntentoFirmaAsync</c>, no aquí).
    /// El primer intento de cada solicitud es síncrono, en <c>CartaService.SolicitarCartaAsync</c>;
    /// este worker solo entra si ese intento (o uno anterior de este mismo worker) falló.
    /// </summary>
    public class TareaFirmaWorker(
        ITareaFirmaQueue queue,
        IServiceScopeFactory scopeFactory,
        IOptions<TareaFirmaOptions> options,
        ILogger<TareaFirmaWorker> logger) : BackgroundService
    {
        private readonly TareaFirmaOptions _options = options.Value;
        private readonly SemaphoreSlim _semaphore = new(options.Value.MaxConcurrencia);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var item in queue.LeerTodoAsync(stoppingToken))
            {
                // Fire-and-forget: no bloquear la lectura del canal mientras un item espera
                // turno/reintento. stoppingToken se respeta dentro de ProcesarAsync.
                _ = ProcesarAsync(item, stoppingToken);
            }
        }

        private async Task ProcesarAsync(TareaFirmaItem item, CancellationToken stoppingToken)
        {
            await _semaphore.WaitAsync(stoppingToken);

            ResultadoIntentoFirma? resultado;
            try
            {
                TareaFirmaMetrics.Iniciar();

                using var scope = scopeFactory.CreateScope();
                var cartaService = scope.ServiceProvider.GetRequiredService<ICartaService>();

                resultado = await cartaService.ProcesarIntentoFirmaAsync(item.IdDato, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return; // apagado normal del host
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado reintentando la firma de la solicitud {IdDato}.", item.IdDato);
                return;
            }
            finally
            {
                TareaFirmaMetrics.Terminar();
                _semaphore.Release();
            }

            if (resultado is not { Exitoso: false, RequiereReintento: true })
                return;

            try
            {
                logger.LogInformation(
                    "Solicitud {IdDato}: se reintentará la firma en {Segundos}s.",
                    item.IdDato, _options.EsperaReintentoSegundos);

                await Task.Delay(TimeSpan.FromSeconds(_options.EsperaReintentoSegundos), stoppingToken);
                await queue.EncolarAsync(item, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // apagado normal del host mientras esperaba para reencolar
            }
        }
    }
}
