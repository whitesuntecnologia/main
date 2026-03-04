using DataTransferObject;
using FluentValidation;

namespace Website.Models.Formulario
{
    public class SolicitudObraParaLicitarModel
    {
        public string? NombreObra { get; set; }
        public DateTime? FechaLicitacion { get; set; }
        public string? NroExpediente { get; set; }
        public FileDTO? File { get; set; }
    }

    public class SolicitudObraParaLicitarModelValidator : AbstractValidator<SolicitudObraParaLicitarModel>
    {
        public SolicitudObraParaLicitarModelValidator()
        {
            RuleFor(p => p.NombreObra)
              .NotEmpty()
              .WithMessage("El campo es requerido");
         
            RuleFor(p => p.FechaLicitacion)
              .NotEmpty()
              .WithMessage("El campo es requerido");

            RuleFor(p => p.NroExpediente)
              .NotEmpty()
              .WithMessage("El campo es requerido");

            RuleFor(p => p.File)
              .NotEmpty()
              .WithMessage("El campo es requerido");
        }
    }

}