using CartaNoAdeudoApi.Core.Entities.Catalogos;
using FluentValidation;

namespace CartaNoAdeudoApi.Core.Utils.Validaciones.Firmantes
{

    public class FirmanteValidator : BaseFluentValidator<Firmante>
    {
        public FirmanteValidator()
        {
            ValidarCampoObligatorioYMaxLength(f => f.Nombre, "Nombre", 255);

            RuleFor(f => f.Puesto)
                .MaximumLength(255).WithMessage("Puesto no debe superar los 255 caracteres")
                .When(f => !string.IsNullOrEmpty(f.Puesto));

            RuleFor(f => f.Genero)
                .IsInEnum().WithMessage("Género inválido");

            RuleFor(f => f.Certificado)
                .MaximumLength(500).WithMessage("Certificado no debe superar los 500 caracteres")
                .When(f => !string.IsNullOrEmpty(f.Certificado));

            RuleFor(f => f.Pfx)
                .MaximumLength(500).WithMessage("Pfx no debe superar los 500 caracteres")
                .When(f => !string.IsNullOrEmpty(f.Pfx));

            RuleFor(f => f.Contrasena)
                .MaximumLength(500).WithMessage("Contraseña no debe superar los 500 caracteres")
                .When(f => !string.IsNullOrEmpty(f.Contrasena));

            RuleFor(f => f.EstatusCertificado)
                .MaximumLength(50).WithMessage("EstatusCertificado no debe superar los 50 caracteres")
                .When(f => !string.IsNullOrEmpty(f.EstatusCertificado));

            RuleFor(f => f.Contrasena)
                .NotEmpty().WithMessage("La contraseña es obligatoria cuando hay un Pfx cargado")
                .When(f => !string.IsNullOrEmpty(f.Pfx));

            RuleFor(f => f.FechaFinCertificado)
                .GreaterThan(f => f.FechaInicioCertificado)
                .WithMessage("FechaFinCertificado debe ser posterior a FechaInicioCertificado");

            RuleFor(f => f.FechaFinAutorizada)
                .GreaterThan(f => f.FechaInicioAutorizada)
                .WithMessage("FechaFinAutorizada debe ser posterior a FechaInicioAutorizada");

            RuleFor(f => f.FechaInicioAutorizada)
                .GreaterThanOrEqualTo(f => f.FechaInicioCertificado)
                .WithMessage("La vigencia operativa no puede iniciar antes que la vigencia del certificado");

            RuleFor(f => f.FechaFinAutorizada)
                .LessThanOrEqualTo(f => f.FechaFinCertificado)
                .WithMessage("La vigencia operativa no puede exceder la vigencia del certificado");

            RuleFor(f => f.Certificado)
                .NotEmpty().WithMessage("Un firmante Activo debe tener Certificado cargado")
                .When(f => f.Activo);

            RuleFor(f => f.Pfx)
                .NotEmpty().WithMessage("Un firmante Activo debe tener el Pfx cargado")
                .When(f => f.Activo);

            RuleFor(f => f.EstatusCertificado)
                .NotEmpty().WithMessage("Un firmante Activo debe tener EstatusCertificado")
                .When(f => f.Activo);
        }
    }
}
