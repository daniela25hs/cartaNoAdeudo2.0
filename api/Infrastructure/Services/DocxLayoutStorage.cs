using Microsoft.Extensions.Options;
using CartaNoAdeudoApi.Core.Interfaces;
using CartaNoAdeudoApi.Core.Options;

namespace CartaNoAdeudoApi.Infrastructure.Services
{
    public class DocxLayoutStorage(IOptions<LayoutStorageOptions> options) : IDocxLayoutStorage
    {
        private readonly string _basePath = options.Value.BasePath;

        public async Task<string> GuardarAsync(Stream contenido, CancellationToken ct = default)
        {
            Directory.CreateDirectory(_basePath);
            var nombreArchivo = $"{Guid.NewGuid()}.docx";

            await using var destino = File.Create(Path.Combine(_basePath, nombreArchivo));
            await contenido.CopyToAsync(destino, ct);

            return nombreArchivo;
        }

        public Task<Stream> AbrirAsync(string archivo, CancellationToken ct = default) =>
            Task.FromResult<Stream>(File.OpenRead(Path.Combine(_basePath, archivo)));

        public void Eliminar(string archivo)
        {
            var ruta = Path.Combine(_basePath, archivo);
            if (File.Exists(ruta))
                File.Delete(ruta);
        }
    }
}
