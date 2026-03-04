using System.ComponentModel.DataAnnotations;

namespace Website.Models.Validators
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidateCUITAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            string? cuit = Convert.ToString(value);

            if (string.IsNullOrEmpty(cuit))
            {
                return ValidationResult.Success!;
            }

            if (cuit.Length != 11)
            {
                return new ValidationResult("El Cuit es inválido, debe tener 11 digitos", new[] { validationContext.MemberName!});
            }

            int verificador;
            int resultado = 0;
            string cuit_nro = cuit.Replace("-", string.Empty);
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
                    return new ValidationResult("El Cuit es inválido", new[] { validationContext.MemberName! });
                }
            }

            return ValidationResult.Success!;
        }
    }
}
