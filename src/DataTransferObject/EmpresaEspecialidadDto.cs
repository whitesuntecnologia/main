using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class EmpresaEspecialidadDto
    {
        public int IdEmpresaEspecialidad { get; set; }

        public int IdEmpresa { get; set; }

        public int IdGrupoTramite { get; set; }

        public int IdEspecialidad { get; set; }
        public string NombreEspecialidad { get; set; }
        public string Rama { get; set; }
        public bool Baja { get; set; }
        public virtual ICollection<EmpresaSeccionDto> Secciones { get; set; } = new List<EmpresaSeccionDto>();
    }
}
