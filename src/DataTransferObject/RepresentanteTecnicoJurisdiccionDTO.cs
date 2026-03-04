using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class TramitesRepresentanteTecnicoJurisdiccionDTO
    {
        public string IdRepresentanteTecnicoJurisdiccion { get; set; } = null!;
        public int IdRepresentanteTecnico { get; set; }
        public int IdProvincia { get; set; }
        public string DescripcionProvincia { get; set; }
    }
}
