using System.Linq.Expressions;
using FluentValidation;

namespace CartaNoAdeudoApi.Core.Utils.Validaciones
{
    public class BaseFluentValidator<T> : AbstractValidator<T>
    {
        protected void ValidarCampoObligatorioYMaxLength(Expression<Func<T, string>> selector, string nombreCampo, int maxLength = 50)
        {
            RuleFor(selector)
                .NotEmpty().WithMessage($"{nombreCampo} es obligatorio")
                .MaximumLength(maxLength).WithMessage($"{nombreCampo} no debe superar los {maxLength} caracteres");
        }

        protected void ValidarCampoObligatorioFecha(Expression<Func<T, DateOnly>> selector, string nombreCampo)
        {
            RuleFor(selector)
                .NotEmpty().WithMessage($"{nombreCampo} es obligatorio")
                .Must(fecha => fecha > DateOnly.MinValue)
                .WithMessage($"{nombreCampo} no es una fecha válida");
        }

        protected void ValidarRFC(Expression<Func<T, string>> selector, string nombreCampo)
        {
            const string RFC_REGEX = @"^([A-ZÑ&]{4}\d{6}[A-Z0-9]{0,3}|[A-ZÑ&]{3}-?\d{6}[A-Z0-9]{0,3})$";

            RuleFor(selector)
                .NotEmpty().WithMessage($"{nombreCampo} es obligatorio")
                .MaximumLength(13).WithMessage($"{nombreCampo} no debe superar los 13 caracteres")
                .Matches(RFC_REGEX).WithMessage($"{nombreCampo} no tiene un formato válido (ej: XXXX######XXX)");
        }
    }
}
