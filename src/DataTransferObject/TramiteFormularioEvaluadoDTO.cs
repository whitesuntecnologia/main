using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class TramiteFormularioEvaluadoDTO
    {
        public int IdTramiteFormEvaluado { get; set; }

        public int IdTramite { get; set; }
        public int NroNotificacion { get; set; }
        public int NroFormulario { get; set; }

        public int IdEstadoEvaluacion { get; set; }
        public string NombreEstadoEvaluacion { get; set; }

        public string MensajeNotificacion { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
