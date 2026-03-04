using FluentValidation;

namespace Website.Models.Formulario
{
    public class BoletaPagoModel
    {
        public int? NroBoleta { get; set; }
        public int? IdFileBoleta { get; set; }
        public string? FilenameBoleta { get; set; }
        public int? IdFileCumplimientoFiscal { get; set; }
        public string? FilenameCumplimientoFiscal { get; set; }

    }
    public class BoletaPagoModelValidator : AbstractValidator<BoletaPagoModel>
    {
        public BoletaPagoModelValidator()
        {
            RuleFor(p => p.NroBoleta)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.IdFileBoleta)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.IdFileCumplimientoFiscal)
                .NotEmpty()
                .WithMessage("El campo es requerido");

        }
    }

}


