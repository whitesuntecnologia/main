using FluentValidation;

namespace Website.Models.Seguridad
{
    public class EmpresaSancionModel
    {
        public int IdEmpresaSancion { get; set; }
        public int IdEmpresa { get; set; }
        public string Nombre { get; set; } = null!;
        public int? IdFileSancion { get; set; }
        public DateTime FechaDesdeSancion { get; set; }
        public DateTime? FechaHastaSancion { get; set; }

        // Para el nombre del archivo
        public string? FilenameSancion { get; set; }
    }
    public class EmpresaSancionModelValidator : AbstractValidator<EmpresaSancionModel>
    {
        public EmpresaSancionModelValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre de la sanción es requerido")
                .MaximumLength(150).WithMessage("El nombre no puede superar los 150 caracteres");

            RuleFor(x => x.FechaDesdeSancion)
                .NotEmpty().WithMessage("La fecha desde es requerida");

            RuleFor(x => x.FechaHastaSancion)
                .GreaterThanOrEqualTo(x => x.FechaDesdeSancion)
                .When(x => x.FechaHastaSancion.HasValue)
                .WithMessage("La fecha hasta debe ser mayor o igual a la fecha desde");



            RuleFor(p => p.IdFileSancion)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(x => x.IdFileSancion)
                .GreaterThan(0).WithMessage("Debe cargar un archivo de la sanción");
        }
    }
}

