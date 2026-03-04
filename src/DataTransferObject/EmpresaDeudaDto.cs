using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class EmpresaDeudaDto
    {
        public int IdEmpresaDeuda { get; set; }

        public int IdEmpresa { get; set; }

        public int? IdGrupoTramite { get; set; }

        public string Entidad { get; set; } = null!;

        public string Periodo { get; set; } = null!;

        public int Situacion { get; set; }

        public decimal Monto { get; set; }

        public int DiasDeAtraso { get; set; }

    }
}
