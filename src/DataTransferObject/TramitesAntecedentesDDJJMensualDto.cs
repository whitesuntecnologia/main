using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class TramitesAntecedentesDdjjMensualDto
    {
        public int IdTramiteAntecedenteDdjjmensual { get; set; }
        public int IdTramiteAntecedente { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
        public decimal Monto { get; set; }

    }
}
