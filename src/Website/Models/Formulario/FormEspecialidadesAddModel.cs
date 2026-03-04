using FluentValidation;
using StaticClass;

namespace Website.Models.Formulario
{
    public class FormEspecialidadesAddModel
    {
        public int IdGrupoTramite { get; set; }
        public  int IdEspecialidadSelected { get; set; }
        public int IdSeccionSelected { get; set; }
        public IEnumerable<int> IdTareasSeleccted = new List<int>();
        
    }
    public class FormEspecialidadesAddModelValidator : AbstractValidator<FormEspecialidadesAddModel>
    {
        public FormEspecialidadesAddModelValidator()
        {

            RuleFor(p => p.IdEspecialidadSelected)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.IdSeccionSelected)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.IdTareasSeleccted)
                .NotEmpty()
                .When(x=> x.IdGrupoTramite != Constants.GruposDeTramite.RegistroConsultores)
                .WithMessage("Debe seleccionar al menos 1 tarea.");

        }
    }
}
