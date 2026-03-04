using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class TramitesBienesRaicesDTO
    {
        
        public int IdTramiteBienRaiz { get; set; }
        public int IdTramite { get; set; }

        public bool Afectado { get; set; }

        public int IdFileDetalleInmueble { get; set; }

        public string FilenameDetalleInmueble { get; set; } = null!;

        public int IdFileCertificadoContable { get; set; }

        public string FilenameCertificadoContable { get; set; } = null!;

        public decimal MontoRealizacion { get; set; }
        public decimal? MontoRealizacionEvaluador { get; set; }
        public bool SinDatosForm8 { get; set; }

    }
}
