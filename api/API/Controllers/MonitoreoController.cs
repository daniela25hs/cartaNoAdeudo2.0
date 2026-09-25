using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CartaNoAdeudoApi.Core.Utils;

namespace CartaNoAdeudoApi.API.Controllers
{
    /// <summary>
    /// Diagnóstico del proceso y de <c>TareaFirmaWorker</c> (cola en memoria de reintentos de
    /// firma): útil para confirmar, durante pruebas, que hay solicitudes procesándose en
    /// background. Adaptado del <c>JobsController</c> del proyecto demo api-tareas-firma. No
    /// expone datos de negocio, solo métricas del proceso.
    /// </summary>
    [ApiController]
    [Route("api/monitoreo")]
    public class MonitoreoController : ControllerBase
    {
        [HttpGet("debug")]
        public IActionResult Debug()
        {
            var proceso = Process.GetCurrentProcess();

            ThreadPool.GetAvailableThreads(out var workerDisponibles, out var completionDisponibles);
            ThreadPool.GetMaxThreads(out var workerMax, out var completionMax);

            return Ok(new
            {
                Pid = proceso.Id,
                Hilos = proceso.Threads.Count,
                MemoriaMb = proceso.WorkingSet64 / 1024 / 1024,
                WorkerDisponibles = workerDisponibles,
                WorkerMax = workerMax,
                CompletionDisponibles = completionDisponibles,
                CompletionMax = completionMax,
                TiempoCpu = proceso.TotalProcessorTime,
                IniciadoEn = proceso.StartTime
            });
        }

        [HttpPost("gc")]
        public IActionResult ForzarGc()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            return Ok("GC ejecutado");
        }

        [HttpGet("metrics")]
        public IActionResult Metrics() => Ok(new
        {
            ProcesandoActual = TareaFirmaMetrics.Procesando,
            MaximoConcurrentes = TareaFirmaMetrics.MaximoProcesando
        });
    }
}
