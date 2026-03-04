using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class EmpresaDTO
    {
        public int IdEmpresa { get; set; }
        public decimal CuitEmpresa { get; set; }
        public string RazonSocial { get; set; } = null!;
        public string Domicilio { get; set; } = null!;
        public DateTime? Vencimiento { get; set; }
        public DateTime? FechaBalance { get; set; }
        public int? AniosAntiguedad { get; set; }
        public int? IdTramiteOrigen { get; set; }
        public string UseridRepresentante { get; set; }
        public string CuitRepresentante { get; set; }
        public DateTime FechaVigenciaDesde { get; set; }
        public DateTime? FechaVigenciaHasta { get; set; }
        public virtual ICollection<EmpresaDeudaDto> Deudas { get; set; } = new List<EmpresaDeudaDto>();
        public virtual ICollection<EmpresaEspecialidadDto> Especialidades { get; set; } = new List<EmpresaEspecialidadDto>();
        public virtual ICollection<EmpresaRepresentanteTecnicoDto> RepresentantesTecnicos { get; set; } = new List<EmpresaRepresentanteTecnicoDto>();
        public virtual ICollection<EmpresaSancionDTO> Sanciones { get; set; } = new List<EmpresaSancionDTO>();
    }
}
