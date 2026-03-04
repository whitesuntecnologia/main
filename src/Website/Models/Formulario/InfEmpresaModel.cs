using FluentValidation;

namespace Website.Models.Formulario
{
    public class InfEmpresaModel
    {
        public int? IdTramiteInfEmp { get; set; }
        public DateTime? FechaInicioActividades { get; set; }
        public int? IdFileConstanciaInscImpNacionales { get; set; }
        public string? FilenameConstanciaInscImpNacionales { get; set; }
        public int? IdFileConstanciaBcra { get; set; }
        public string? FilenameConstanciaBcra { get; set; }

        public int? AniosAntiguedad
        {
            get
            {
                if (FechaInicioActividades.HasValue)
                {
                    var years = DateTime.Today.Year - FechaInicioActividades.Value.Year;
                    // Si todavía no pasó el aniversario en el año final, resto 1
                    if (DateTime.Today < FechaInicioActividades.Value.AddYears(years))
                    {
                        years--;
                    }

                    return years;
                }
                return null;
            }
        }
    }
    public class InfEmpresaModelValidator : AbstractValidator<InfEmpresaModel>
    {
        public InfEmpresaModelValidator()
        {

            RuleFor(p => p.FechaInicioActividades)
                .NotEmpty()
                .WithMessage("El campo es requerido");


            RuleFor(p => p.IdFileConstanciaInscImpNacionales)
            .NotEmpty()
            .WithMessage($"El campo es requerido");


            RuleFor(p => p.IdFileConstanciaBcra)
            .NotEmpty()
            .WithMessage($"El campo es requerido");

        }
    }
}
