using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class EmpresaRepresentanteTecnicoDto
    {
        public int IdEmpresaRepresentanteTecnico { get; set; }

        public int IdEmpresa { get; set; }

        public int IdGrupoTramite { get; set; }

        public string Apellido { get; set; } = null!;

        public string Nombres { get; set; } = null!;

        public decimal Cuit { get; set; }

        public string Cargo { get; set; } = null!;

        public string Matricula { get; set; } = null!;

        public DateTime FechaVencimientoMatricula { get; set; }

        public DateTime FechaVencimientoContrato { get; set; }

        public int IdFileContrato { get; set; }

        public string FilenameContrato { get; set; } = null!;

        public int IdFileBoleta { get; set; }

        public string FilenameBoleta { get; set; } = null!;

        public int IdFileMatricula { get; set; }

        public string FilenameMatricula { get; set; } = null!;
    }
}
