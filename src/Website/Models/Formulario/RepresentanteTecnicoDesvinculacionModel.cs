using FluentValidation;
using Website.Models.Seguridad;

namespace Website.Models.Formulario
{
    public class RepresentanteTecnicoDesvinculacionModel
    {
        public int IdRepresentanteTecnico { get; set; }
        public int? IdFileDesvinculacion { get; set; }
        public string FilenameDesvinculacion { get; set; } = null!;
    }

    public class RepresentanteTecnicoDesvinculacionModelValidator : AbstractValidator<RepresentanteTecnicoDesvinculacionModel>
    {
        public RepresentanteTecnicoDesvinculacionModelValidator()
        {
            
            RuleFor(x => x.IdFileDesvinculacion)
                .NotEmpty().WithMessage("Debe cargar el archivo con la nota de renuncia firmada por el representante técnico")
                .GreaterThan(0).WithMessage("Debe cargar el archivo con la nota de renuncia firmada por el representante técnico");
        }
    }
}
