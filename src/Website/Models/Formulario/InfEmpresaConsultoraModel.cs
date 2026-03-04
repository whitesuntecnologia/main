using CsvHelper;
using FluentValidation;
using StaticClass;

namespace Website.Models.Formulario
{
    public class InfEmpresaConsultoraModel
    {
        public int IdTramiteInfEmpCon { get; set; }
        public int IdTramite { get; set; }
        public int TipoEmpresa { get; set; }
        public string? RazonSocial { get; set; }
        public int? IdTipoSociedad { get; set; }
        public int? IdFileContrato { get; set; }
        public string? FilenameContrato { get; set; }
        public int? IdFileEstatuto { get; set; }
        public string? FilenameEstatuto { get; set; }
        public string? DeRegComercio { get; set; }
        public DateTime? FechaRegComercio { get; set; }
        public string? LibroRegComercio { get; set; }
        public string? TomoRegComercio { get; set; }
        public string? FolioRegComercio { get; set; }
        public int? IdFileRegComercio { get; set; }
        public string? FilenameRegComercio { get; set; }
        public int? AniosDuracionSoc { get; set; }
        public DateTime? FechaConstitucionSoc { get; set; }
        public DateTime? FechaVencimientoSoc { get; set; }
        public string? ProrrogaDePlazoSoc { get; set; }
        public int? IdFileActaDesignacion { get; set; }
        public string? FilenameActaDesignacion { get; set; }

        //Como contribuyente de
        public string? Cuit { get; set; }
        public string? NroIibb { get; set; }
        public DateTime? FechaInscripcionIibb { get; set; }
        public int TipoIibb { get; set; }
        public int TipoIva { get; set; }
        public DateTime? FechaInscripcionIva { get; set; }
        public bool EsInscriptoGan { get; set; }
        public DateTime? FechaInscripcionGan { get; set; }
        public string? OtrosImpuestos { get; set; }
        public int? IdFileConstanciaBcra { get; set; }
        public string? FilenameConstanciaBcra { get; set; }
        public List<InfEmpresaConsultoraPersonaModel> Personas { get; set; } = new();
        public List<InfEmpresaConsultoraDocumentoModel> Documentos { get; set; } = new();
        public List<InfEmpresaConsultoraDeudaModel> Deudas { get; set; } = new();

    }
    public class InfEmpresaConsultoraPersonaModel
    {
        public int IdPersona { get; set; }
        public int IdTramiteInfEmpCon { get; set; }
        public string? Apellidos { get; set; }
        public string? Nombres { get; set; }
        public int? IdTipoCaracterLegal { get; set; }
        public string? DescripcionTipoCaracterLegal { get; set; }
        public int? NroDni { get; set; }
    }
    public class InfEmpresaConsultoraDeudaModel
    {
        public int? IdTramiteInfEmpDeuda { get; set; }
        public string? Entidad { get; set; } = null!;

        public int? Mes { get; set; } = null!;
        public int? Anio { get; set; } = null!;
        public string? Periodo
        {
            get
            {
                string strMes = (Mes.HasValue ? Mes.Value.ToString("00") : "");
                string strAnio = (Anio.HasValue ? Anio.Value.ToString() : "");
                return strMes + "/" + strAnio;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var partes = value.Split('/');
                    if (partes.Length == 2)
                    {
                        if (int.TryParse(partes[0], out int mes))
                            Mes = mes;
                        if (int.TryParse(partes[1], out int anio))
                            Anio = anio;
                    }
                }
                else
                {
                    Mes = null;
                    Anio = null;
                }
            }
        }

        public int? Situacion { get; set; }

        public decimal? Monto { get; set; }

