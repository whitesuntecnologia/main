using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class TramitesBalanceGeneralDTO
    {
        public int? IdTramiteBalanceGeneral { get; set; }

        public int IdTramite { get; set; }
        public int IdTipoTramite { get; set; }

        public int Anio { get; set; }

        public DateTime FechaBalance { get; set; }

        public decimal? OoficDepositosCortoPlazo { get; set; }

        public decimal? OoficDepositosLargoPlazo { get; set; }

        public decimal? OoficInversionesCortoPlazo { get; set; }

        public decimal? OoficInversionesLargoPlazo { get; set; }

        public decimal? OoficOtrosConceptos { get; set; }

        public decimal? OpartDepositosCortoPlazo { get; set; }

        public decimal? OpartDepositosLargoPlazo { get; set; }

        public decimal? OpartInversionesCortoPlazo { get; set; }

        public decimal? OpartInversionesLargoPlazo { get; set; }

        public decimal? OpartInversiones { get; set; }

        public decimal? OpartOtrosConceptos { get; set; }

        public decimal? DispCajayBancos { get; set; }

        public decimal? BusoInversiones { get; set; }

        public decimal? BusoMaqUtilAfec { get; set; }

        public decimal? BusoMaqUtilNoAfec { get; set; }

        public decimal? BusoBienesRaicesAfec { get; set; }

        public decimal? BusoBienesRaicesNoAfec { get; set; }

        public decimal? BusoOtrosConceptos { get; set; }

        public decimal? BcamMateriales { get; set; }

        public decimal? BcamOtrosConceptos { get; set; }

        public decimal? DeuCortoPlazo { get; set; }

        public decimal? DeuLargoPlazo { get; set; }

        public int? IdFile { get; set; }
        public string Filename { get; set; }

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
    }
}
