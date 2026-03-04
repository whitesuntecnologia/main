using FluentValidation;

namespace Website.Models.Shared
{
    public class ReAsignarEvaluadorModel
    {
        public string userid { get; set; } = null!;
    }
    public class ReAsignarEvaluadorModelValidator : AbstractValidator<ReAsignarEvaluadorModel>
    {
        public ReAsignarEvaluadorModelValidator()
        {

            RuleFor(p => p.userid)
                .NotEmpty()
                .WithMessage("El campo es requerido");

        }
    }
}
