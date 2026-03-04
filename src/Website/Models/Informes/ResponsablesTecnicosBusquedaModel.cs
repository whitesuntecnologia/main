using FluentValidation;

namespace Website.Models.Informes
{
    public class ResponsablesTecnicosBusquedaModel
    {
        public string? Apellido { get; set; }
        public string? Nombres { get; set; }
        public decimal? Cuit { get; set; }
        public string? RazonSocial { get; set; }
        public DateTime? FechaVencimientoMatriculaDesde { get; set; }
        public DateTime? FechaVencimientoMatriculaHasta { get; set; }
        public DateTime? FechaVencimientoContratoDesde { get; set; }
        public DateTime? FechaVencimientoContratoHasta { get; set; }
    }

    public class ResponsablesTecnicosBusquedaModelValidator : AbstractValidator<ResponsablesTecnicosBusquedaModel>
    {
        public ResponsablesTecnicosBusquedaModelValidator()
        {

            RuleFor(model => model.Cuit)
              .Custom((value, context) =>
              {

                  string? cuit = Convert.ToString(value);

                  if (string.IsNullOrEmpty(cuit))
                  {
                      //puede ser nulo
                      return;
                  }

                  if (cuit?.Length != 11)
                  {
                      context.AddFailure("Cuit", $"El Cuit es inválido, debe tener 11 digitos.");
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
                          context.AddFailure("Cuit", $"El Cuit es inválido.");
                      }
                  }

              });
        }
    }
}
