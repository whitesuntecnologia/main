using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class itemGrillaConsultaEmpresasDTO
    {
        public int IdEmpresa { get;set; }
        public string RazonSocial { get;set;}
        public string Cuit { get; set; }
        public decimal? CapacidadEconomica { get; set; }
        public decimal? CapacidadTecnica { get; set; }
        public decimal? CapacidadContratacion { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public int CantidadRepresentantesTecnicos { get; set; }
        public List<EspecialidadDTO> Especialidades { get; set; }
        List<string> TramitesInscriptos { get; set; }
    }


}
