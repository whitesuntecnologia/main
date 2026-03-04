using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class TramitesInfEmpDocumentoDTO
    {
        public int IdTramiteInfEmpDocumento { get; set; }
        public int IdTramite { get; set; }
        public int? IdTramiteInfEmp { get; set; }

        public int IdTipoDocumento { get; set; }
        public string DescripcionTipoDocumento { get; set; }

        public int IdFile { get; set; }
        public string Filename { get; set; }


        public DateTime CreateDate { get; set; }

        public string CreateUser { get; set; } = null!;

        //propiedades solo para leer en las grillas
        public long Size { get; set; }
        public string SizeStr
        {
            get
            {
                decimal kb = this.Size / 1024.0m;
                decimal mb = kb / 1024.0m;
                string result = (mb < 1 ? Math.Ceiling(kb).ToString("N0") + " Kb." : mb.ToString("N2") + " Mb.");
                return result;
            }
        }


    }
}
