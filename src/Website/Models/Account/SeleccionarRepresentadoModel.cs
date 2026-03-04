using FluentValidation;

namespace Website.Models.Account
{
    public class SeleccionarRepresentadoModel
    {
        public string? userid { get; set; }
        public int? IdEmpresa { get; set; }
    }
    public class SeleccionarRepresentadoModelValidator : AbstractValidator<SeleccionarRepresentadoModel>
    {
        public SeleccionarRepresentadoModelValidator()
        {


            RuleFor(p => p.IdEmpresa)
                .NotEmpty()
                .WithMessage("El campo es requerido");


        }
    }
}