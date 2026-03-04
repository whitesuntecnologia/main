using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public  class TramitesObrasDTO
    {
        public int IdTramiteObra { get; set; }
        public int IdTramite { get; set; }
        public int IdTramiteEspecialidad { get; set; }
        public string NombreEspecialidad { get; set; }
        public int IdTramiteEspecialidadSeccion { get; set; }
        public int IdSeccion { get; set; }
        public string NombreSeccion { get; set; }
        public int IdObraPciaLp { get; set; }
        public string NombreObra { get; set; }
        public string Expediente { get; set; }
        public string Comitente { get; set; } = null!;
        public int? IdTipoObra { get; set; }
        public string NombreTipoObra { get; set; }
        public decimal? Monto { get; set; }
        public string PeriodoBase { get; set; } = null!;
        public string PeriodoContrato { get; set; } = null!;
        public string PeriodoFin { get; set; } = null!;
        public int IdFile { get; set; }
        public string Filename { get; set; } = null!;
    }
}
