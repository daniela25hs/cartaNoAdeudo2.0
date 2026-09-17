using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using CartaNoAdeudoApi.Core.Interfaces;

namespace CartaNoAdeudoApi.Infrastructure.Services
{
    /// <summary>
    /// Extrae los marcadores <c>{{Campo}}</c> del texto de un .docx (RF-002). Se usa
    /// <c>Body.InnerText</c> (concatena todo el texto del documento) en vez de leer
    /// cada <c>&lt;w:t&gt;</c> por separado, porque Word suele partir un mismo marcador
    /// en varios runs (autocorrección, revisiones) y así no se pierden fragmentados.
    /// </summary>
    public partial class DocxMarcadorExtractor : IMarcadorExtractor
    {
        [GeneratedRegex(@"\{\{\s*(\w+)\s*\}\}")]
        private static partial Regex MarcadorRegex();

        public Task<IReadOnlyList<string>> ExtraerAsync(Stream docx, CancellationToken ct = default)
        {
            using var documento = WordprocessingDocument.Open(docx, false);
            var texto = documento.MainDocumentPart?.Document.Body?.InnerText ?? string.Empty;

            IReadOnlyList<string> marcadores = MarcadorRegex().Matches(texto)
                .Select(m => m.Groups[1].Value)
                .Distinct()
                .ToList();

            return Task.FromResult(marcadores);
        }
    }
}
