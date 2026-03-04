using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class EspecialidadTareaDTO
    {
        public int IdTarea { get; set; }
        public string DescripcionTarea { get; set; } = null!;
        public int IdSeccion { get; set; }
        public string DescripcionSeccion { get; set; } = null!;
        public bool Baja { get; set; }
    }
}
