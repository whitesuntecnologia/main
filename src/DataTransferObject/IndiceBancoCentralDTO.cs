using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class IndiceBancoCentralDTO
    {
        public int IdIndiceBcra { get; set; }
        public int IdSituacionBcra { get; set; }
        public string NombreSituacionBcra { get; set; }
        public decimal MinimoDeudaAdmisible { get; set; }
        public decimal MaximoDeudaAdmisible { get; set; }
    }
}
