using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class FiltroInformeResponsablesTecnicosDTO
    {
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public decimal? Cuit { get; set; }
        public string RazonSocial { get; set; }
        public DateTime? FechaVencimientoMatriculaDesde { get; set; }
        public DateTime? FechaVencimientoMatriculaHasta { get; set; }
        public DateTime? FechaVencimientoContratoDesde { get; set; }
        public DateTime? FechaVencimientoContratoHasta { get; set; }
    }
}
