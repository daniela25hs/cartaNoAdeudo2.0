namespace CartaNoAdeudoApi.Core.Common.Layouts
{
    /// <summary>
    /// Campos de <c>carta.Datos</c> (y relacionados, como el <c>Firmante</c> activo) que un
    /// layout .docx puede referenciar como marcador <c>{{Campo}}</c>. RF-002: el alta/edición
    /// de un layout extrae sus marcadores y los valida contra esta lista antes de permitir
    /// activarlo.
    ///
    /// TODO: confirmar con negocio la lista exacta de <see cref="Obligatorios"/> — se
    /// asume que un layout no puede activarse sin folio, fecha de firma (como
    /// <c>FechaFirmado</c> o como <c>Dia</c>+<c>Mes</c>+<c>Anio</c>) y firma, por ser
    /// los datos que solo existen una vez la carta fue firmada.
    ///
    /// TODO: <c>Dia</c>/<c>Mes</c>/<c>Anio</c>/<c>DiaLetra</c>/<c>AnioLetra</c>/<c>CodigoQR</c>
    /// se agregaron a partir de la plantilla real (Template_Carta_No_Adeudo.docx) pero su
    /// resolución (derivar de <c>FechaFirmado</c>, convertir día/año a letra, generar el QR)
    /// todavía no está implementada — ver generación del .docx final, pendiente junto con el PDF.
    /// </summary>
    public static class LayoutMarcadores
    {
        public static readonly IReadOnlyList<string> Reconocidos =
        [
            "Nombre", "Rfc", "RO", "Email", "InicioVigencia", "Vencimiento",
            "Alcoholes", "Folio", "FechaFirmado", "TipoCarta", "Firma",
            "Dia", "Mes", "Anio", "Puesto", "FundamentoLegal", "CodigoQR",
            // Nombre del Firmante activo (distinto de "Nombre", que es el contribuyente);
            // sin este marcador separado un layout no puede distinguir a las dos personas.
            "NombreFirmante",
            // Día y año del mismo FechaFirmado pero escritos con letra ("veintisiete",
            // "dos mil veintitrés"), para el párrafo final. "Mes" no necesita su propia
            // versión en letra porque el nombre del mes ya es texto.
            "DiaLetra", "AnioLetra"
        ];

        public static readonly IReadOnlyList<string> Obligatorios =
        [
            "Nombre", "Rfc", "Folio", "Firma"
        ];

        /// <summary>
        /// Grupos de marcadores donde basta con que estén todos los de una de las
        /// alternativas para satisfacer la obligación de "fecha de firma" (el layout puede
        /// mostrarla como una sola fecha o como sus componentes por separado).
        /// </summary>
        public static readonly IReadOnlyList<IReadOnlyList<string>> ObligatoriosAlternativos =
        [
            ["FechaFirmado"],
            ["Dia", "Mes", "Anio"]
        ];
    }
}
