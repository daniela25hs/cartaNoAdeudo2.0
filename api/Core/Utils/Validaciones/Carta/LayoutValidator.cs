using FluentValidation;
using CartaNoAdeudoApi.Core.Entities.Catalogos;

namespace CartaNoAdeudoApi.Core.Utils.Validaciones.Carta
{
    public class LayoutValidator : BaseFluentValidator<Layout>
    {
        public LayoutValidator()
        {
            ValidarCampoObligatorioYMaxLength(l => l.Archivo, "Archivo", 500);

            RuleFor(l => l.Descripcion)
                .MaximumLength(500).WithMessage("Descripcion no debe superar los 500 caracteres")
                .When(l => !string.IsNullOrEmpty(l.Descripcion));

            RuleFor(l => l.FechaFinAutorizada)
                .GreaterThan(l => l.FechaInicioAutorizada)
                .WithMessage("FechaFinAutorizada debe ser posterior a FechaInicioAutorizada");
        }
    }
}
