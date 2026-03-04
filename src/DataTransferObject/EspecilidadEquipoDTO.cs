using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class EspecialidadEquipoDTO
    {
        public int IdEquipo { get; set; }
        public string DescripcionEquipo { get; set; } = null!;
        public bool Baja { get; set; }
    }
}
