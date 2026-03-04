using FluentValidation;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Website.Models.Formulario;

namespace Website.Models.ModuloObras
{
    public partial class ObrasPciaLPAddModel
    {
        public int? IdObraPciaLp { get; set; }
        public string Expediente { get; set; } = null!;
        public string ObraNombre { get; set; } = null!;
        public string? EstadoObra { get; set; }
        public int? PlazoObra { get; set; }
        public decimal? MontoObra { get; set; }
        public DateTime? FechaFinObra { get; set; }
        public string? Empresa { get; set; }
        public decimal? CuitEmpresa { get; set; }
        public bool EsUte { get; set; }
        public string? Licitante { get; set; }
        public decimal? PorcentajeParticipacion { get; set; }
        public int? AnioAvanceObra { get; set; }
        public int? MesAvanceObra { get; set; }
        public decimal? PorcentajeAvanceObra { get; set; }
        public bool? EsAltaPorProceso { get; set; }
        public bool EsAltaPorUsuario { get; set; }
        public bool BajaLogica { get; set; }
        public decimal? CoeficienteConceptual { get; set; }
        public DateTime? FechaInformeCoeficiente { get; set; }
        public int? IdFileInformeCoeficiente { get; set; }
        public string? FilenameInformeCoeficiente { get; set; }
    }

    public class ObrasPciaLPAddModelValidator : AbstractValidator<ObrasPciaLPAddModel>
    {
        public ObrasPciaLPAddModelValidator()
        {

            RuleFor(p => p.ObraNombre)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.EstadoObra)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.PlazoObra)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.PlazoObra)
                .GreaterThan(0)
                .WithMessage("El plazo debe ser mayor a 0.");

            RuleFor(p => p.MontoObra)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.MontoObra)
            .GreaterThan(0)
            .WithMessage("El plazo debe ser mayor a 0.");



            RuleFor(p => p.PorcentajeParticipacion)
             .NotEmpty()
             .When(x => x.EsUte)
             .WithMessage("El campo es requerido cuando es una UTE.");

            RuleFor(p => p.PorcentajeParticipacion)
                .GreaterThan(0)
                .When(x => x.EsUte)
                .WithMessage("El porcentaje debe ser mayor a 0.");

            RuleFor(model => model.CuitEmpresa)
              .Custom((value, context) =>
              {
                  
                  string? cuit = Convert.ToString(value);

                  if (!string.IsNullOrEmpty(cuit) && cuit != "0" && cuit?.Length != 11)
                  {
                      context.AddFailure("CuitEmpresa", $"El Cuit es inválido, debe tener 11 digitos.");
                      return;
                  }

                  if (!string.IsNullOrEmpty(cuit) && cuit != "0")
                  {
                      int verificador;
                      int resultado = 0;
                      string cuit_nro = cuit?.Replace("-", string.Empty) ?? string.Empty;
                      string codes = "6789456789";
                      long cuit_long = 0;

                      if (long.TryParse(cuit_nro, out cuit_long))
                      {
                          verificador = int.Parse(cuit_nro[cuit_nro.Length - 1].ToString());
                          int x = 0;

                          while (x < 10)
                          {
                              int digitoValidador = int.Parse(codes.Substring((x), 1));
                              int digito = int.Parse(cuit_nro.Substring((x), 1));
                              int digitoValidacion = digitoValidador * digito;
                              resultado += digitoValidacion;
                              x++;
                          }

                          resultado = resultado % 11;

                          if (resultado != verificador)
                          {
                              context.AddFailure("CuitEmpresa", $"El Cuit es inválido.");
                          }
                      }
                  }

              });


            RuleFor(p => p.Empresa)
                .NotEmpty()
                .When(x=> x.CuitEmpresa.HasValue && x.CuitEmpresa.GetValueOrDefault() > 0)
                .WithMessage("El campo es requerido");

            RuleFor(p => p.CoeficienteConceptual)
                .InclusiveBetween(0m,1.2m)
                .When(x => x.CoeficienteConceptual.HasValue)
                .WithMessage("El Coeficiente conceptual debe ser un valor entre 0 y 1.2.");


            RuleFor(p => p.FechaInformeCoeficiente)
                .NotEmpty()
                .When(x => x.CoeficienteConceptual.HasValue)
                .WithMessage("El campo es requerido.");


            RuleFor(p => p.IdFileInformeCoeficiente)
                .NotEmpty()
                .When(x => x.CoeficienteConceptual.HasValue)
                .WithMessage("El archivo del informe es requerido.");


            RuleFor(p => p.Licitante)
                .NotEmpty()
                .WithMessage("El campo es requerido");


        }
    }
}
