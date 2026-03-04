using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class ItemGrillaConsultarTramitesDTO
    {
        public int IdTramite { get; set; }
        public int IdTipoTramite { get; set; }
        public string NombreTipoTramite { get; set; }
        public string IdentificadorUnico { get; set; }
        public int IdEstado { get; set; }
        public string NombreEstado { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
    }

    public class ItemGrillaBandejaDTO
    {
        public int IdTramite { get; set; }
        public int IdTipoTramite { get; set; }
        public string NombreTipoTramite { get; set; }
        public string UsernameEmpresa { get; set; }
        public string IdentificadorUnico { get; set; }
        public int IdEstado { get; set; }
        public string NombreEstado { get; set; }
        public string UsuarioAsignado { get; set; } 
        public DateTime? FechaAsignacion { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
    }

    public class ItemGrillaConsultaTramiteDTO
    {
        public int IdTramite { get; set; }
        public int IdTipoTramite { get; set; }
        public string NombreTipoTramite { get; set; }
        public string UsernameEmpresa { get; set; }
        public string RazonSocialEmpresa { get; set; }
        public string IdentificadorUnico { get; set; }
        public int IdEstado { get; set; }
        public string NombreEstado { get; set; }
        public string UsuarioAsignado { get; set; }
        public string UsernameAsignado { get; set; }
        public DateTime? FechaAsignacion { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
    }

}
