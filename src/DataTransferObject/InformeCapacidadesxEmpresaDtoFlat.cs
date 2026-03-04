using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class InformeCapacidadesxEmpresaDtoFlat
    {
        public int IdEmpresa { get; set; }
        public string CuitEmpresa { get; set; } = string.Empty;
        public string RazonSocial { get; set; } = string.Empty;
        public string Domicilio { get; set; } = string.Empty;
        public DateTime? FechaInscripcion { get; set; }
        public DateTime? Vencimiento { get; set; }

        // Diccionario para capacidades dinámicas por especialidad
        // Key: IdEspecialidad_TipoCapacidad (ej: "1_CT", "1_CC", "1_CE")
        public Dictionary<string, decimal> Capacidades { get; set; } = new();

        // Para facilitar el acceso
        public decimal GetCapacidad(string id, string tipo)
        {
            var key = $"{id}_{tipo}";
            return Capacidades.ContainsKey(key) ? Capacidades[key] : 0;
        }

        public void SetCapacidad(string id, string tipo, decimal valor)
        {
            var key = $"{id}_{tipo}";
            Capacidades[key] = valor;
        }
    }

    public class EspecialidadInfo
    {
        public int IdSeccion { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
}
