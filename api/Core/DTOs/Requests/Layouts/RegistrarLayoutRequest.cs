using System.ComponentModel.DataAnnotations;

namespace CartaNoAdeudoApi.Core.DTOs.Requests.Layouts
{
    /// <summary>
    /// Metadatos capturados por el administrador al cargar una plantilla
    /// <c>.docx</c> para Carta de No Adeudo (RF-002). El archivo viaja como
    /// <c>IFormFile</c> en el controller.
    ///
    /// NOTA: RF-002 lista "Nombre del layout" como entrada, pero <c>cat.Layout</c>
    /// del diagrama solo tiene <c>Descripcion</c>; aquí se usa esa como rótulo.
    /// Tampoco hay tipo de carta en el layout: la selección de RF-003 asume un
    /// único layout activo y vigente.
    /// </summary>
    public record RegistrarLayoutRequest(
        [property: Required(ErrorMessage = "La descripción del layout es obligatoria")]
        string Descripcion,

        [property: Required(ErrorMessage = "La fecha de inicio de vigencia operativa es obligatoria")]
        DateOnly VigenciaInicio,

        [property: Required(ErrorMessage = "La fecha de fin de vigencia operativa es obligatoria")]
        DateOnly VigenciaFin,

        /// <summary>RF-002 entrada "Estatus del layout (Activo/Inactivo)".</summary>
        bool Activo
    );

    /// <summary>
    /// Modificación de un layout ya registrado (RF-002 "administrar"). El
    /// <c>.docx</c> es opcional al editar; si no se adjunta se conserva la
    /// plantilla actual. RN-007: los cambios no afectan documentos ya emitidos.
    /// </summary>
    public record ActualizarLayoutRequest(
        [property: Required(ErrorMessage = "La descripción del layout es obligatoria")]
        string Descripcion,

        [property: Required(ErrorMessage = "La fecha de inicio de vigencia operativa es obligatoria")]
        DateOnly VigenciaInicio,

        [property: Required(ErrorMessage = "La fecha de fin de vigencia operativa es obligatoria")]
        DateOnly VigenciaFin
    );
}
