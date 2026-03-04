using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public  class TramitesRepresentanteTecnicoDTO
    {
        public int IdRepresentanteTecnico { get; set; }
        public int IdTramite { get; set; }
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public decimal CUIT { get; set; }
        public string Cargo { get; set; }
        public string Matricula { get; set; }
        public DateTime FechaVencimientoMatricula { get; set; }
        public DateTime FechaVencimientoContrato { get; set; }
        public int IdFileContrato { get; set; }
        public string FilenameContrato { get; set; }
        public int IdFileBoleta { get; set; }
        public string FilenameBoleta { get; set; }
        public int IdFileMatricula { get; set; }
        public string FilenameMatricula { get; set; }
        public int? IdFileCurriculum { get; set; }
        public string FilenameCurriculum { get; set; }
        public List<TramitesRepresentanteTecnicoJurisdiccionDTO> TramitesRepresentantesTecnicosJurisdicciones { get; set; } = new();
        public List<TramitesRepresentanteTecnicoEspecilidadDTO> TramitesRepresentantesTecnicosEspecialidades { get; set; } = new();

        public bool EstaRegistrado { get; set; }    //Indica si existe en la tabla EmrpesasRepresentantesTecnicos el CUIT del representante.
        public bool EstaDesvinculado { get; set; }    //Indica si existe en la tabla TramitesRepresentantesTecnicosDesvinculaciones.
        public int? IdFileDesvinculacion { get; set; }
    }

}
