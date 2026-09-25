using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CartaNoAdeudoApi.Core.Interfaces;
using CartaNoAdeudoApi.Core.Options;

namespace CartaNoAdeudoApi.Infrastructure.Services
{
    /// <summary>Convierte un .docx a PDF invocando "soffice --headless --convert-to pdf" como
    /// proceso externo. Cada conversión usa un directorio de trabajo y un perfil de LibreOffice
    /// propios (aislados con -env:UserInstallation) para poder correr en paralelo sin pisarse
    /// el lockfile del perfil.</summary>
    public class LibreOfficePdfConverter(IOptions<PdfOptions> options, ILogger<LibreOfficePdfConverter> logger) : IPdfConverter
    {
        private readonly PdfOptions _options = options.Value;

        public async Task<byte[]> ConvertirAsync(byte[] docx, CancellationToken ct = default)
        {
            var directorioTrabajo = Path.Combine(Path.GetTempPath(), "carta-pdf-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directorioTrabajo);

            var docxPath = Path.Combine(directorioTrabajo, "carta.docx");
            var perfilUri = new Uri(Path.Combine(directorioTrabajo, "lo-profile")).AbsoluteUri;

            using var proceso = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _options.LibreOfficePath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                }
            };
            proceso.StartInfo.ArgumentList.Add("--headless");
            proceso.StartInfo.ArgumentList.Add("--norestore");
            proceso.StartInfo.ArgumentList.Add($"-env:UserInstallation={perfilUri}");
            proceso.StartInfo.ArgumentList.Add("--convert-to");
            proceso.StartInfo.ArgumentList.Add("pdf");
            proceso.StartInfo.ArgumentList.Add("--outdir");
            proceso.StartInfo.ArgumentList.Add(directorioTrabajo);
            proceso.StartInfo.ArgumentList.Add(docxPath);

            try
            {
                await File.WriteAllBytesAsync(docxPath, docx, ct);

                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(_options.TimeoutSegundos));
                using var enlazado = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

                proceso.Start();
                var salidaTask = proceso.StandardOutput.ReadToEndAsync(enlazado.Token);
                var errorTask = proceso.StandardError.ReadToEndAsync(enlazado.Token);

                try
                {
                    await proceso.WaitForExitAsync(enlazado.Token);
                }
                catch (OperationCanceledException) when (!ct.IsCancellationRequested)
                {
                    TryKill(proceso);
                    throw new TimeoutException(
                        $"La conversión a PDF con LibreOffice superó los {_options.TimeoutSegundos}s.");
                }

                var salida = await salidaTask;
                var error = await errorTask;

                if (proceso.ExitCode != 0)
                    throw new InvalidOperationException(
                        $"LibreOffice terminó con código {proceso.ExitCode}. stderr: {error}. stdout: {salida}");

                var pdfPath = Path.Combine(directorioTrabajo, "carta.pdf");
                if (!File.Exists(pdfPath))
                    throw new InvalidOperationException(
                        $"LibreOffice no generó el PDF esperado en '{pdfPath}'. stderr: {error}. stdout: {salida}");

                logger.LogInformation("PDF generado con LibreOffice ({Bytes} bytes).", new FileInfo(pdfPath).Length);
                return await File.ReadAllBytesAsync(pdfPath, ct);
            }
            finally
            {
                TryKill(proceso);
                try { Directory.Delete(directorioTrabajo, recursive: true); } catch { /* best-effort cleanup */ }
            }
        }

        private static void TryKill(Process proceso)
        {
            try
            {
                if (!proceso.HasExited)
                    proceso.Kill(entireProcessTree: true);
            }
            catch { /* ya terminó o no se pudo matar; no es fatal */ }
        }
    }
}
