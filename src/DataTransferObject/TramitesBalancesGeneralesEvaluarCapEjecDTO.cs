using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class TramitesBalancesGeneralesEvaluarCapEjecDTO
    {
        public int IdTramiteBalanceGeneralEvaluarCe { get; set; }
        public int IdTramiteBalanceGeneralEvaluar { get; set; }
        public int? IdSeccion { get; set; }
        public string? DescripcionSeccion { get; set; }
        public decimal? CapacidadEconomica { get; set; }
        public decimal? CapacidadProduccion { get; set; }
        public decimal? CapacidadEconomica080 { get; set; }
        public decimal? CapacidadProduccion020 { get; set; }
        public decimal? CapacidadEjecucion { get; set; }
        public decimal? CoeficienteConceptual { get; set; }
        public decimal? CapacidadEjecucionAnual { get; set; }
        public decimal? CoeficienteEstadoDeudaBcra { get; set; }
        public decimal? CapacidadEjecucionFinal { get; set; }
        public bool ExistsInForm1 { get; set; }
    }
}
