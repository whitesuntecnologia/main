using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EntitiesCustom
{
    public class InformeObraCustom
    {
        public string? Nombre { get; set; }
        public string? Expediente { get; set; }
        public string? CuitEmpresa { get; set; }
        public string? RazonSocialEmpresa { get; set; }
        public decimal? MontoObra { get; set; }
        public decimal? PorcentajeAvance { get; set; }
    }
}
