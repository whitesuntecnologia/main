using FluentValidation;

namespace Website.Models.Parametros
{
    public class EspecialidadSeccionModel
    {
        public int? IdSeccion { get; set; }
        public string? DescripcionSeccion { get; set; } = null!;
        public int? IdEspecialidad { get; set; }
        public bool Baja { get; set; }
    }
    public class EspecialidadSeccionModelValidator : AbstractValidator<EspecialidadSeccionModel>
    {
        public EspecialidadSeccionModelValidator()
        {

            RuleFor(p => p.DescripcionSeccion)
            .NotEmpty()
            .WithMessage("El campo es requerido");
            
            RuleFor(p => p.IdEspecialidad)
            .NotEmpty()
            .WithMessage("El campo es requerido");

        }
    }
}
