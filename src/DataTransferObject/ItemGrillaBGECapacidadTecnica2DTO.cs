using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class ItemGrillaBGECapacidadTecnica2DTO
    {
        public int IdSeccion { get; set; }
        public string Rama { get; set; }
        public string DescripcionSeccion { get; set; }

        public decimal? Monto { get; set; }
        public int? Mes { get; set; }
        public int? Anio { get; set; }
        public string Periodo
        {
            get
            {
                string res = null;
                if(Mes > 0 && Anio > 0)
                    res = Mes.ToString().PadLeft(2, Convert.ToChar("0")) + "/" + Anio.ToString();

                return res;
            }
        }
        public decimal? CoefActualizacion { get; set; }
        public decimal? MontoActualizado
        {
            get
            {
                return Monto * CoefActualizacion;
            }
        }
        public decimal? CoefTipoObra { get; set; } 
        public decimal? CapacidadTecnica
        {
            get
            {
                return MontoActualizado * CoefTipoObra;
            }
        }

        public decimal? CapacidadTecnicaxEquipo { get; set; }
        public decimal? CoerficienteConceptual { get; set; }
        public decimal? CapacidadTexnica { get; set; }
    }
 
}
