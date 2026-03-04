using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class TramitesEquipoDTO
    {
        public int IdTramiteEquipo { get; set; }
        public int IdTramite { get; set; }
        public bool Afectado { get; set; }
        public int IdFileDetalleEquipo { get; set; }
        public string FilenameDetalleEquipo { get; set; }
        public int IdFileCertificadoContable { get; set; }
        public string FilenameCertificadoContable { get; set; }
        public int? IdFileDocumentacionEquipo { get; set; }
        public string FilenameDocumentacionEquipo { get; set; }

        public decimal MontoRealizacion { get; set; }
        public decimal? MontoRealizacionEvaluador { get; set; }

    }
}
