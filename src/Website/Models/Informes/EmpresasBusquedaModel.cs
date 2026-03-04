using DataTransferObject.BLs;
using FluentValidation;

namespace Website.Models.Informes
{
    public class EmpresasBusquedaModel
    {
        public decimal? Cuit { get;set; }
        public string? RazonSocial { get; set; }
        public DateTime? FechaRegistroDesde { get;set; }
        public DateTime? FechaRegistroHasta { get; set; }
        public DateTime? FechaVencimientoDesde { get; set; }
        public DateTime? FechaVencimientoHasta { get; set; }
        public int TiposEmpresasSelected { get; set; } = 1;
        public IEnumerable<int> EspecialidadesSelected { get; set; } = null!;
        public decimal? CapacidadTecnicaDesde { get; set; }
        public decimal? CapacidadTecnicaHasta { get; set; }
        public decimal? CapacidadEconomicaDesde { get; set; }
        public decimal? CapacidadEconomicaHasta { get; set; }
    }
    public class EmpresasBusquedaModelValidator : AbstractValidator<EmpresasBusquedaModel>
    {
        public EmpresasBusquedaModelValidator()
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
