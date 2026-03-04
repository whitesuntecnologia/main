using System.ComponentModel.DataAnnotations;

namespace Website.Models.Validators
{

    [AttributeUsage(AttributeTargets.Property)]
    public class ValidateFechaMayorAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            DateTime? fecha = Convert.ToDateTime(value);
            
            if (!fecha.HasValue)
            {
                return ValidationResult.Success!;
            }

            if (fecha.Value < DateTime.Now)
            {
                return new ValidationResult( "La fecha no puede ser inferior a la actual.", new[] { validationContext.MemberName! });
            }

            return ValidationResult.Success!;
        }
    }
}
