using FluentValidation;
using CartaNoAdeudoApi.Core.Entities.Cartas;

namespace CartaNoAdeudoApi.Core.Utils.Validaciones.Carta
{
    public class CartaValidator : BaseFluentValidator<Datos>
    {
        public CartaValidator()
        {
            RuleFor(d => d.IdTipoCarta)
                .NotEmpty().WithMessage("El tipo de carta es obligatorio");

            ValidarRFC(d => d.Rfc, "RFC");
            ValidarCampoObligatorioYMaxLength(d => d.Nombre, "Nombre", 255);
            ValidarCampoObligatorioYMaxLength(d => d.RO, "RO", 25);
            ValidarCampoObligatorioYMaxLength(d => d.Email, "Email", 100);

            RuleFor(d => d.Email)
                .EmailAddress().WithMessage("Email inválido")
                .When(d => !string.IsNullOrEmpty(d.Email));

            RuleFor(d => d.Alcoholes)
                .MaximumLength(20).WithMessage("Alcoholes no debe superar los 20 caracteres")
                .When(d => !string.IsNullOrEmpty(d.Alcoholes));

            RuleFor(d => d.Folio)
                .MaximumLength(100).WithMessage("Folio no debe superar los 100 caracteres")
                .When(d => !string.IsNullOrEmpty(d.Folio));

            ValidarCampoObligatorioFecha(d => d.InicioVigencia, "InicioVigencia");
            ValidarCampoObligatorioFecha(d => d.Vencimiento, "Vencimiento");

            RuleFor(d => d.Vencimiento)
                .GreaterThan(d => d.InicioVigencia)
                .WithMessage("Vencimiento debe ser posterior a InicioVigencia");

            RuleFor(d => d.TipoCarta)
                .Must(tipo => tipo is null || tipo.Activo)
                .WithMessage("El tipo de carta seleccionado no está activo");

            RuleFor(d => d.Folio)
                .Must((dato, folio) => dato.Estatus != EstatusSolicitud.Firmada || !string.IsNullOrWhiteSpace(folio))
                .WithMessage("Una carta Firmada debe tener Folio asignado");

            RuleFor(d => d.FechaFirmado)
                .Must((dato, fechaFirmado) => dato.Estatus != EstatusSolicitud.Firmada || fechaFirmado.HasValue)
                .WithMessage("Una carta Firmada debe tener FechaFirmado")
                .Must((dato, fechaFirmado) => dato.Estatus == EstatusSolicitud.Firmada || !fechaFirmado.HasValue)
                .WithMessage("FechaFirmado solo aplica cuando el estatus es Firmada");

            RuleFor(d => d.Firmas)
                .Must((dato, firmas) => dato.Estatus != EstatusSolicitud.Firmada || firmas.Count > 0)
                .WithMessage("Una carta Firmada debe tener al menos una firma registrada");
        }
    }
}
