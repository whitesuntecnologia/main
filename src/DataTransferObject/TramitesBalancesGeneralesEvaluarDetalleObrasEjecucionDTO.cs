using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class TramitesBalancesGeneralesEvaluarDetalleObrasEjecucionDTO
    {
        public int IdTramiteBalanceGeneralEvaluarDoe { get; set; }
        public int IdTramiteBalanceGeneralEvaluar { get; set; }
        public int IdObraPciaLp { get; set; }
        public string ObraNombre { get; set; } = null!;
        public decimal TotalContratado { get; set; }
        public decimal TotalCertificado { get; set; }
        public int Plazo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaCertificacion { get; set; }
        public int DiasTranscurridos { get; set; }
        public decimal MontoMensual { get; set; }
        public decimal MontoAnual { get; set; }
        public decimal CoefCertificado { get; set; }
        public decimal PorcentajeCertificado { get; set; }
        public decimal PorcentajeTiempo { get; set; }
        public decimal CoefMatriz { get; set; }
        public decimal CoefParticipacion { get; set; }
        public decimal MontoComprometido { get; set; }
    }
}
