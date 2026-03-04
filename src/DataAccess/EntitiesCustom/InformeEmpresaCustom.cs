using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EntitiesCustom
{
    public class InformeEmpresaCustom
    {
        public int IdEmpresa { get; set; }
        public decimal CuitEmpresa { get; set; }
        public string RazonSocial { get; set; } = null!;
        public string Domicilio { get; set; } = null!;
        public List<Especialidades> Especialidades { get; set; } = new();
        public decimal CapacidadEconomicaAnualOtorgada { get; set; }
        public decimal CapacidadTecnicaAnualOtorgada { get; set; }
        public decimal CapacidadContratacionAnualOtorgada { get; set; }
        public DateTime? FechaDeRegistro { get; set; }
        public DateTime? FechaDeVencimiento { get; set; }
        public int CantidadRepresentantesTecnicos { get; set; }
        public List<Tramites> Tramites { get; set; } = new();
    }
}
