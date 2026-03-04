using DataTransferObject.BLs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class InformeEmpresaDTO
    {
        public int IdEmpresa { get; set; }
        public decimal CuitEmpresa { get; set; }
        public string RazonSocial { get; set; } = null!;
        public string Domicilio { get; set; } = null!;
        public List<EspecialidadDTO> Especialidades { get; set; } = new();
        public decimal CapacidadEconomicaAnualOtorgada { get; set; }
        public decimal CapacidadTecnicaAnualOtorgada { get; set; }
        public decimal CapacidadContratacionAnualOtorgada { get; set; }
        public DateTime? FechaDeRegistro { get; set; }
        public DateTime? FechaDeVencimiento { get; set; }
        public int CantidadRepresentantesTecnicos { get; set; }
        public int? IdTramiteOrigen { get; set; }
        public int? IdTramiteOrigenReco { get; set; }
        public string Especialidades1Linea
        {
            get
            {
                return string.Join(Environment.NewLine, Especialidades.Select(s => s.NombreEspecialidad).ToArray());
            }
        }
    }
}
