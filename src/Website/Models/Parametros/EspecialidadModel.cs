using FluentValidation;

namespace Website.Models.Parametros
{
    public class EspecialidadModel
    {
        public int? IdEspecialidad { get; set; }

        public string? NombreEspecialidad { get; set; } = null!;

        public string? Rama { get; set; } = null!;

        public bool Baja { get; set; }
    }
    public class EspecialidadModelValidator : AbstractValidator<EspecialidadModel>
    {
        public EspecialidadModelValidator()
        {

            RuleFor(p => p.NombreEspecialidad)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.Rama)
            .NotEmpty()
            .WithMessage("El campo es requerido");
        }
    }
}
