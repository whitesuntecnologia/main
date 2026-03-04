using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class TramitesObraEjecucionDTO
    {
        public int IdTramiteObraEjec { get; set; }

        public int IdTramite { get; set; }

        public string Expediente { get; set; } = null!;

        public int IdObraPciaLp { get; set; }
        public string ObraNombre { get; set; } = null!;
        public decimal? PorcentajeParticipacionUTE { get; set; } = null!;

        public string Comitente { get; set; } = null!;

        public int IdTipoObra { get; set; }
        public string NombreTipoObra { get; set; }
        public string Ubicacion { get; set; } = null!;

        public string PeriodoBase { get; set; } = null!;

        public DateTime FechaInicio { get; set; }

        public decimal MontoMensual { get; set; }

        public decimal MontoAnual { get; set; }

        public decimal TotalContratado { get; set; }

        public decimal TotalCertificado { get; set; }

        public decimal Saldo { get; set; }

        public decimal PorcentajeCertificado { get; set; }
        public decimal PlazoObra { get; set; }
        public decimal PlazoAmpliacion { get; set; }
        
        public int IdFile { get; set; }

        public string Filename { get; set; } = null!;

    }
}
