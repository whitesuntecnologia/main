using DataTransferObject;
using FluentValidation;
using Website.Models.Formulario;
using Website.Models.Validators;

namespace Website.Models.Account
{
    public class SolicitudUsuarioModel
    {
        public string? ApellidoyNombresSolicitante { get; set; }
        [ValidateCUITAttribute]
        public long? CuitSolicitante { get; set; }
        public string? RazonSocial { get; set; }
        [ValidateCUITAttribute]
        public long? CuitEmpresa { get; set; }
        public FileDTO? File1 { get; set; }
        public FileDTO? File2 { get; set; }
        public FileDTO? File3 { get; set; }
    }
    public class SolicitudUsuarioModelValidator : AbstractValidator<SolicitudUsuarioModel>
    {
        public SolicitudUsuarioModelValidator()
        {

            RuleFor(p => p.ApellidoyNombresSolicitante)
                .NotEmpty()
                .WithMessage("El campo es requerido");
            
            RuleFor(p => p.CuitSolicitante)
                .NotEmpty()
                .WithMessage("El campo es requerido");


            RuleFor(p => p.RazonSocial)
                .NotEmpty()
                .WithMessage("El campo es requerido");
            
            RuleFor(p => p.CuitEmpresa)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.File1)
                .NotNull()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.File2)
                .NotNull()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.File3)
                .NotNull()
                .WithMessage("El campo es requerido");



        }
    }
}
