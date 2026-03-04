using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class FiltroInformeEmpresasDTO
    {
        public bool EmpresasRegistradas { get; set; }
        public decimal? Cuit { get; set; }
        public string? RazonSocial { get; set; }
        public DateTime? FechaRegistroDesde { get; set; }
        public DateTime? FechaRegistroHasta { get; set; }
        public DateTime? FechaVencimientoDesde { get; set; }
        public DateTime? FechaVencimientoHasta { get; set; }
        public IEnumerable<int> EspecialidadesSelected { get; set; } = null!;
        public decimal? CapacidadTecnicaDesde { get; set; }
        public decimal? CapacidadTecnicaHasta { get; set; }
        public decimal? CapacidadEconomicaDesde { get; set; }
        public decimal? CapacidadEconomicaHasta { get; set; }

    }
}
