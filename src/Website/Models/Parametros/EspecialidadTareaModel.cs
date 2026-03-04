using FluentValidation;

namespace Website.Models.Parametros
{
    public class EspecialidadTareaModel
    {
        public int? IdTarea { get; set; }
        public string? DescripcionTarea { get; set; } = null!;
        public int? IdSeccion { get; set; }
        public bool Baja { get; set; }
    }
    public class EspecialidadTareaModelValidator : AbstractValidator<EspecialidadTareaModel>
    {
        public EspecialidadTareaModelValidator()
        {

            RuleFor(p => p.DescripcionTarea)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.IdSeccion)
            .NotEmpty()
            .WithMessage("El campo es requerido");
        }
    }
}