        public int? DiasDeAtraso { get; set; }
    }
    public class InfEmpresaConsultoraDocumentoModel
    {
        public int IdTramiteInfEmpConsDocumento { get; set; }
        public int IdTramiteInfEmpCons { get; set; }
        public int? IdTipoDocumento { get; set; } = null!;
        public string Descripcion { get; set; } = null!;

        public int? TamanioMaximoMb { get; set; }

        public string Extension { get; set; } = null!;

        public string? AcronimoGde { get; set; }

        public bool Obligatorio { get; set; }
        public bool SePermiteVariasVeces { get; set; }
        public int? IdFile { get; set; }
        public string? Filename { get; set; }

        //propiedades solo para leer en las grillas
        public long? Size { get; set; }
        public string? SizeStr
        {
            get
            {
                string? result = null;
                if (this.Size.HasValue)
                {
                    decimal kb = this.Size.Value / 1024.0m;
                    decimal mb = kb / 1024.0m;
                    result = (mb < 1 ? Math.Ceiling(kb).ToString("N0") + " Kb." : mb.ToString("N2") + " Mb.");
                }
                return result;
            }
        }
    }
    public class InfEmpConsDocumentoAddModelValidator : AbstractValidator<InfEmpresaConsultoraDocumentoModel>
    {
        public InfEmpConsDocumentoAddModelValidator()
        {

            RuleFor(p => p.IdTipoDocumento)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.IdFile)
                .NotEmpty()
                .WithMessage("El campo es requerido");
        }

    }
    public class InfEmpresaConsultoraModelValidator : AbstractValidator<InfEmpresaConsultoraModel>
    {
        public InfEmpresaConsultoraModelValidator()
        {

            RuleFor(p => p.TipoEmpresa)
                .GreaterThan(0)
                .WithMessage("El campo es requerido");

            RuleFor(p => p.RazonSocial)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.IdTipoSociedad)
                .NotEmpty()
                .When(p => p.TipoEmpresa != 1)
                .WithMessage("El campo es requerido");

            RuleFor(p => p.IdFileContrato)
                .NotEmpty()
                .When(p => p.TipoEmpresa != 1)
                .WithMessage("El campo es requerido");

            RuleFor(p => p.IdFileEstatuto)
                .NotEmpty()
                .When(p => p.TipoEmpresa != 1)
                .WithMessage("El campo es requerido");

            RuleFor(p => p.DeRegComercio)
                .NotEmpty()
                .When(p => p.TipoEmpresa != 1)
                .WithMessage("El campo es requerido");

            RuleFor(p => p.FechaRegComercio)
                .NotEmpty()
                .When(p => p.TipoEmpresa != 1)
                .WithMessage("El campo es requerido")
                .LessThan(DateTime.Today)
                .When(p => p.TipoEmpresa != 1)
                .WithMessage("La fecha debe ser menor o igual a la fecha actual.");

            RuleFor(p => p.LibroRegComercio)
                .NotEmpty()
                .When(p => p.TipoEmpresa != 1)
                .WithMessage("El campo Libro es requerido");

            RuleFor(p => p.TomoRegComercio)
                .NotEmpty()
                .When(p => p.TipoEmpresa != 1)
                .WithMessage("El campo Tomo es requerido");

            RuleFor(p => p.FolioRegComercio)
                .NotEmpty()
                .When(p => p.TipoEmpresa != 1)
                .WithMessage("El campo Folio es requerido");

            RuleFor(p => p.IdFileRegComercio)
                .NotEmpty()
                .When(p => p.TipoEmpresa != 1)
                .WithMessage("El campo es requerido");

            RuleFor(p => p.AniosDuracionSoc)
                .NotEmpty()
                .When(p => p.TipoEmpresa != 1)
                .WithMessage("El campo es requerido");

            RuleFor(p => p.FechaConstitucionSoc)
                .NotEmpty()
                .When(p => p.TipoEmpresa != 1)
                .WithMessage("El campo es requerido")
                .LessThan(DateTime.Today)
                .When(p => p.TipoEmpresa != 1)
                .WithMessage("La fecha debe ser menor o igual a la fecha actual.");

            RuleFor(p => p.FechaConstitucionSoc)
                .LessThan(x => x.FechaVencimientoSoc)
                .When(x => x.FechaVencimientoSoc.HasValue && x.TipoEmpresa != 1)
                .WithMessage("La fecha debe ser menor a la fecha de vencimiento");

            RuleFor(p => p.FechaVencimientoSoc)
                .NotEmpty()
                .When(p => p.TipoEmpresa != 1)
                .WithMessage("El campo es requerido")
                .GreaterThan(DateTime.Today)
                .When(p => p.TipoEmpresa != 1)
                .WithMessage("La fecha debe ser mayor a la actual");

            RuleFor(p => p.IdFileActaDesignacion)
                .NotEmpty()
                .When(p => p.TipoEmpresa != 1)
                .WithMessage("El campo es requerido");

            RuleFor(p => p.Cuit)
                .Custom((cuit, context) =>
                {
                    if (!string.IsNullOrWhiteSpace(cuit))
                    {
                        cuit = cuit.Replace("-", "");
                        if (cuit.Length < 11)
                        {
                            context.AddFailure("Cuit", $"El CUIT debe tener 11 dígitos.");
                        }
                        else if (!Functions.ValidateCUIT(cuit.ToString()))
                        {
                            context.AddFailure("Cuit", $"El CUIT es inválido.");
                        }
                    }
                    else
                    {
                        context.AddFailure("Cuit", $"El campo es requerido.");
                    }
                });

            RuleFor(p => p.FechaInscripcionIva)
              .NotEmpty()
              .WithMessage("El campo es requerido")
              .LessThan(DateTime.Today)
              .WithMessage("La fecha debe ser menor a la actual");

            RuleFor(p => p.NroIibb)
                .NotEmpty()
                .When(p => p.TipoIibb != 0)
                .WithMessage("El campo es requerido");

            RuleFor(p => p.FechaInscripcionIibb)
                .NotEmpty()
                .When(p => p.TipoIibb != 0)
                .WithMessage("El campo es requerido")
                .LessThan(DateTime.Today)
                .When(p => p.TipoIibb != 0)
                .WithMessage("La fecha debe ser menor a la actual");

            RuleFor(p => p.TipoIibb)
                .GreaterThan(0)
                .When(p => p.TipoEmpresa != 1)
                .WithMessage("El campo es requerido");

            RuleFor(p => p.TipoIva)
                .GreaterThan(0)
                .WithMessage("El campo es requerido");

            RuleFor(p => p.IdFileConstanciaBcra)
             .NotEmpty()
             .WithMessage("El campo es requerido");
        }
    }
    public class InfEmpresaConsultoraDeudaModelValidator : AbstractValidator<InfEmpresaConsultoraDeudaModel>
    {
        public InfEmpresaConsultoraDeudaModelValidator()
        {

            RuleFor(p => p.Entidad)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.Mes)
                .NotEmpty()
                .WithMessage("El campo Mes es requerido");

            RuleFor(p => p.Anio)
                .NotEmpty()
                .WithMessage("El campo Año es requerido");


            RuleFor(p => p.Situacion)
                .NotEmpty()
                .WithMessage("El campo es requerido");


            RuleFor(p => p.Monto)
                .NotEmpty()
                .WithMessage("El campo es requerido");


            RuleFor(p => p.DiasDeAtraso)
                .NotEmpty()
                .WithMessage("El campo es requerido");


            RuleFor(model => model.Anio)
              .Custom((anio, context) =>
              {
                  var m = (InfEmpresaConsultoraDeudaModel)context.InstanceToValidate;
                  bool valida = true;
                  //Verifica que el período no sea menor a 3 meses desde la actualizad
                  //y que tampoco sea mayor a la actualidad
                  if (m.Mes.HasValue && m.Anio.HasValue)
                  {

                      var DateMinValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-3);
                      var DateMaxValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                      var PeriodoValue = new DateTime(m.Anio.Value, m.Mes.Value, 1);

                      if (PeriodoValue < DateMinValue)
                          valida = false;
                      else if (PeriodoValue > DateMaxValue)
                          valida = false;
                  }

                  if (!valida)
                  {
                      context.AddFailure("Anio", $"El período ingresado no puede ser anterior a 3 meses del período actual.");
                  }
              })
              .When(x => x.Anio.HasValue && x.Mes.HasValue);

        }
    }
}