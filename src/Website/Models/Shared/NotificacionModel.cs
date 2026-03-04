using FluentValidation;
using Website.Models.Formulario;

namespace Website.Models.Shared
{
    public class NotificacionModel
    {
        public int? IdTramite { get; set; }
        public string? Titulo { get; set; }
        public string? Mensaje { get; set; }
       
    }

    public class NotificacionModelValidator : AbstractValidator<NotificacionModel>
    {
        public NotificacionModelValidator()
        {
            RuleFor(p => p.IdTramite)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.Titulo)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.Mensaje)
                .NotEmpty()
                .WithMessage("El campo es requerido");
        }
    }
}