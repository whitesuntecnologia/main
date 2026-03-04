using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class ItemGrillaMisCertificadosDTO
    {
        public int IdTramite { get; set; }
        public int IdTipoTramite { get; set; }
        public string NombreTipoTramite { get; set; }
        public int IdFileGEDO { get; set; }
        public string numeroGEDO { get; set; }
    }
}
