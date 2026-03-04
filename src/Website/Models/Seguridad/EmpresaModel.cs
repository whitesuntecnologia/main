using FluentValidation;
using StaticClass;

namespace Website.Models.Seguridad
{
    public class EmpresaModel
    {
        public int? IdEmpresa { get; set; }

        public decimal? CuitEmpresa { get; set; }
        public string? RazonSocial { get; set; } = null!;
        public string? Domicilio { get; set; } = null!;
        public DateTime? Vencimiento { get; set; }
        public string? UseridRepresentante { get; set; }
        public DateTime? FechaVigenciaDesde { get; set; }
        public DateTime? FechaVigenciaHasta { get; set; }
        public List<EmpresaSancionModel> Sanciones { get; set; } = new();

    }
    public class EmpresaModelValidator : AbstractValidator<EmpresaModel>
    {
        public EmpresaModelValidator()
        {

            RuleFor(p => p.CuitEmpresa)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.RazonSocial)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.UseridRepresentante)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.FechaVigenciaDesde)
                .NotEmpty()
                .WithMessage("El campo Año es requerido");

            RuleFor(p => p.FechaVigenciaDesde)
            .LessThan(x => x.FechaVigenciaHasta)
            .When(x => x.FechaVigenciaHasta.HasValue)
            .WithMessage("La fecha desde no puede ser mayor a la fecha hasta.");


            RuleFor(model => model.CuitEmpresa)
              .Custom((cuit, context) =>
              {

                  if (cuit.HasValue)
                  {
                      if (cuit?.ToString().Length < 11)
                      {
                          context.AddFailure("CuitEmpresa", $"El CUIT debe tener 11 dígitos.");
                      }
                      else if (!Functions.ValidateCUIT(cuit.ToString()))
                      {
                          context.AddFailure("CuitEmpresa", $"El CUIT es inválido.");
                      }
                  }
              })
              .When(x => x.CuitEmpresa.HasValue)
              ;

            //RuleFor(model => model.CuitRepresentante)
            //  .Custom((cuit, context) =>
            //  {
            //      if (cuit.HasValue)
            //      {
            //          if (cuit?.ToString().Length < 11)
            //          {
            //              context.AddFailure("CuitRepresentante", $"El CUIT debe tener 11 dígitos.");
            //          }
            //          else if (!Functions.ValidateCUIT(cuit.ToString()))
            //          {
            //              context.AddFailure("CuitRepresentante", $"El CUIT es inválido.");
            //          }
            //      }
            //  })
            //  .When(x => x.CuitRepresentante.HasValue)
            //  ;


        }
    }
}
