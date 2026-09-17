using CartaNoAdeudoApi.Core.Entities.Cartas;

namespace CartaNoAdeudoApi.Core.Interfaces.Repositories
{
    /// <summary>
    /// <c>TiposCartas</c> no hereda de <see cref="Entities.BaseEntity"/> (su PK calca el
    /// diagrama ER como <c>int</c>), así que no puede resolverse vía
    /// <c>IUnitOfWork.Repository&lt;T&gt;()</c>. Repo dedicado solo para eso.
    /// </summary>
    public interface ITipoCartaRepository
    {
        Task<TiposCartas?> GetByClaveAsync(string clave, CancellationToken ct = default);
    }
}
