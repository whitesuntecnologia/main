using FluentValidation;
using Microsoft.Build.Framework;

namespace Website.Models.Formulario
{
    public class InfEmpresaDocumentoAddModel
    {
        public int? IdTipoDocumento { get; set; } = null!;
        public int? IdFile { get; set; }
        public string? Filename { get; set; }
    }
    public class InfEmpresaDocumentoAddModelValidator : AbstractValidator<InfEmpresaDocumentoAddModel>
    {
        public InfEmpresaDocumentoAddModelValidator()
        {

            RuleFor(p => p.IdTipoDocumento)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.IdFile)
                .NotEmpty()
                .WithMessage("El campo es requerido");
        }
    }
}
