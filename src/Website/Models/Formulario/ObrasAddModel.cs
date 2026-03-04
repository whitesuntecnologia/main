using FluentValidation;
using Website.Models.Validators;

namespace Website.Models.Formulario
{
    public partial class ObrasAddModel
    {
        public int? IdObraPciaLp { get; set; }
        public string ObraNombre { get; set; } = null!;
        public string? EstadoObra { get; set; }
        public int? PlazoObra { get; set; }
        public decimal? MontoObra { get; set; }
        public DateTime? FechaFinObra { get; set; }
        public string? Empresa { get; set; }
        public decimal? CuitEmpresa { get; set; }
        public bool EsUte { get; set; }
        public decimal? PorcentajeParticipacion { get; set; }
    }
    public class ObrasAddModelValidator : AbstractValidator<ObrasAddModel>
    {
        public ObrasAddModelValidator()
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


            RuleFor(p => p.Empresa)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.PorcentajeParticipacion)
                .NotEmpty()
                .When(x=> x.EsUte)
                .WithMessage("El campo es requerido cuando es una UTE.");

            RuleFor(p => p.PorcentajeParticipacion)
                .GreaterThan(0)
                .When(x => x.EsUte)
                .WithMessage("El porcentaje debe ser mayor a 0.");

            
            RuleFor(model => model.CuitEmpresa)
              .Custom((value, context) =>
              {

                  string? cuit = Convert.ToString(value);

                  if (string.IsNullOrEmpty(cuit))
                  {
                      context.AddFailure("CuitEmpresa", $"El campo es requerido.");
                      return;
                  }

                  if (cuit?.Length != 11)
                  {
                      context.AddFailure("CuitEmpresa", $"El Cuit es inválido, debe tener 11 digitos.");
                      return;
                  }

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

              });

        }
    }
}
