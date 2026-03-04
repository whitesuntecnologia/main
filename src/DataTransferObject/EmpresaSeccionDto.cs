using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class EmpresaSeccionDto
    {
        public int IdEmpresaEspecialidadSeccion { get; set; }
        public int IdEmpresaEspecialidad { get; set; }
        public int IdSeccion { get; set; }
        public string DescripcionSeccion { get; set; }
        public bool Baja { get; set; }
        public virtual ICollection<EmpresaTareaDto> Tareas { get; set; } = new List<EmpresaTareaDto>();
    }
}
