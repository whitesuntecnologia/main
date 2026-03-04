using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class AlertaDTO
    {
        public int IdEmpresa { get; set; }
        public string RazonSocial { get; set; }
        public string Mensaje { get; set; } = null!;
        public DateTime Vencimiento { get; set; }
    }
}
