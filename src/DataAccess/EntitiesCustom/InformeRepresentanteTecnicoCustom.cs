using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EntitiesCustom
{
    public class InformeRepresentanteTecnicoCustom
    {
        public string Apellido { get; set; } = null!;
        public string Nombres { get; set; } = null!;
        public decimal? Cuit { get; set; } = null!;
        public string RazonSocialEmpresa { get; set; } = null!;
        public List<Especialidades> Especialidades { get; set; } = null!;
        public List<Tramites> Tramites { get; set; } = null!;
        public DateTime? FechaVencimientoMatricula { get; set; }
        public DateTime? FechaVencimientoContrato { get; set; }
    }
}
