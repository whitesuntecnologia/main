using FluentValidation;

namespace Website.Models.Formulario
{
    public class EquiposAddModel
    {
        public EquiposItemModel Afectado { get; set; } = new();
        public EquiposItemModel NoAfectado { get; set; } = new();

    }
    public class EquiposItemModel
    {
        public int? IdTramiteEquipo { get; set; }
        public int? IdTramite { get; set; }
        public bool Afectado { get; set; }
        public int? IdFileDetalleEquipo { get; set; }
        public string? FilenameDetalleEquipo { get; set; }
        public int? IdFileCertificadoContable { get; set; }
        public string? FilenameCertificadoContable { get; set; }
        public int? IdFileDocumentacionEquipo { get; set; }
        public string? FilenameDocumentacionEquipo { get; set; }
        public decimal? MontoRealizacion { get; set; }
        public decimal? MontoRealizacionEvaluador { get; set; }
        public bool isEvaluandoTramite { get; set; }
    }
    public class EquiposAddModelValidator : AbstractValidator<EquiposAddModel>
    {
        public EquiposAddModelValidator()
        {

            RuleFor(p => p.Afectado.IdFileDetalleEquipo)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.Afectado.IdFileCertificadoContable)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.Afectado.MontoRealizacion)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.NoAfectado.MontoRealizacion)
               .NotEmpty()
               .When(x => x.NoAfectado.MontoRealizacion > 0 || x.NoAfectado.IdFileDetalleEquipo > 0 || x.NoAfectado.IdFileCertificadoContable > 0)
               .WithMessage("El campo es requerido");

            When(x => x.Afectado.isEvaluandoTramite, () =>
            {
                RuleFor(p => p.Afectado.MontoRealizacionEvaluador)
                    .NotNull() // usalo si es nullable; si no, podés obviarlo
                    .GreaterThan(0m).WithMessage("El monto debe ser mayor a 0")
                    .LessThanOrEqualTo(p => p.Afectado.MontoRealizacion)
                    .WithMessage("El monto debe ser menor o igual al ingresado por la empresa");
            });


            RuleFor(p => p.NoAfectado.IdFileDetalleEquipo)
                .NotEmpty()
                .When(x => x.NoAfectado.MontoRealizacion > 0 || x.NoAfectado.IdFileDetalleEquipo > 0 || x.NoAfectado.IdFileCertificadoContable > 0)
                .WithMessage("El campo es requerido");

            RuleFor(p => p.NoAfectado.IdFileCertificadoContable)
                .NotEmpty()
                .When(x => x.NoAfectado.MontoRealizacion > 0 || x.NoAfectado.IdFileDetalleEquipo > 0 || x.NoAfectado.IdFileCertificadoContable > 0)
                .WithMessage("El campo es requerido");

            When(x => x.NoAfectado.isEvaluandoTramite && x.NoAfectado.MontoRealizacion > 0, () =>
            {
                RuleFor(p => p.NoAfectado.MontoRealizacionEvaluador)
                    .NotNull()
                    .GreaterThan(0m).WithMessage("El monto debe ser mayor a 0")
                    .LessThanOrEqualTo(p => p.NoAfectado.MontoRealizacion)
                    .WithMessage("El monto debe ser menor o igual al ingresado por la empresa");
            });

        }
    }
}
