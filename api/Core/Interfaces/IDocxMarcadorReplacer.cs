namespace CartaNoAdeudoApi.Core.Interfaces
{
    /// <summary>
    /// Combina un layout .docx (RF-002) con los valores reales de una solicitud, sustituyendo
    /// cada marcador <c>{{Campo}}</c> reconocido por su valor. A diferencia de
    /// <see cref="IMarcadorExtractor"/> (que solo lee), esta interfaz escribe: produce un .docx
    /// nuevo, listo para convertirse a PDF.
    /// </summary>
    public interface IDocxMarcadorReplacer
    {
        /// <param name="docx">Contenido del layout .docx activo.</param>
        /// <param name="valores">Marcador (sin llaves) -&gt; texto a insertar.</param>
        /// <param name="imagenQr">
        /// PNG del código QR a insertar donde aparezca el marcador <c>{{CodigoQR}}</c>
        /// (se ignora si ese marcador no está en el documento, o si es null).
        /// </param>
        byte[] Reemplazar(byte[] docx, IReadOnlyDictionary<string, string> valores, byte[]? imagenQr = null);
    }
}
