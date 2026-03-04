using FluentValidation;

namespace Website.Models.Formulario
{
    public class PresentaEnUteModel
    {
        public bool SePresentaEnUte { get; set; }
        public decimal? PorcParticipUte { get; set; }
    }

    public class PresentaEnUteModelValidator : AbstractValidator<PresentaEnUteModel>
    {
        public PresentaEnUteModelValidator()
        {

            RuleFor(p => p.PorcParticipUte)
              .NotEmpty().WithMessage("El Porcentaje no debe estar vacío.")
              .GreaterThan(0).WithMessage("El Porcentaje debe ser mayor a Cero.")
              .When(p => p.SePresentaEnUte);
                

            RuleFor(p => p.PorcParticipUte)
                .Empty()
                .When(p => !p.SePresentaEnUte)
                .WithMessage("El Porcentaje debe estar vacío si no se presenta en UTE.");


            
            
        }

    }
}
