using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class TramitesBalancesGeneralesEvaluarCapTecxEquipoDTO
    {
        public int IdTramiteBalanceGeneralEvaluarCte { get; set; }
        public int IdTramiteBalanceGeneralEvaluar { get; set; }
        public int? IdSeccion { get; set; }
        public string? DescripcionSeccion { get; set; }
        public decimal? Importe { get; set; }
        public decimal? CoefCapacTecnicaxEquipo { get; set; }
        public decimal? MultiplicadorCapEconomica { get; set; }
        public decimal? ImporteResultante { get; set; }
    }
}
