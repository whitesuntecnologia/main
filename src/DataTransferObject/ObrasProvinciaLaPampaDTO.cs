using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class ObrasProvinciaLaPampaDTO
    {
        public int IdObraPciaLp { get; set; }
        public string Expediente { get; set; } = null!;
        public string ObraNombre { get; set; } = null!;
        public string EstadoObra { get; set; }
        public string Licitante { get; set; }
        public int? PlazoObra { get; set; }
        public decimal? MontoObra { get; set; }
        public DateTime? FechaFinObra { get; set; }
        public decimal? MontoDolaresObra { get; set; }
        public string Empresa { get; set; }
        public string CuitEmpresa { get; set; }
        public bool? EsUte { get; set; }
        public decimal? PorcentajeParticipacion { get; set; }
        public int? AnioAvanceObra { get; set; }
        public string MesAvanceObra { get; set; }
        public decimal? PorcentajeAvanceObra { get; set; }
        public bool? EsAltaPorProceso { get; set; }
        public bool EsAltaPorUsuario { get; set; }
        public bool? BajaLogica { get; set; }
        public decimal? CoeficienteConceptual { get; set; }
        public DateTime? FechaInformeCoeficiente { get; set; }
        public int? IdFileInformeCoeficiente { get; set; }
        public string FilenameInformeCoeficiente { get; set; }
    }
}
