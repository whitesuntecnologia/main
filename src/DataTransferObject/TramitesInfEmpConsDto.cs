using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public  class TramitesInfEmpConsDto
    {
        public int IdTramiteInfEmpCon { get; set; }
        public int IdTramite { get; set; }
        public int TipoEmpresa { get; set; }
        public string RazonSocial { get; set; } = null!;
        public int? IdTipoSociedad { get; set; }
        public int? IdFileContrato { get; set; }
        public string FilenameContrato { get; set; }
        public int? IdFileEstatuto { get; set; }
        public string FilenameEstatuto { get; set; }
        public string DeRegComercio { get; set; } = null!;
        public DateTime? FechaRegComercio { get; set; }
        public string LibroRegComercio { get; set; } = null!;
        public string TomoRegComercio { get; set; } = null!;
        public string FolioRegComercio { get; set; } = null!;
        public int? IdFileRegComercio { get; set; }
        public string FilenameRegComercio { get; set; }
        public int? AniosDuracionSoc { get; set; }
        public DateTime? FechaConstitucionSoc { get; set; }
        public DateTime? FechaVencimientoSoc { get; set; }
        public string ProrrogaDePlazoSoc { get; set; }
        public int? IdFileActaDesignacion { get; set; }
        public string FilenameActaDesignacion { get; set; }
        public string Cuit { get; set; }
        public string NroIibb { get; set; }
        public DateTime? FechaInscripcionIibb { get; set; }
        public int TipoIibb { get; set; }
        public int TipoIva { get; set; }
        public DateTime? FechaInscripcionIva { get; set; }
        public bool EsInscriptoGan { get; set; }
        public DateTime? FechaInscripcionGan { get; set; }
        public string OtrosImpuestos { get; set; }
        public int? IdFileConstanciaBcra { get; set; }
        public string FilenameConstanciaBcra { get; set; }
        public List<TramitesInfEmpConsPersonaDto> Personas { get;set; }
        public List<TramitesInfEmpConsDocumentoDto> Documentos { get; set; }
        public List<TramitesInfEmpConsDeudaDto> Deudas { get; set; }
    }
}
