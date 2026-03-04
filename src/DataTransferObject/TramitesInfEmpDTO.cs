using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class TramitesInfEmpDTO
    {
        public int IdTramiteInfEmp { get; set; }
        public int IdTramite { get; set; }
        public DateTime FechaInicioActividades { get; set; }
        public int? IdFileConstanciaInscImpNacionales { get; set; }
        public string FilenameConstanciaInscImpNacionales { get; set; }
        public int? IdFileConstanciaBcra { get; set; }
        public string FilenameConstanciaBcra { get; set; }
        
    }
}
