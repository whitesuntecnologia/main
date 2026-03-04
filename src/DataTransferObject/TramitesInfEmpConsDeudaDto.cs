using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class TramitesInfEmpConsDeudaDto
    {
        public int IdTramiteInfEmpConsDeuda { get; set; }
        public int IdTramite { get; set; }
        public int? IdTramiteInfEmpCons { get; set; }

        public string Entidad { get; set; } = null!;

        public string Periodo { get; set; } = null!;

        public int Situacion { get; set; }

        public decimal Monto { get; set; }

        public int DiasDeAtraso { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreateUser { get; set; } = null!;

        public DateTime? LastUpdateDate { get; set; }

        public string LastUpdateUser { get; set; }

    }
}
