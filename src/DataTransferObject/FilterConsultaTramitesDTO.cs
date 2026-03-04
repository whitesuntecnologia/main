using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class FilterConsultaTramitesDTO
    {
        public int? IdEstado { get; set; }
        public int? IdTramite { get; set; }
        public int? IdEmpresa { get; set; }
    }
}
