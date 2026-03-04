using DataTransferObject;
using DocumentFormat.OpenXml.Office2021.MipLabelMetaData;
using FluentValidation;
using Microsoft.AspNetCore.Components;

namespace Website.Models.Formulario
{
    public class BienesRaicesModel 
    {
        public delegate void OnChageSinDatosFormDelegate(bool value);
        public event OnChageSinDatosFormDelegate OnChageSinDatosForm = null!;
        public BienesRaicesItemModel Afectado { get; set; } = new();
        public BienesRaicesItemModel NoAfectado { get; set; } = new();
        private bool _SinDatosForm8 { get; set; }
        public bool SinDatosForm8
        {
            get { return _SinDatosForm8; }
            set
            {
                _SinDatosForm8 = value;
                if (value)
                {
                    if (Afectado != null)
                    {
                        Afectado.MontoRealizacion = null;
                        Afectado.IdFileDetalleInmueble = null;
                        Afectado.FilenameDetalleInmueble = null;
                        Afectado.IdFileCertificadoContable = null;
                        Afectado.FilenameCertificadoContable = null;
                    }
                    //if (NoAfectado != null)
                    //{
                    //    NoAfectado.MontoRealizacion = null;
                    //    NoAfectado.IdFileDetalleInmueble = null;
                    //    NoAfectado.FilenameDetalleInmueble = null;
                    //    NoAfectado.IdFileCertificadoContable = null;
                    //    NoAfectado.FilenameCertificadoContable = null;
                    //}
                }
                if (OnChageSinDatosForm != null)
                {
                    OnChageSinDatosForm.Invoke(value);
                }

            }
        }
    }
    public class BienesRaicesItemModel
    {
        public int? IdTramiteBienRaiz { get; set; }
        public int? IdTramite { get; set; }

        public int? IdFileDetalleInmueble { get; set; }

        public string? FilenameDetalleInmueble { get; set; } = null!;

        public int? IdFileCertificadoContable { get; set; }

        public string? FilenameCertificadoContable { get; set; } = null!;

        public decimal? MontoRealizacion { get; set; }
        public decimal? MontoRealizacionEvaluador { get; set; }
        public bool isEvaluandoTramite { get; set; }
    }
    
    public class BienesRaicesModelValidator : AbstractValidator<BienesRaicesModel>
    {
        public BienesRaicesModelValidator()
        {

            RuleFor(p => p.Afectado.IdFileDetalleInmueble)
                .NotEmpty()
                .When(x=> !x.SinDatosForm8)
                .WithMessage("El campo es requerido");

            RuleFor(p => p.Afectado.IdFileCertificadoContable)
                .NotEmpty()
                .When(x => !x.SinDatosForm8)
                .WithMessage("El campo es requerido");

            RuleFor(p => p.Afectado.MontoRealizacion)
                .NotEmpty()
                .When(x => !x.SinDatosForm8)
                .WithMessage("El campo es requerido");

            RuleFor(p => p.NoAfectado.MontoRealizacion)
                .NotEmpty()
                .When(x => (x.NoAfectado.MontoRealizacion > 0 || x.NoAfectado.IdFileDetalleInmueble > 0 || x.NoAfectado.IdFileCertificadoContable > 0))
                .WithMessage("El campo es requerido");

            //RuleFor(p => p.Afectado.MontoRealizacionEvaluador)
            //   .NotEmpty()
            //   .When(x => x.Afectado.isEvaluandoTramite)
            //   .WithMessage("El monto debe ser mayor a 0")
            //   .GreaterThan(0)
            //   .When(x => x.Afectado.isEvaluandoTramite)
            //   .WithMessage("El monto debe ser mayor a 0");


            RuleFor(p => p.NoAfectado.IdFileDetalleInmueble)
                .NotEmpty()
                .When( x=> (x.NoAfectado.MontoRealizacion > 0 || x.NoAfectado.IdFileDetalleInmueble > 0 || x.NoAfectado.IdFileCertificadoContable > 0))
                .WithMessage("El campo es requerido");

            RuleFor(p => p.NoAfectado.IdFileCertificadoContable)
                .NotEmpty()
                .When(x => (x.NoAfectado.MontoRealizacion > 0 || x.NoAfectado.IdFileDetalleInmueble > 0 || x.NoAfectado.IdFileCertificadoContable > 0))
                .WithMessage("El campo es requerido");

            //RuleFor(p => p.NoAfectado.MontoRealizacionEvaluador)
            //   .NotEmpty()
            //   .When(x => x.NoAfectado.isEvaluandoTramite && x.NoAfectado.MontoRealizacion > 0)
            //   .WithMessage("El monto debe ser mayor a 0")
            //   .GreaterThan(0)
            //   .When(x => x.NoAfectado.isEvaluandoTramite && x.NoAfectado.MontoRealizacion > 0)
            //   .WithMessage("El monto debe ser mayor a 0");
        }
    }
    
}
