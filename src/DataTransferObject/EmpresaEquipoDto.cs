using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class EmpresaEquipoDto
    {
        public int IdEmpresaEspecialidadEquipo { get; set; }

        public int IdEmpresaEspecialidadTarea { get; set; }

        public int IdEquipo { get; set; }
        public string DescripcionEquipo { get; set; }
        public bool Baja { get; set; }
    }
}
