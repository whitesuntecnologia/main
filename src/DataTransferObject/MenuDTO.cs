using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class MenuDTO
    {
        public int IdMenu { get; set; }

        public string Descripcion { get; set; } = null!;
        public string Aclaraciones { get; set; } = null!;

        public string IconCss { get; set; }

        public string Url { get; set; } = null!;

        public int? IdMenuPadre { get; set; }

        public bool Visible { get; set; }
        public string TituloHijos { get; set; }
        public string Tipo { get; set; }   // guarda si el dato proveiene de Menues o de permisos M/P

        public List<MenuDTO> SubMenues { get; set; } = new();
        public MenuDTO MenuPadre { get; set; } 
        public List<PermisoDTO> Permisos { get; set; } = new();
        public bool TienePermiso { get; set; }
        
    }

    public class PermisoDTO
    {
        public int IdPermiso { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public List<int> Menues { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
