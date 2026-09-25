namespace CartaNoAdeudoApi.Core.Interfaces
{
    /// <summary>
    /// Guarda/lee/borra los .pfx de <c>Firmante</c> en disco. <c>Firmante.Pfx</c> guarda solo
    /// el nombre que regresa <see cref="GuardarAsync"/>, no la ruta completa.
    /// </summary>
    public interface IPfxStorage
    {
        Task<string> GuardarAsync(byte[] contenido, CancellationToken ct = default);
        Task<byte[]> LeerAsync(string archivo, CancellationToken ct = default);
        void Eliminar(string archivo);
    }
}
