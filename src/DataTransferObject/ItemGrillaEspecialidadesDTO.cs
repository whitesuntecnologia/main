using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class ItemGrillaEspecialidadesDTO
    {
        public int IdTramiteEspecialidad { get; set; }
        public int IdTramite { get; set; }
        public int IdEspecialidad { get; set; }
        public string DescripcionEspecialidad { get; set; }
        public string Rama { get; set; }
        public List<ItemGrillaEspecialidadesSeccionesDTO> Secciones { get; set; } = new();
    }

    public class ItemGrillaEspecialidadesSeccionesDTO
    {
        public int IdTramiteEspecialidadSeccion { get; set; }
        public int IdSeccion { get; set; }
        public string DescripcionSeccion { get; set; }
        public bool Baja { get; set; }
        public decimal? CoefCapacTecnicaxEquipo { get; set; }
        public decimal? MultiplicadorCapEconomica { get; set; }

        public List<ItemGrillaEspecialidadesTareasDTO> Tareas { get; set; } = new();
    }

    public class ItemGrillaEspecialidadesTareasDTO
    {
        public int IdTarea { get; set; }
        public string DescripcionTarea { get; set; }
        public int IdSeccion { get; set; }
        public string DescripcionSeccion { get; set; }
        public List<ItemGrillaEspecialidadesEquiposDTO> Equipos { get; set; } = new();
    }
    public class ItemGrillaEspecialidadesEquiposDTO
    {
        public int IdEquipo { get; set; }
        public string DescripcionEquipo { get; set; }
    }
}
