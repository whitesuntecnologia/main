using FluentValidation;
using Radzen.Blazor;

namespace Website.Models.Formulario
{
    public class AntecedentesModel
    {
        public int? IdTramiteAntecedenteObra { get; set; }
        public int IdTramiteEspecialidad { get; set; }
        public string? DescripcionEspecialidad { get; set; }
        public int IdTramiteEspecialidadSeccion { get; set; }
        public string? DescripcionSeccion { get; set; }
        public int? IdTramiteAntecedente { get; set; }
        public int? IdObraPciaLp { get; set; }
        public string? Ubicacion { get; set; } = null!;
        public string? Comitente { get; set; } = null!;
        public int? Plazo { get; set; }
        public decimal? MontoContrato { get; set; }
        public decimal? MontoEjecutado { get; set; }
        public decimal? MontoAejecutar 
        { 
            get
            {
                decimal? result = null;
                if (this.MontoContrato.HasValue && this.MontoEjecutado.HasValue)
                    result = this.MontoContrato.Value - this.MontoEjecutado.Value;
                return result;
            }
             
        }
        public string? RepresentanteTecnico { get; set; } = null!;
        public int? IdFile { get; set; }
        public string? Filename { get; set; } = null!;

        public int? MesBase { get; set; } = null!;
        public int? AnioBase { get; set; } = null!;
        public string? PeriodoBase
        {
            get
            {
                return (this.MesBase.HasValue && this.AnioBase.HasValue ? MesBase?.ToString().PadLeft(2, Convert.ToChar("0")) + "/" + AnioBase.ToString() : null);
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var arr = value.Split("/");
                    if (arr.Length == 2)
                    {
                        MesBase = Convert.ToInt32(arr[0]);
                        AnioBase = Convert.ToInt32(arr[1]);
                    }
                }
            }
        }
        public int? MesInicio { get; set; } = null!;
        public int? AnioInicio { get; set; } = null!;
        public string? PeriodoInicio
        {
            get
            {
                return (this.MesInicio.HasValue && this.AnioInicio.HasValue ? MesInicio?.ToString().PadLeft(2, Convert.ToChar("0")) + "/" + AnioInicio.ToString() : null);
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var arr = value.Split("/");
                    if (arr.Length == 2)
                    {
                        MesInicio = Convert.ToInt32(arr[0]);
                        AnioInicio = Convert.ToInt32(arr[1]);
                    }
                }
            }
        }
        public DateTime FechaInicioSolicitud { get; set; }
    }
   
    public class AntecedentesModelValidator : AbstractValidator<AntecedentesModel>
    {
        public AntecedentesModelValidator()
        {

       
            RuleFor(p => p.IdObraPciaLp)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.Ubicacion)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.Comitente)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.Plazo)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.MontoContrato)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.MontoContrato)
            .GreaterThan(0)
            .WithMessage("El valor debe ser mayor a 0.");

            RuleFor(p => p.MontoEjecutado)
            .NotEmpty()
            .WithMessage("El campo es requerido");


            RuleFor(p => p.MesBase)
            .NotEmpty()
            .When(x => x.AnioBase.HasValue)
            .WithMessage("El Mes es requerido");

            RuleFor(p => p.AnioBase)
            .NotEmpty()
            .WithMessage("El Año es requerido");

            RuleFor(p => p.AnioBase)
               .InclusiveBetween(1950, DateTime.Now.Year + 1)
               .When(x => x.AnioBase.HasValue)
               .WithMessage($"El año debe ser un valor de 1950 y {DateTime.Now.Year}");



            RuleFor(p => p.PeriodoBase)
               .Custom((value, context) =>
               {
                   var m = context.InstanceToValidate;

                   if (m.AnioBase.HasValue && m.MesBase.HasValue)
                   {
                       int PeriodoBaseNum = int.Parse($"{m.AnioBase}{string.Format("{0:00}", m.MesBase)}");
                       int PeriodoActualNum = int.Parse($"{DateTime.Today.Year}{string.Format("{0:00}",DateTime.Today.Month)}");
                       
                       if (PeriodoBaseNum > PeriodoActualNum)
                       {
                           context.AddFailure("PeriodoBase", $"El Período base no puede ser mayor al período actual.");
                       }
                   }

               });

            RuleFor(p => p.MesInicio)
            .NotEmpty()
            .When(x => x.AnioInicio.HasValue)
            .WithMessage("El Mes es requerido");

            RuleFor(p => p.AnioInicio)
            .NotEmpty()
            .WithMessage("El año es requerido");

            RuleFor(p => p.AnioInicio)
               .InclusiveBetween(1950, DateTime.Now.Year + 1)
               .When(x => x.AnioInicio.HasValue)
               .WithMessage($"El año debe ser un valor de 1950 y {DateTime.Now.Year + 1}")
               ;

            RuleFor(p => p.PeriodoInicio)
                .Custom((value, context) =>
                {
                    var m = context.InstanceToValidate;
                    
                    if (m.AnioInicio.HasValue && m.MesInicio.HasValue)
                    {
                        int PeriodoInicioNum = int.Parse($"{m.AnioInicio}{string.Format("{0:00}", m.MesInicio)}");
                        int PeriodoActualNum = int.Parse($"{DateTime.Today.Year}{string.Format("{0:00}", DateTime.Today.Month)}");

                        if (PeriodoInicioNum > PeriodoActualNum)
                        {
                            context.AddFailure("PeriodoInicio", $"El Período de inicio no puede ser mayor al período actual.");
                            return;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(value) &&!string.IsNullOrWhiteSpace(m.PeriodoBase))
                    {
                        int periodoBaseNum = int.Parse( $"{m.AnioBase}{string.Format("{0:00}", m.MesBase)}");
                        int periodoInicioNum = int.Parse($"{m.AnioInicio}{string.Format("{0:00}", m.MesInicio)}");
                        if (periodoInicioNum < periodoBaseNum)
                        {
                            context.AddFailure("PeriodoInicio", $"El Período de Inicio no puede ser anterior al Período Base.");
                        }
                    }



                });

             RuleFor(p => p.RepresentanteTecnico)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.IdFile)
            .NotEmpty()
            .WithMessage("El campo es requerido");


        }
     
    }
  
}