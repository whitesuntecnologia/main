using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class ItemGrillaVisorTramiteDTO
    {
        public int IdTramite { get; set; }
        public int NroFormulario { get; set; }
        public string IdentificadorUnico { get; set; }
        public string DescripcionFormulario { get; set; }
        public bool Guardado { get; set; }
        public double PorcentajeAvance { get; set; }
        public string Url { get; set; }
        public int? IdEstadoEvaluacion { get; set; }
        public string NombreEstadoEvaluacion { get; set; }
        public string MensajeNotificacion { get; set; }
        public bool PermiteEditar { get; set; }
    }
}
