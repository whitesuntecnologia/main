using FluentValidation;

namespace Website.Models.Formulario
{
    public class BalanceGeneralModel
    {
        public int? IdTramiteBalanceGeneral { get; set; }

        public int IdTramite { get; set; }

        public int? Anio { get; set; }

        private DateTime? _fechaBalance { get; set; }
        public DateTime? FechaBalance {
            get { return _fechaBalance; }
            set {
                _fechaBalance = value;
                if (_fechaBalance.HasValue)
                    Anio = _fechaBalance.Value.Year;
                else
                    Anio = null;
            }
        }

        public decimal? OoficDepositosCortoPlazoEmp { get; set; }

        public decimal? OoficDepositosLargoPlazoEmp { get; set; }

        public decimal? OoficInversionesCortoPlazoEmp { get; set; }

        public decimal? OoficInversionesLargoPlazoEmp { get; set; }

        public decimal? OoficOtrosConceptosEmp { get; set; }

        public decimal? OpartDepositosCortoPlazoEmp { get; set; }

        public decimal? OpartDepositosLargoPlazoEmp { get; set; }

        public decimal? OpartInversionesCortoPlazoEmp { get; set; }

        public decimal? OpartInversionesLargoPlazoEmp { get; set; }

        public decimal? OpartInversionesEmp { get; set; }

        public decimal? OpartOtrosConceptosEmp { get; set; }

        public decimal? DispCajayBancosEmp { get; set; }

        public decimal? BusoInversionesEmp { get; set; }

        public decimal? BusoMaqUtilAfecEmp { get; set; }

        public decimal? BusoMaqUtilNoAfecEmp { get; set; }
        public decimal? BusoBienesRaicesAfecEmp { get; set; }

        public decimal? BusoBienesRaicesNoAfecEmp { get; set; }

        public decimal? BusoOtrosConceptosEmp { get; set; }

        public decimal? BcamMaterialesEmp { get; set; }

        public decimal? BcamOtrosConceptosEmp { get; set; }

        public decimal? DeuCortoPlazoEmp { get; set; }

        public decimal? DeuLargoPlazoEmp { get; set; }

        public int? IdFile { get; set; }
        public string? Filename { get; set; }
       

        public decimal ActivoTotal
        {
            get
            {
                return ActivoCorrienteTotal + ActivoNoCorrienteTotal;
            }
        }
        public decimal ActivoCorrienteTotal
        {
            get
            {
                decimal result = 0;
                result = OoficDepositosCortoPlazoEmp.GetValueOrDefault() +
                        OoficInversionesCortoPlazoEmp.GetValueOrDefault() +
                        OoficOtrosConceptosEmp.GetValueOrDefault() +
                        OpartDepositosCortoPlazoEmp.GetValueOrDefault() +
                        OpartInversionesCortoPlazoEmp.GetValueOrDefault() +
                        OpartInversionesEmp.GetValueOrDefault() +
                        OpartOtrosConceptosEmp.GetValueOrDefault() +
                        DispCajayBancosEmp.GetValueOrDefault() +
                        BcamMaterialesEmp.GetValueOrDefault() +
                        BcamOtrosConceptosEmp.GetValueOrDefault();
                return result;
            }
        }
        public decimal ActivoNoCorrienteTotal
        {
            get
            {
                decimal result = 0;
                result = OoficDepositosLargoPlazoEmp.GetValueOrDefault() +
                        OoficInversionesLargoPlazoEmp.GetValueOrDefault() +
                        OpartDepositosLargoPlazoEmp.GetValueOrDefault() +
                        OpartInversionesLargoPlazoEmp.GetValueOrDefault() +
                        BusoInversionesEmp.GetValueOrDefault() +
                        BusoMaqUtilAfecEmp.GetValueOrDefault() +
                        BusoMaqUtilNoAfecEmp.GetValueOrDefault() +
                        BusoBienesRaicesAfecEmp.GetValueOrDefault() +
                        BusoBienesRaicesNoAfecEmp.GetValueOrDefault() +
                        BusoOtrosConceptosEmp.GetValueOrDefault();
                return result;
            }
        }
        public decimal PasivoTotal
        {
            get
            {
                return PasivoCorrienteTotal + PasivoNoCorrienteTotal;
            }
        }
        public decimal PasivoCorrienteTotal
        {
            get
            {
                decimal result = 0;
                result = DeuCortoPlazoEmp.GetValueOrDefault();
                return result;
            }
        }
        public decimal PasivoNoCorrienteTotal
        {
            get
            {
                decimal result = 0;
                result = DeuLargoPlazoEmp.GetValueOrDefault();
                return result;
            }
        }
    }

    public class BalanceGeneralModelValidator : AbstractValidator<BalanceGeneralModel>
    {
        public BalanceGeneralModelValidator()
        {

            RuleFor(p => p.Anio)
                .NotEmpty()
                .WithMessage("El campo es requerido");

            RuleFor(p => p.Anio)
                .NotEmpty().InclusiveBetween(DateTime.Now.Year - 2, DateTime.Now.Year + 1)
                .WithMessage($"El año debe ser un valor entre {DateTime.Now.Year - 2} y {DateTime.Now.Year + 1}");

            RuleFor(p => p.FechaBalance)
                .NotEmpty()
                .WithMessage("El campo es requerido")
                .GreaterThan(DateTime.Now.AddMonths(-18))
                .WithMessage("La fecha de balance no puede ser anterior a 18 meses, de lo contratio la capacidad ya se encontraría vencida")
                ;


            RuleFor(p => p.IdFile)
                .NotEmpty()
                .WithMessage("El campo es requerido");


        }
    }
}
