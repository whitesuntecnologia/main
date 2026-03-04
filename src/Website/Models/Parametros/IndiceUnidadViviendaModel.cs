using FluentValidation;

namespace Website.Models.Parametros
{
    public class IndiceUnidadViviendaModel
    {
        public int? IdUvi { get; set; }
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
        public class IndiceUnidadViviendaModelValidator : AbstractValidator<IndiceUnidadViviendaModel>
        {
            public IndiceUnidadViviendaModelValidator()
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
