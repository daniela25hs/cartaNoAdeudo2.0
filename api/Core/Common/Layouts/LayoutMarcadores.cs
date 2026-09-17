namespace CartaNoAdeudoApi.Core.Common.Layouts
{
    /// <summary>
    /// Campos de <c>carta.Datos</c> (y relacionados) que un layout .docx puede referenciar
    /// como marcador <c>{{Campo}}</c>. RF-002: el alta/edición de un layout extrae sus
    /// marcadores y los valida contra esta lista antes de permitir activarlo.
    ///
    /// TODO: confirmar con negocio la lista exacta de <see cref="Obligatorios"/> — se
    /// asume que un layout no puede activarse sin folio, fecha de firma y firma, por ser
    /// los datos que solo existen una vez la carta fue firmada.
    /// </summary>
    public static class LayoutMarcadores
    {
        public static readonly IReadOnlyList<string> Reconocidos =
        [
            "Nombre", "Rfc", "RO", "Email", "InicioVigencia", "Vencimiento",
            "Alcoholes", "Folio", "FechaFirmado", "TipoCarta", "Firma"
        ];

        public static readonly IReadOnlyList<string> Obligatorios =
        [
            "Nombre", "Rfc", "Folio", "FechaFirmado", "Firma"
        ];
    }
}
