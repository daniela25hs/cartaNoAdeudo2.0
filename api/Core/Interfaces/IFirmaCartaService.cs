using CartaNoAdeudoApi.Core.Entities.Cartas;

namespace CartaNoAdeudoApi.Core.Interfaces
{
    /// <summary>Resultado de firmar una carta; si falla, <see cref="MensajeError"/> dice por qué.</summary>
    public record ResultadoFirmaCarta(
        bool Exitoso,
        FirmaGenerada? Firma,
        string? CadenaOriginal,
        Guid? IdFirmante,
        string? NombreFirmante,
        string? PuestoFirmante,
        string? MensajeError
    );

    /// <summary>
    /// Firma una carta de forma local: elige al firmante activo con vigencia operativa hoy,
    /// abre su PFX, arma la cadena original (<c>CadenaOriginalCarta</c>) y la firma con
    /// <see cref="ICertificadoFirmaService"/>. Nunca lanza por fallas de firma: las devuelve en
    /// el resultado para que la solicitud (ya persistida) quede reintentable.
    /// </summary>
    public interface IFirmaCartaService
    {
        Task<ResultadoFirmaCarta> FirmarAsync(Datos dato, string tipoCartaClave, CancellationToken ct = default);
    }
}
