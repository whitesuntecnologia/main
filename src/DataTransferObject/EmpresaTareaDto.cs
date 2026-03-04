using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class EmpresaTareaDto
    {
        public int IdEmpresaEspecialidadTarea { get; set; }

        public int? IdEmpresaEspecialidadSeccion { get; set; }

        public int IdTarea { get; set; }
        public string DescripcionTarea { get; set; }
        public bool Baja { get; set; }
        public virtual ICollection<EmpresaEquipoDto> Equipos { get; set; } = new List<EmpresaEquipoDto>();
    }
}
