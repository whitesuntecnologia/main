using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class TramitesBalancesGeneralesEvaluarConstanciaDTO
    {
        public int IdTramiteBalanceGeneralEvaluarCc { get; set; }
        public int IdTramiteBalanceGeneralEvaluar { get; set; }
        public int? IdSeccion { get; set; }
        public string? DescripcionSeccion { get; set; }
        public decimal? CapacidadTecnica { get; set; }
        public decimal? CapacidadTecnicaUvi { get; set; }
        public decimal? CapacidadContratacion { get; set; }
        public decimal? CapacidadContratacionUvi { get; set; }
        public decimal? EjecucaionAnual { get; set; }
        public decimal? EjecucaionAnualUvi { get; set; }
    }
}
