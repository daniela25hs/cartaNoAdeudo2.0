using System.Threading.Channels;
using CartaNoAdeudoApi.Core.Interfaces;

namespace CartaNoAdeudoApi.Infrastructure.Services
{
    /// <summary>Implementación de <see cref="ITareaFirmaQueue"/> con <see cref="Channel{T}"/>.
    /// Debe registrarse como singleton: todo lector/escritor comparte el mismo canal.</summary>
    public class TareaFirmaQueue : ITareaFirmaQueue
    {
        private readonly Channel<TareaFirmaItem> _channel = Channel.CreateUnbounded<TareaFirmaItem>();

        public async ValueTask EncolarAsync(TareaFirmaItem item, CancellationToken ct = default) =>
            await _channel.Writer.WriteAsync(item, ct);

        public IAsyncEnumerable<TareaFirmaItem> LeerTodoAsync(CancellationToken ct = default) =>
            _channel.Reader.ReadAllAsync(ct);
    }
}
