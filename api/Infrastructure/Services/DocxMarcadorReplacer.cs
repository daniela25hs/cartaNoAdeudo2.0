using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using CartaNoAdeudoApi.Core.Interfaces;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace CartaNoAdeudoApi.Infrastructure.Services
{
    /// <summary>
    /// Sustituye marcadores <c>{{Campo}}</c> en un .docx por valores reales, a nivel párrafo:
    /// concatena el texto de todos los <c>&lt;w:t&gt;</c> del párrafo (Word suele partir un
    /// mismo marcador en varias runs por autocorrección), hace el reemplazo sobre ese texto
    /// completo y lo deja en la primera run del párrafo, vaciando las demás. Esto simplifica
    /// runs con formato mixto dentro de un mismo párrafo a solo el formato de la primera run;
    /// aceptable para una plantilla de marcadores simples, no para texto con negritas/cursivas
    /// parciales alrededor de un marcador.
    /// </summary>
    public partial class DocxMarcadorReplacer : IDocxMarcadorReplacer
    {
        [GeneratedRegex(@"\{\{\s*(\w+)\s*\}\}")]
        private static partial Regex MarcadorRegex();

        private const string MarcadorQr = "CodigoQR";
        private const long QrTamanoEmu = 1200000L; // ~3.3cm de lado

        public byte[] Reemplazar(byte[] docx, IReadOnlyDictionary<string, string> valores, byte[]? imagenQr = null)
        {
            using var ms = new MemoryStream();
            ms.Write(docx, 0, docx.Length);
            ms.Position = 0;

            using (var documento = WordprocessingDocument.Open(ms, true))
            {
                var mainPart = documento.MainDocumentPart
                    ?? throw new InvalidOperationException("El .docx del layout no tiene MainDocumentPart.");
                var body = mainPart.Document.Body
                    ?? throw new InvalidOperationException("El .docx del layout no tiene Body.");

                foreach (var paragraph in body.Descendants<Paragraph>().ToList())
                    ReemplazarEnParrafo(paragraph, valores, mainPart, imagenQr);

                mainPart.Document.Save();
            }

            return ms.ToArray();
        }

        private static void ReemplazarEnParrafo(
            Paragraph paragraph,
            IReadOnlyDictionary<string, string> valores,
            MainDocumentPart mainPart,
            byte[]? imagenQr)
        {
            var textos = paragraph.Descendants<Text>().ToList();
            if (textos.Count == 0) return;

            var textoCompleto = string.Concat(textos.Select(t => t.Text));
            if (!textoCompleto.Contains("{{")) return;

            var tieneQr = imagenQr is not null && textoCompleto.Contains("{{" + MarcadorQr + "}}");

            var textoReemplazado = MarcadorRegex().Replace(textoCompleto, m =>
                m.Groups[1].Value == MarcadorQr
                    ? "" // el QR se inserta como imagen, no como texto
                    : valores.TryGetValue(m.Groups[1].Value, out var valor) ? valor : m.Value);

            if (textoReemplazado == textoCompleto) return;

            textos[0].Text = textoReemplazado;
            textos[0].Space = SpaceProcessingModeValues.Preserve;
            for (var i = 1; i < textos.Count; i++)
                textos[i].Text = string.Empty;

            if (tieneQr)
                paragraph.AppendChild(CrearRunConImagen(mainPart, imagenQr!));
        }

        private static Run CrearRunConImagen(MainDocumentPart mainPart, byte[] imagenPng)
        {
            var imagePart = mainPart.AddImagePart(ImagePartType.Png);
            using (var stream = new MemoryStream(imagenPng))
                imagePart.FeedData(stream);
            var relationshipId = mainPart.GetIdOfPart(imagePart);

            var drawing = new Drawing(
                new DW.Inline(
                    new DW.Extent { Cx = QrTamanoEmu, Cy = QrTamanoEmu },
                    new DW.EffectExtent { LeftEdge = 0L, TopEdge = 0L, RightEdge = 0L, BottomEdge = 0L },
                    new DW.DocProperties { Id = 1U, Name = "CodigoQR" },
                    new DW.NonVisualGraphicFrameDrawingProperties(
                        new A.GraphicFrameLocks { NoChangeAspect = true }),
                    new A.Graphic(
                        new A.GraphicData(
                            new PIC.Picture(
                                new PIC.NonVisualPictureProperties(
                                    new PIC.NonVisualDrawingProperties { Id = 0U, Name = "CodigoQR.png" },
                                    new PIC.NonVisualPictureDrawingProperties()),
                                new PIC.BlipFill(
                                    new A.Blip { Embed = relationshipId },
                                    new A.Stretch(new A.FillRectangle())),
                                new PIC.ShapeProperties(
                                    new A.Transform2D(
                                        new A.Offset { X = 0L, Y = 0L },
                                        new A.Extents { Cx = QrTamanoEmu, Cy = QrTamanoEmu }),
                                    new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle })
                            )
                        ) { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                )
                {
                    DistanceFromTop = 0U,
                    DistanceFromBottom = 0U,
                    DistanceFromLeft = 0U,
                    DistanceFromRight = 0U,
                });

            return new Run(drawing);
        }
    }
}
