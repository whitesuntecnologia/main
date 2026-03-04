using DataTransferObject;
using FluentValidation;
using StaticClass;
using System.ComponentModel.DataAnnotations;
using Website.Models.Validators;

namespace Website.Models.Formulario
{
    public class RepresentanteTecnicoModel
    {
        public int? IdRepresentanteTecnico { get; set; }
        public int? IdGrupoTramite { get; set; }
        public List<int> Especialidades { get; set; } = new();
        [Required(ErrorMessage = "El campo es requerido")]
        public string Apellido { get; set; } = "";
        [Required(ErrorMessage = "El campo es requerido")]
        public string Nombres { get; set; } = "";
        [Required(ErrorMessage = "El campo es requerido")]
        [ValidateCUITAttribute]
        //[RegularExpression(@"^\d{11}$", ErrorMessage = "El C.U.I.T. debe contener 11 dígitos")]
        public decimal? CUIT { get; set; }
        [Required(ErrorMessage = "El campo es requerido")]
        public string Cargo { get; set; } = "";
        [Required(ErrorMessage = "El campo es requerido")]
        public string Matricula { get; set; } = "";
        [Required(ErrorMessage = "El campo es requerido")]
        public DateTime? FechaVencimientoMatricula { get; set; }
        [Required(ErrorMessage = "El campo es requerido")]
        public DateTime? FechaVencimientoContrato { get; set; }
        public int? IdFileContrato { get; set; }
        public string? FilenameContrato { get; set; }
        public int? IdFileBoleta { get; set; }
        public string? FilenameBoleta{ get; set; }
        public int? IdFileMatricula { get; set; }
        public string? FilenameMatricula { get; set; }
        public int? IdFileCurriculum { get; set; }
        public string? FilenameCurriculum { get; set; }
        public List<int> Jurisdicciones { get; set; } = new();
    }
    public class RepresentanteTecnicoModelValidator : AbstractValidator<RepresentanteTecnicoModel>
    {
        public RepresentanteTecnicoModelValidator()
        {

            RuleFor(p => p.Especialidades)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.Matricula)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.Matricula)
            .MinimumLength(6)
            .WithMessage("El campo debe tener como mínimo 6 caracteres.");


            //RuleFor(model => model.Matricula)
            //  .Custom((matricula, context) =>
            //  {
            //      matricula = new string(matricula.Where(c => (char.IsDigit(c))).ToArray());
            //      if (!string.IsNullOrWhiteSpace(matricula))
            //      {
            //          decimal mat = Convert.ToDecimal(matricula);
            //          if (mat <= 0)
            //          {
            //              context.AddFailure("Matricula", $"El valor de la matrícula debe ser mayor a cero.");
            //          }
            //      }
            //  });


            RuleFor(p => p.Jurisdicciones)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.IdFileContrato)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.IdFileBoleta)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.IdFileMatricula)
            .NotEmpty()
            .WithMessage("El campo es requerido");

            RuleFor(p => p.FechaVencimientoMatricula)
                .GreaterThan(DateTime.Now)
                .WithMessage("Las fecha debe ser mayor a la fecha actual");


            RuleFor(p => p.FechaVencimientoContrato)
                .GreaterThan(DateTime.Now)
                .WithMessage("Las fecha debe ser mayor a la fecha actual");


            RuleFor(p => p.IdFileCurriculum)
            .NotEmpty()
            .When(x=> x.IdGrupoTramite == Constants.GruposDeTramite.RegistroConsultores)
            .WithMessage("El campo es requerido");
        }
    }
}
