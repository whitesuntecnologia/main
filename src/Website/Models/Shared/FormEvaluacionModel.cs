using FluentValidation;
using StaticClass;
using Website.Models.Seguridad;

namespace Website.Models.Shared
{
    public class FormEvaluacionModel
    {
        public int? IdTramiteFormEvaluado { get; set; }
        public int? IdTramite { get; set; }
        public int? NroNotificacion { get; set; }
        public int? NroFormulario { get; set; }

        private int? _IdEstadoEvaluacion;
        public int? IdEstadoEvaluacion {
            get
            {
                return _IdEstadoEvaluacion;
            }
            set
            {
                _IdEstadoEvaluacion = value;
                if (_IdEstadoEvaluacion.GetValueOrDefault() != (int) Constants.EstadosEvaluacion.Notificar)
                    MensajeNotificacion = null;
            }
        }
        public string? MensajeNotificacion { get; set; }
    }
    public class FormEvaluacionModelValidator : AbstractValidator<FormEvaluacionModel>
    {
        public FormEvaluacionModelValidator()
        {

            RuleFor(p => p.IdTramite)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.NroFormulario)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.IdEstadoEvaluacion)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.MensajeNotificacion)
                .NotEmpty()
                .When(x=> x.IdEstadoEvaluacion == (int) Constants.EstadosEvaluacion.Notificar)
                .WithMessage("El campo es requerido");

        }
    }
}
