using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class TramitesInfEmpConsPersonaDto
    {
        public int IdPersona { get; set; }
        public int IdTramiteInfEmpCon { get; set; }

        public string Apellidos { get; set; } = null!;

        public string Nombres { get; set; } = null!;

        public int IdTipoCaracterLegal { get; set; }

        public int NroDni { get; set; }

    }
}
