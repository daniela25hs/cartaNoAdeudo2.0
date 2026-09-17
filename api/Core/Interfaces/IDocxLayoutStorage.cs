namespace CartaNoAdeudoApi.Core.Interfaces
{
    /// <summary>
    /// Guarda/lee/borra los .docx de <c>Layout</c> en disco. <c>Layout.Archivo</c> guarda
    /// solo el nombre que regresa <see cref="GuardarAsync"/>, no la ruta completa.
    /// </summary>
    public interface IDocxLayoutStorage
    {
        Task<string> GuardarAsync(Stream contenido, CancellationToken ct = default);
        Task<Stream> AbrirAsync(string archivo, CancellationToken ct = default);
        void Eliminar(string archivo);
    }
}
