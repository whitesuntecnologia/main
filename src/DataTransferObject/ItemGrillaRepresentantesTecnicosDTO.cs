using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public  class ItemGrillaRepresentantesTecnicosDTO
    {
        public int IdRepresenta { get; set; }
        public int IdTramite { get; set; }
        public int IdEspecialidad { get; set; }
        public string DescripcionEspecialidad { get; set; }
    }
}
