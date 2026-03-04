using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class MenuTreeDTO
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } 
        public string Tipo { get;  set; }   // guarda si el dato proveiene de Menues o de permisos M/P
        public bool Checked { get; set; }   

        public List<MenuTreeDTO> SubMenues { get; set; } = new();

    }
}
