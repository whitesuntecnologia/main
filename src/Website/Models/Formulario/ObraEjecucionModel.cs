using FluentValidation;
using Microsoft.VisualBasic;
using System.Globalization;
using Website.Models.Validators;

namespace Website.Models.Formulario
{
    public class ObraEjecucionModel
    {
        public int? IdTramiteObraEjec { get; set; }
        public int IdTramite { get; set; }
        public string? Expediente { get; set; } = null!;
        public int? IdObraPciaLp { get; set; }
        public string? Comitente { get; set; } = null!;
        public int? IdTipoObra { get; set; }
        public string? Ubicacion { get; set; } = null!;
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
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaCertificacion { get; set; }
        public decimal? MontoMensual
        {
            get
            {
                decimal? result = 0;
                // Convierte los dias en meses
                decimal plazoTranscurrido = (this.PlazoObra.GetValueOrDefault() + this.PlazoAmpliacion.GetValueOrDefault()) / 30m;
                if (this.TotalContratado.HasValue && plazoTranscurrido != 0)
                    result = this.TotalContratado.Value / plazoTranscurrido ;
                return result;
            }
        }
        public decimal? MontoAnual 
        { 
            get
            {
                decimal? result = 0;
                if (this.MontoMensual.HasValue)
                {
                    result = this.MontoMensual * 12;
                }
                    
                return result;
            }
        }
        public decimal? TotalContratado { get; set; }
        public decimal? TotalCertificado { get; set; }
        public decimal? Saldo 
        { 
            get
            {
                decimal? result = null;
                if (this.TotalContratado.HasValue && this.TotalCertificado.HasValue)
                    result = this.TotalContratado.Value - this.TotalCertificado.Value;
                return result;
            }
        }
        public decimal? PorcentajeCertificado 
        { 
            get
            {
                decimal? result = null;
                if (this.TotalContratado.HasValue && this.TotalCertificado.HasValue && this.TotalContratado.Value != 0)
                    result = this.TotalCertificado.Value / this.TotalContratado.Value * 100;
                return result;
            }
        }
        public int? PlazoTranscurrido 
        { 
            get
            {
                int? result = null;
                if (this.FechaInicio.HasValue && this.FechaCertificacion.HasValue)
                {
                    TimeSpan diferencia = FechaCertificacion.Value - FechaInicio.Value;
                    result = Convert.ToInt32(diferencia.TotalDays + 1 );    // + 1 para que se tome en consideracion el dia actual o el ultimo dia.
                }
                return result;
            }
        }
        public int? IdFile { get; set; }
        public string? Filename { get; set; } = null!;
        public int? PlazoObra { get; set; } = null!;
        public int? PlazoAmpliacion { get; set; } = null!;

    }
    public class ObraEjecucionModelValidator : AbstractValidator<ObraEjecucionModel>
    {
        public ObraEjecucionModelValidator()
        {

            RuleFor(p => p.Expediente)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.IdObraPciaLp)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.Comitente)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.IdTipoObra)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.Ubicacion)
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
            .InclusiveBetween(1950, 2050)
            .When(x => x.AnioBase.HasValue)
            .WithMessage($"El año ingresado en inválido");

            RuleFor(p => p.FechaInicio)
            .NotEmpty()
            .WithMessage("El campo es requerido")
            .When(x=> !string.IsNullOrWhiteSpace(x.PeriodoBase) && x.PeriodoBase.Length == 7)
            .GreaterThanOrEqualTo(x=> DateTime.ParseExact("01/" + x.PeriodoBase,"dd/MM/yyyy", CultureInfo.InvariantCulture))
            .WithMessage("La fecha de inicio no puede ser menor al período base.")
            ;
            


            RuleFor(p => p.MontoMensual)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.MontoMensual)
            .GreaterThan(0)
            .WithMessage("El valor debe ser mayor a 0.");

            RuleFor(p => p.MontoAnual)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.MontoAnual)
            .GreaterThan(0)
            .WithMessage("El valor debe ser mayor a 0.");

            RuleFor(p => p.TotalContratado)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            
            RuleFor(p => p.TotalCertificado)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.TotalCertificado)
            .GreaterThan(0)
            .WithMessage("El valor debe ser mayor a 0.");

            RuleFor(p => p.Saldo)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.Saldo)
            .GreaterThan(0)
            .WithMessage("El valor debe ser mayor a 0.");

            RuleFor(p => p.PorcentajeCertificado)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.PlazoTranscurrido)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.PlazoTranscurrido)
            .GreaterThan(0)
            .WithMessage("El valor debe ser mayor a 0.");

            RuleFor(p => p.IdFile)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.PlazoObra)
            .NotEmpty()
            .WithMessage("El campo es requerido");
        }
    }
}
