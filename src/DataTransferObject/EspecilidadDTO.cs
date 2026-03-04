using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class EspecilidadDTO
    {
        public int IdEspecialidad { get; set; }

        public string NombreEspecialidad { get; set; } = null!;

        public string Rama { get; set; } = null!;

        public bool Baja { get; set; }

    }
}
