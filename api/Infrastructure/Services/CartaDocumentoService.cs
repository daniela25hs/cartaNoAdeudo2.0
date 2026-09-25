using System.Globalization;
using Microsoft.Extensions.Options;
using QRCoder;
using CartaNoAdeudoApi.Core.Entities.Cartas;
using CartaNoAdeudoApi.Core.Exceptions;
using CartaNoAdeudoApi.Core.Interfaces;
using CartaNoAdeudoApi.Core.Interfaces.Repositories;
using CartaNoAdeudoApi.Core.Options;

namespace CartaNoAdeudoApi.Infrastructure.Services
{
    public class CartaDocumentoService(
        IUnitOfWork uow,
        IDocxLayoutStorage layoutStorage,
        IDocxMarcadorReplacer replacer,
        IPdfConverter pdfConverter,
        IOptions<PdfOptions> pdfOptions) : ICartaDocumentoService
    {
        private static readonly CultureInfo CulturaEsMx = CultureInfo.GetCultureInfo("es-MX");
        private readonly PdfOptions _pdfOptions = pdfOptions.Value;

        public async Task<byte[]> GenerarPdfAsync(
            Datos dato,
            string tipoCartaClave,
            string tipoCartaDescripcion,
            string? nombreFirmante,
            string? puestoFirmante,
            string? firmaBase64,
            CancellationToken ct = default)
        {
            // El layout vigente se busca contra la fecha de firma (no "hoy"): si el layout activo
            // cambió entre que se firmó y que se generó el PDF, la carta debe seguir viendo el
            // layout que estaba vigente cuando se firmó.
            var fechaReferencia = DateOnly.FromDateTime((dato.FechaFirmado ?? DateTimeOffset.UtcNow).UtcDateTime);
            var layout = await uow.Layouts.GetActivoVigenteAsync(fechaReferencia, ct)
                ?? throw new ConflictException("No hay un layout activo y vigente para generar la carta.");

            await using var layoutStream = await layoutStorage.AbrirAsync(layout.Archivo, ct);
            using var layoutMs = new MemoryStream();
            await layoutStream.CopyToAsync(layoutMs, ct);

            var valores = ConstruirValores(dato, tipoCartaDescripcion, nombreFirmante, puestoFirmante, firmaBase64);
            var qrPng = GenerarQr(dato);

            var docxCombinado = replacer.Reemplazar(layoutMs.ToArray(), valores, qrPng);
            return await pdfConverter.ConvertirAsync(docxCombinado, ct);
        }

        private byte[]? GenerarQr(Datos dato)
        {
            if (string.IsNullOrWhiteSpace(_pdfOptions.ValidarUrlBase) || string.IsNullOrEmpty(dato.Folio))
                return null;

            var url = $"{_pdfOptions.ValidarUrlBase}?rfc={Uri.EscapeDataString(dato.Rfc)}&folio={Uri.EscapeDataString(dato.Folio)}";

            using var generador = new QRCodeGenerator();
            using var datosQr = generador.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            using var qr = new PngByteQRCode(datosQr);
            return qr.GetGraphic(20);
        }

        /// <summary>
        /// TODO: confirmar con negocio "FundamentoLegal" (se deja vacío: es contenido legal
        /// que no corresponde inventar en código).
        /// </summary>
        private static Dictionary<string, string> ConstruirValores(
            Datos dato, string tipoCartaDescripcion, string? nombreFirmante, string? puestoFirmante, string? firmaBase64)
        {
            var fechaFirmado = dato.FechaFirmado?.UtcDateTime;

            return new Dictionary<string, string>
            {
                ["Nombre"] = dato.Nombre,
                ["Rfc"] = dato.Rfc,
                ["RO"] = dato.RO,
                ["Email"] = dato.Email,
                ["InicioVigencia"] = dato.InicioVigencia.ToString("dd/MM/yyyy"),
                ["Vencimiento"] = dato.Vencimiento.ToString("dd/MM/yyyy"),
                ["Alcoholes"] = dato.Alcoholes ?? "",
                ["Folio"] = dato.Folio ?? "",
                ["FechaFirmado"] = fechaFirmado?.ToString("dd 'de' MMMM 'de' yyyy", CulturaEsMx) ?? "",
                ["TipoCarta"] = tipoCartaDescripcion,
                ["Firma"] = firmaBase64 ?? "",
                ["Dia"] = fechaFirmado?.Day.ToString() ?? "",
                ["Mes"] = fechaFirmado is null ? "" : CulturaEsMx.DateTimeFormat.GetMonthName(fechaFirmado.Value.Month),
                ["Anio"] = fechaFirmado?.Year.ToString() ?? "",
                ["Puesto"] = puestoFirmante ?? "",
                ["FundamentoLegal"] = "",
                ["NombreFirmante"] = nombreFirmante ?? "",
            };
        }
    }
}
