using FluentValidation;

namespace Website.Models.Parametros
{
    public class IndiceBancoCentralModel
    {
        public int? IdIndiceBcra { get; set; }
        public int? IdSituacionBcra { get; set; }
        public string? NombreSituacionBcra { get; set; }
        public decimal? MinimoDeudaAdmisible { get; set; }
        public decimal? MaximoDeudaAdmisible { get; set; }
    }
    public class IndiceBancoCentralModelValidator : AbstractValidator<IndiceBancoCentralModel>
    {
        public IndiceBancoCentralModelValidator()
        {
            RuleFor(p => p.IdSituacionBcra)
               .NotEmpty()
               .WithMessage("El campo es requerido");
            
            RuleFor(p => p.MinimoDeudaAdmisible)
               .NotEmpty()
               .WithMessage("El campo es requerido");

            RuleFor(p => p.MaximoDeudaAdmisible)
               .NotEmpty()
               .WithMessage("El campo es requerido");

            RuleFor(p => p.MinimoDeudaAdmisible)
                .InclusiveBetween(0.01m, 100m)
                .WithMessage($"El porcentaje debe ser mayor a 0 y menor o igual a 100.");

            RuleFor(p => p.MaximoDeudaAdmisible)
                .InclusiveBetween(0.01m, 100m)
                .WithMessage($"El porcentaje debe ser mayor a 0 y menor o igual a 100.");
        }
    }
}
