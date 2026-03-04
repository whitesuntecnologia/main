using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class TramitesBalancesGeneralesEvaluarCapTecnicaDTO
    {
        public int IdTramiteBalanceGeneralEvaluarCt { get; set; }
        public int IdTramiteBalanceGeneralEvaluar { get; set; }
        public int? IdSeccion { get; set; }
        public string? DescripcionSeccion { get; set; }
        public decimal? Monto { get; set; }
        public int? Mes { get; set; }
        public int? Anio { get; set; }
        public decimal? CoefActualizacion { get; set; }
        public decimal? CoefTipoObra { get; set; }
        public decimal? CapacidadTecnicaxEquipo { get; set; }
        public decimal? CoerficienteConceptual { get; set; }
        public decimal? CapacidadTecnicaFinal { get; set; }

    }
}
