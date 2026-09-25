using System.Text.RegularExpressions;
using CartaNoAdeudoApi.Core.Entities.Cartas;

namespace CartaNoAdeudoApi.Core.Utils
{
    /// <summary>
    /// Arma la cadena a firmar de una carta con la convención de "cadena original" del SAT:
    /// <c>||campo1|campo2|...||</c>, campos en orden fijo, valores vacíos omitidos, espacios
    /// repetidos colapsados y sin el carácter <c>|</c> dentro de los valores.
    ///
    /// TODO: confirmar con negocio qué campos y en qué orden entran. El orden es parte del
    /// contrato: cambiarlo invalida la verificación de todas las cartas ya firmadas.
    /// </summary>
    public static partial class CadenaOriginalCarta
    {
        public static string Construir(Datos dato, string tipoCartaClave) => Construir(
            dato.Folio,
            tipoCartaClave,
            dato.Rfc,
            dato.Nombre,
            dato.RO,
            dato.InicioVigencia.ToString("yyyy-MM-dd"),
            dato.Vencimiento.ToString("yyyy-MM-dd"),
            dato.Alcoholes);

        public static string Construir(params string?[] campos) =>
            "||" + string.Join("|", campos.Select(Normalizar).Where(c => c.Length > 0)) + "||";

        private static string Normalizar(string? valor) =>
            string.IsNullOrWhiteSpace(valor)
                ? string.Empty
                : Espacios().Replace(valor.Replace("|", " "), " ").Trim();

        [GeneratedRegex(@"\s+")]
        private static partial Regex Espacios();
    }
}
