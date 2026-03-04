using FluentValidation;

namespace Website.Models.Formulario
{
    public class ObrasModel
    {

        public int? IdTramiteObra { get; set; }
        public int IdTramite { get; set; }
        public int? IdTramiteEspecialidad { get; set; }
        public string? NombreEspecialidad { get; set; }
        public int? IdTramiteEspecialidadSeccion { get; set; }
        public string? NombreSeccion { get; set; }
        public int? IdObraPciaLP { get; set; }
        public string? Expediente { get; set; }
        public string Designacion { get; set; } = null!;
        public string Comitente { get; set; } = null!;
        public int? IdTipoObra { get; set; }
        public decimal? Monto { get; set; }
        public int? MesBase { get; set; } = null!;
        public int? AnioBase { get; set; } = null!;

        public string? PeriodoBase
        {
            get
            {
                return (this.MesBase.HasValue && this.AnioBase.HasValue ? MesBase?.ToString().PadLeft(2,Convert.ToChar("0"))  + "/" + AnioBase.ToString(): null);
            }
            set
            {
                if(!string.IsNullOrEmpty(value))
                {
                    var arr = value.Split("/");
                    if(arr.Length == 2)
                    {
                        MesBase = Convert.ToInt32(arr[0]);
                        AnioBase = Convert.ToInt32(arr[1]);
                    }
                }
            }
        }
        public int? PeriodoBaseNumber
        {
            get
            {
                return (this.AnioBase * 100 + this.MesBase);
            }
        }
        public int? MesContrato { get; set; } = null!;
        public int? AnioContrato { get; set; } = null!;
        public string? PeriodoContrato
        {
            get
            {
                return (this.MesContrato.HasValue && this.AnioContrato.HasValue ? MesContrato?.ToString().PadLeft(2, Convert.ToChar("0")) + "/" + AnioContrato.ToString() : null);
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var arr = value.Split("/");
                    if (arr.Length == 2)
                    {
                        MesContrato = Convert.ToInt32(arr[0]);
                        AnioContrato = Convert.ToInt32(arr[1]);
                    }
                }
            }
        }
        public int? PeriodoContratoNumber
        {
            get
            {
                return (this.AnioContrato * 100 + this.MesContrato);
            }
        }
        public int? MesFin { get; set; } = null!;
        public int? AnioFin { get; set; } = null!;
        public string? PeriodoFin
        {
            get
            {
                return (this.MesFin.HasValue && this.AnioFin.HasValue ? MesFin?.ToString().PadLeft(2, Convert.ToChar("0")) + "/" + AnioFin.ToString() : null);
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var arr = value.Split("/");
                    if (arr.Length == 2)
                    {
                        MesFin = Convert.ToInt32(arr[0]);
                        AnioFin = Convert.ToInt32(arr[1]);
                    }
                }
            }
        }
        public int? PeriodoFinNumber
        {
            get
            {
                return (this.AnioFin * 100 + this.MesFin);
            }
        }
        public int? IdFile { get; set; }

        public string? Filename { get; set; } = null!;
        


    }
    public class ObrasModelValidator : AbstractValidator<ObrasModel>
    {
        public ObrasModelValidator()
        {

            RuleFor(p => p.IdTramiteEspecialidad)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.IdTramiteEspecialidadSeccion)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.IdObraPciaLP)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.Comitente)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.IdTipoObra)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.Monto)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.MesBase)
            .NotEmpty()
            .When(x=> x.AnioBase.HasValue)
            .WithMessage("El Mes es requerido");

            RuleFor(p => p.AnioBase)
            .NotEmpty()
            .WithMessage("El Año es requerido");

            RuleFor(p => p.MesContrato)
            .NotEmpty()
            .When(x => x.AnioContrato.HasValue)
            .WithMessage("El Mes es requerido");

            RuleFor(p => p.AnioContrato)
            .NotEmpty()
            .WithMessage("El Año es requerido");

            RuleFor(p => p.PeriodoContratoNumber)
            .GreaterThanOrEqualTo(p => p.PeriodoBaseNumber)
            .When(p => p.PeriodoContratoNumber.HasValue && p.PeriodoBaseNumber.HasValue)
            .WithMessage("El período de contrato debe ser mayor al período base");

            RuleFor(p => p.MesFin)
            .NotEmpty()
            .When(x => x.AnioFin.HasValue)
            .WithMessage("El Mes es requerido");

            RuleFor(p => p.AnioFin)
            .NotEmpty()
            .WithMessage("El año es requerido");

            RuleFor(p => p.PeriodoFinNumber)
            .GreaterThanOrEqualTo(p => p.PeriodoContratoNumber)
            .When(p => p.PeriodoFinNumber.HasValue && p.PeriodoContratoNumber.HasValue)
            .WithMessage("El período de fin debe ser mayor al período de inicio del contrato");

            RuleFor(model => model.AnioFin)
              .Custom((anio, context) =>
              {
                  var m = (ObrasModel)context.InstanceToValidate;
                  bool valida = true;
                  //Verifica que el período no sea menor a 5 años desde la actualizad
                  //y que tampoco sea mayor a la actualidad
                  if (m.MesFin.HasValue && m.AnioFin.HasValue)
                  {

                      var DateMinValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddYears(-5);
                      var DateMaxValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                      var PeriodoValue = new DateTime(m.AnioFin.Value, m.MesFin.Value, 1);

                      if (PeriodoValue < DateMinValue)
                          valida = false;
                      else if (PeriodoValue > DateMaxValue)
                          valida = false;
                  }

                  if (!valida)
                  {
                      context.AddFailure("AnioFin", $"El período ingresado no puede ser anterior a 5 años del período actual.");
                  }
              })
              .When(x => x.AnioFin.HasValue && x.MesFin.HasValue);

            RuleFor(p => p.IdFile)
            .NotEmpty()
            .WithMessage("El campo es requerido");


        }
    }
}
