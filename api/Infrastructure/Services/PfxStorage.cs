using Microsoft.Extensions.Options;
using CartaNoAdeudoApi.Core.Interfaces;
using CartaNoAdeudoApi.Core.Options;

namespace CartaNoAdeudoApi.Infrastructure.Services
{
    public class PfxStorage(IOptions<FirmanteStorageOptions> options) : IPfxStorage
    {
        private readonly string _basePath = options.Value.PfxPath;

        public async Task<string> GuardarAsync(byte[] contenido, CancellationToken ct = default)
        {
            Directory.CreateDirectory(_basePath);
            var nombreArchivo = $"{Guid.NewGuid()}.pfx";

            await File.WriteAllBytesAsync(Path.Combine(_basePath, nombreArchivo), contenido, ct);
            return nombreArchivo;
        }

        public Task<byte[]> LeerAsync(string archivo, CancellationToken ct = default) =>
            File.ReadAllBytesAsync(Path.Combine(_basePath, archivo), ct);

        public void Eliminar(string archivo)
        {
            var ruta = Path.Combine(_basePath, archivo);
            if (File.Exists(ruta))
                File.Delete(ruta);
        }
    }
}
