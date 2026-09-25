using CartaNoAdeudoApi.Core.Entities.Cartas;

namespace CartaNoAdeudoApi.Core.Interfaces
{
    /// <summary>
    /// Orquesta la generación del PDF final de una carta ya firmada (RF-002/RF-003): toma el
    /// layout .docx activo y vigente, lo combina con los datos de <paramref name="dato"/> (más
    /// el firmante y el QR de validación) vía <see cref="IDocxMarcadorReplacer"/>, y convierte
    /// el resultado a PDF con <see cref="IPdfConverter"/>.
    /// </summary>
    public interface ICartaDocumentoService
    {
        /// <param name="dato">Debe venir con <see cref="Datos.TipoCarta"/> cargado (Include).</param>
        /// <param name="nombreFirmante">Nombre del firmante que firmó (de <c>ResultadoFirmaCarta</c>).</param>
        /// <param name="puestoFirmante">Puesto del firmante, si tiene.</param>
        /// <param name="firmaBase64">Firma RSA-SHA256 en base64 (<c>Firmas.Firma</c>) que se imprime en el marcador Firma.</param>
        Task<byte[]> GenerarPdfAsync(
            Datos dato,
            string tipoCartaClave,
            string tipoCartaDescripcion,
            string? nombreFirmante,
            string? puestoFirmante,
            string? firmaBase64,
            CancellationToken ct = default);
    }
}
