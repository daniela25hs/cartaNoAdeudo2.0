namespace CartaNoAdeudoApi.Core.Utils
{
    /// <summary>
    /// Contador en memoria de cuántos intentos de firma está procesando <c>TareaFirmaWorker</c>
    /// ahora mismo, y el máximo simultáneo visto desde que arrancó el proceso. Solo para
    /// monitoreo (ver <c>MonitoreoController</c>); vive en memoria, así que se reinicia en cada
    /// despliegue/reinicio, igual que <c>ITareaFirmaQueue</c>.
    /// </summary>
    public static class TareaFirmaMetrics
    {
        private static int _procesando;
        private static int _maximoProcesando;

        public static int Procesando => _procesando;
        public static int MaximoProcesando => _maximoProcesando;

        public static void Iniciar()
        {
            var actual = Interlocked.Increment(ref _procesando);
            ActualizarMaximo(actual);
        }

        public static void Terminar() => Interlocked.Decrement(ref _procesando);

        private static void ActualizarMaximo(int valor)
        {
            int actual;
            do
            {
                actual = _maximoProcesando;
                if (valor <= actual) return;
            } while (Interlocked.CompareExchange(ref _maximoProcesando, valor, actual) != actual);
        }
    }
}
