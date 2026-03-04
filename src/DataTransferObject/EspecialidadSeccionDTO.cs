using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class EspecialidadSeccionDTO
    {
        public int IdSeccion { get; set; }
        public string Rama { get; set; } = null!;
        public string DescripcionSeccion { get; set; } = null!;
        public int IdEspecialidad { get; set; }
        public string DescripcionEspecialidad { get; set; } = null!;
        public bool Baja { get; set; }
        public decimal? CoefCapacTecnicaxEquipo { get; set; }
        public decimal? MultiplicadorCapEconomica { get; set; }

    }
}
