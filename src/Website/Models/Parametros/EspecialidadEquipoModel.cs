using FluentValidation;

namespace Website.Models.Parametros
{
    public class EspecialidadEquipoModel
    {
        public int? IdEquipo { get; set; }
        public string? DescripcionEquipo { get; set; } = null!;
        public bool Baja { get; set; }
    }
    public class EspecialidadEquipoModelValidator : AbstractValidator<EspecialidadEquipoModel>
    {
        public EspecialidadEquipoModelValidator()
        {

            RuleFor(p => p.DescripcionEquipo)
            .NotEmpty()
            .WithMessage("El campo es requerido");

        }
    }
}