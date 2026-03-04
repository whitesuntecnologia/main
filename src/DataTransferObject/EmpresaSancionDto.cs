using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class EmpresaSancionDTO
    {
        public int IdEmpresaSancion { get; set; }
        public int IdEmpresa { get; set; }
        public string Nombre { get; set; } = null!;
        public int IdFileSancion { get; set; }
        public DateTime FechaDesdeSancion { get; set; }
        public DateTime? FechaHastaSancion { get; set; }
        public string FilenameSancion { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; } = null!;
    }
}
