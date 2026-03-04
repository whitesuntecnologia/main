using FluentValidation;
using Radzen.Blazor;
using Website.Models.Formulario;

namespace Website.Models.Parametros
{
    public class IndiceConstruccionModel
    {
        public int? IdIcc { get; set; }
        public int? Mes { get; set; }
        public int? Anio { get; set; }
        public decimal? Valor { get; set; }
        public string? Periodo
        {
            get
            {
                return (this.Mes.HasValue && this.Anio.HasValue ? Mes?.ToString().PadLeft(2, Convert.ToChar("0")) + "/" + Anio.ToString() : null);
            }
        }
        public class IndiceConstruccionModelValidator : AbstractValidator<IndiceConstruccionModel>
        {
            public IndiceConstruccionModelValidator()
            {

                RuleFor(p => p.Mes)
                .NotEmpty()
                .WithMessage("El campo Mes es requerido");

                RuleFor(p => p.Anio)
                    .NotEmpty()
                    .WithMessage("El campo Año es requerido");

                RuleFor(p => p.Valor)
                .NotEmpty()
                .WithMessage("El campo es requerido");


                RuleFor(p => p.Anio)
                .InclusiveBetween(1950, 2050)
                .When(x => x.Anio.HasValue)
                .WithMessage($"El año ingresado en inválido");

                RuleFor(p => p.Valor)
                    .GreaterThan(0)
                    .WithMessage("El Valor debe ser mayor a cero");

            }
        }
    }
}
