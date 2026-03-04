using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class TramitesRepresentanteTecnicoEspecilidadDTO
    {
        public int IdRepresentanteEspecialidad { get; set; }
        public int IdRepresentanteTecnico { get; set; }
        public int IdTramiteEspecialidad { get; set; }
        public string NombreEspecialidad { get; set; }
    }
}
