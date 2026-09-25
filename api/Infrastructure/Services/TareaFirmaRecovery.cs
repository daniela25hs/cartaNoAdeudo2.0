using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CartaNoAdeudoApi.Core.Entities.Cartas;
using CartaNoAdeudoApi.Core.Interfaces;
using CartaNoAdeudoApi.Core.Interfaces.Repositories;

namespace CartaNoAdeudoApi.Infrastructure.Services
{
    /// <summary>
    /// Repuebla <see cref="ITareaFirmaQueue"/> (vive solo en memoria del proceso, así que se
    /// vacía en cada reinicio/despliegue) con las solicitudes que se quedaron en
    /// <c>EstatusSolicitud.SolicitudFirma</c>: pueden ser solicitudes cuyo intento seguía en
    /// curso cuando la app se detuvo, o que estaban esperando su siguiente reintento en
    /// <c>TareaFirmaWorker</c>. Las que ya llegaron a <c>ErrorDefinitivo</c> NO se reencolan
    /// aquí a propósito; esas requieren <c>ICartaService.ReprocesarSolicitudesAsync</c>.
    ///
    /// Corre sola al arrancar la app (<see cref="StartAsync"/>, como <c>IHostedService</c>) y
    /// también se puede disparar a mano en caliente, sin reiniciar, vía
    /// <see cref="RecuperarAsync"/> (expuesto en <c>POST api/cartas/recovery</c>) — para eso se
    /// registra como singleton además de hosted service, ver <c>ServiceExtensions.AddTareaFirma</c>.
    /// </summary>
    public class TareaFirmaRecovery(
        IServiceScopeFactory scopeFactory,
        ITareaFirmaQueue queue,
        ILogger<TareaFirmaRecovery> logger) : ITareaFirmaRecovery, IHostedService
    {
        public async Task<int> RecuperarAsync(bool resetRetries = false, CancellationToken ct = default)
        {
            using var scope = scopeFactory.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var pendientes = await uow.Repository<Datos>().AsQueryable()
                .Include(d => d.TipoCarta)
                .Where(d => d.Estatus == EstatusSolicitud.SolicitudFirma)
                .ToListAsync(ct);

            logger.LogInformation(
                "Recuperación de firmas: {Cantidad} solicitud(es) pendiente(s) (resetRetries={ResetRetries}).",
                pendientes.Count, resetRetries);

            if (resetRetries)
            {
                foreach (var dato in pendientes)
                {
                    dato.Intentos = 0;
                    uow.Repository<Datos>().Update(dato);
                }
                await uow.SaveAsync(ct);
            }

            foreach (var dato in pendientes)
                await queue.EncolarAsync(new TareaFirmaItem(dato.Id, dato.TipoCarta!.Clave), ct);

            return pendientes.Count;
        }

        public Task StartAsync(CancellationToken cancellationToken) => RecuperarAsync(ct: cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
