using FluentValidation;

namespace Website.Models.Formulario
{
    public class CertificadoLicitarModel
    {
        public int? IdObraPciaLP { get; set; }

    }
    public class CertificadoLicitarModelValidator : AbstractValidator<CertificadoLicitarModel>
    {
        public CertificadoLicitarModelValidator()
        {

            RuleFor(p => p.IdObraPciaLP)
                .NotEmpty()
                .WithMessage("El campo es requerido");
        }
    }
}
