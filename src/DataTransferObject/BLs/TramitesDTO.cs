using Microsoft.AspNetCore.Components;
using StaticClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.BLs
{
    public class TramitesDTO 
    {
        public int IdTramite { get; set; }
        public string IdentificadorUnico { get; set; }
        public int IdGrupoTramite { get; set; }
        public int IdTipoTramite { get; set; }
        public string NombreTipoTramite { get; set; }
        public int IdEmpresa { get; set; }
        public decimal CuitEmpresa { get; set; }
        public string RazonSocial { get; set; }
        public int IdEstado { get; set; }
        public string NombreEstado { get; set; }
        public bool PermiteEditarEmpresa { get; set; }
        public bool PermiteEditarFormulario (Constants.Formularios form)
        {
            bool result = false;

            if (this.IdTipoTramite == Constants.TiposDeTramite.Reli_Inscripcion 
                || this.IdTipoTramite == Constants.TiposDeTramite.Reli_ActualizacionCompleta
                || this.IdTipoTramite == Constants.TiposDeTramite.Reco_Inscripcion)
                result = true;
            else if(this.IdTipoTramite == Constants.TiposDeTramite.Reli_Licitar && 
                (form == Constants.Formularios.BoletaPago || form == Constants.Formularios.ObrasEnEjecucion))
                result = true;
            else if (this.IdTipoTramite == Constants.TiposDeTramite.Reli_ActualizacionSoloTecnicos &&
                (form == Constants.Formularios.RepresentantesTecnicos ))
                result = true;
            else if (this.IdTipoTramite == Constants.TiposDeTramite.Reli_ActualizacionCapacidadTecnica &&
                (form == Constants.Formularios.RepresentantesTecnicos || form == Constants.Formularios.Equipos 
                    || form == Constants.Formularios.Obras))
                result = true;

            return result;
        
        }
        public int? NroBoleta { get; set; }
        public int? IdFileBoleta { get; set; }
        public string FilenameBoleta { get; set; }
        public int? IdFileCumplimientoFiscal { get; set; }
        public string FilenameCumplimientoFiscal { get; set; }
        public string UsuarioAsignado { get; set; }
        public string EvaluadorAsignado { get; set; }
        public DateTime? FechaAsignacion { get; set; }
        public bool SinDatosForm8 { get; set; }
        public bool SinDatosForm10 { get; set; }
        public bool SinDatosForm11 { get; set; }
        public bool SinDatosForm12 { get; set; }
        public string numeroGEDO { get; set; }
        public int? IdFileGEDO { get; set; }
        public int? IdObraPciaLP { get; set; }
        public string NombreObraPciaLP { get; set; }
        public bool SePresentaEnUte { get; set; }
        public decimal? PorcParticipUte { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUpdateUser { get; set; }
    }
}
