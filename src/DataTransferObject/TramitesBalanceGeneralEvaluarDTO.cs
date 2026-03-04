using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class TramitesBalanceGeneralEvaluarDTO
    {
        public int IdTramiteBalanceGeneralEvaluar { get; set; }
        public int IdTramiteBalanceGeneral { get; set; }
        public decimal CoefObrasOficialesDepositosCortoPlazo { get; set; }
        public decimal CoefObrasOficialesDepositosLargoPlazo { get; set; }
        public decimal CoefObrasOficialesInversionesCortoPlazo { get; set; }
        public decimal CoefObrasOficialesInversionesLargoPlazo { get; set; }
        public decimal CoefObrasOficialesOtrosConceptos { get; set; }
        public decimal CoefObrasParticularesDepositosCortoPlazo { get; set; }
        public decimal CoefObrasParticularesDepositosLargoPlazo { get; set; }
        public decimal CoefObrasParticularesInversionesCortoPlazo { get; set; }
        public decimal CoefObrasParticularesInversionesLargoPlazo { get; set; }
        public decimal CoefObrasParticularesInversiones { get; set; }
        public decimal CoefObrasParticularesOtrosConceptos { get; set; }
        public decimal CoefDisponibilidadesCajaYBancos { get; set; }
        public decimal CoefBienesDeUsoInversiones { get; set; }
        public decimal CoefBienesDeUsoEquiposAfectados { get; set; }
        public decimal CoefBienesDeUsoEquiposNoAfectados { get; set; }
        public decimal CoefBienesDeUsoInmueblesAfectados { get; set; }
        public decimal CoefBienesDeUsoInmueblesNoAfectados { get; set; }
        public decimal CoefBienesDeUsoOtrosConceptos { get; set; }
        public decimal CoefBienesDeCambioMaterialesEnDeposito { get; set; }
        public decimal CoefBienesDeCambioOtrosConceptos { get; set; }
        public decimal CoefDeudasCortoPlazo { get; set; }
        public decimal CoefDeudasLargoPlazo { get; set; }
        public decimal CoefCapacidadEconomicaArqRep { get; set; }
        public decimal CoefCapacidadEconomicaByC { get; set; }
        public decimal CoefCapacidadEconomicaAyC { get; set; }
        public decimal CapitalRealEspecifico { get; set; }
        public decimal CapitalRealEspecificoEvaluar { get; set; }
        public decimal CapacidadEconomicaArqRep { get; set; }
        public decimal CapacidadEconomicaByC { get; set; }
        public decimal CapacidadEconomicaAyC { get; set; }
        public int EstadoDeudaBcra { get; set; }
        public decimal CoeficienteEstadoDeudaBcra { get; set; }
        public decimal TotalEquiposAfecEvaluar { get; set; }
        public decimal TotalEquiposNoAfecEvaluar { get; set; }
        public decimal TotalInmueblesAfecEvaluar { get; set; }
        public decimal TotalInmueblesNoAfecEvaluar { get; set; }
        public decimal IndiceSolvencia { get; set; }
        public decimal IndiceLiquidez { get; set; }
        public decimal IndiceSolvenciaResultante { get; set; }
        public decimal IndiceLiquidezResultante { get; set; }
        public decimal IndiceAntiguedadEmpresa { get; set; }
        public decimal IndiceRelacionEquipos { get; set; }
        public decimal IndiceRelacionEquiposResultante { get; set; }
        public decimal Factor { get; set; }
        public decimal TotalContratadoObrasEjecucion { get; set; }
        public decimal IndiceUvi { get; set; }
        public decimal CoeficienteConceptual { get; set; }
        public int AniosAntiguedadEmpresa { get; set; }
        public List<TramitesBalancesGeneralesEvaluarConstanciaDTO> TramitesBalancesGeneralesEvaluarConstanciaCap { get; set; } = new();
        public List<TramitesBalancesGeneralesEvaluarCapTecnicaDTO> TramitesBalancesGeneralesEvaluarCapTecnica { get; set; } = new();
        public List<TramitesBalancesGeneralesEvaluarCapEjecDTO> TramitesBalancesGeneralesEvaluarCapEjec { get; set; } = new();
        public List<TramitesBalancesGeneralesEvaluarCapProdDTO> TramitesBalancesGeneralesEvaluarCapProd { get; set; } = new();
        public List<TramitesBalancesGeneralesEvaluarCapTecxEquipoDTO> TramitesBalancesGeneralesEvaluarCapTecxEquipo { get; set; } = new();
        public List<TramitesBalancesGeneralesEvaluarDetalleObrasEjecucionDTO> TramitesBalancesGeneralesEvaluarDetalleObrasEjecucion { get; set; } = new();

    }
}
