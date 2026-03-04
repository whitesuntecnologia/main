using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class TramitesAntecedentesDto
    {
        public int IdTramiteAntecedente { get; set; }
        public int IdTramite { get; set; }
        public int IdTramiteEspecialidad { get; set; }
        public string DescripcionEspecialidad { get; set; }
        public int IdTramiteEspecialidadSeccion { get; set; }
        public string DescripcionSeccion { get; set; }
        public int IdObraPciaLp { get; set; }
        public string NombreObra { get; set; } = null!;

        public string Ubicacion { get; set; } = null!;

        public string Comitente { get; set; } = null!;

        public string PeriodoBase { get; set; } = null!;

        public string PeriodoInicio { get; set; } = null!;

        public int Plazo { get; set; }

        public decimal MontoContrato { get; set; }

        public decimal MontoEjecutado { get; set; }

        public decimal MontoAejecutar { get; set; }

        public string RepresentanteTecnico { get; set; } = null!;

        public int IdFile { get; set; }

        public string Filename { get; set; } = null!;
    }

}
