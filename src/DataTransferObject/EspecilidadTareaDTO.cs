using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class EspecilidadTareaDTO
    {
        public int IdTarea { get; set; }
        public string DescripcionTarea { get; set; } = null!;
        public int IdEspecialidad { get; set; }
        public int IdSeccion { get; set; }
        public bool Baja { get; set; }

    }
}
