using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EntitiesCustom
{
    public class AlertaCustom
    {
        public int NroOrden { get; set; }
        public string Titulo { get; set; } = null!;
        public string Mensaje { get; set; } = null!;
        public DateTime Vencimiento { get; set; }
    }
}
