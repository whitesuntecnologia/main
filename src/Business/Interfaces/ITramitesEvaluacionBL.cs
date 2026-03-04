using DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface ITramitesEvaluacionBL
    {
        Task<TramiteFormularioEvaluadoDTO> GetEvaluacionAsync(int IdTramite, int Nronotificacion, int NroFormulario);
        Task<int> GetNumeroNotificacionActualAsync(int IdTramite);
        Task<TramiteFormularioEvaluadoDTO> GuardarEvaluacionAsync(TramiteFormularioEvaluadoDTO dto);
        Task<bool> isAssignedAsync(int IdTramite, string userid);
        Task<int> GetEstadoDeudaBCRAAsync(int IdTramite);
        Task<List<ItemGrillaBGECapacidadTecnicaxEquipoDTO>> GetGrillaBGECapacidadTecnicaxEquipoAsync(int IdTramite);
        Task<List<ItemGrillaBGECapacidadTecnica2DTO>> GetGrillaCapacidadTecnica2(int IdTramite);
        Task<decimal> GetPromedioCoeficienteConceptualAsync(int IdTramite);
        Task ActualizarEquiposMontoRealizacionEvaluador(int IdTramite, decimal MontoRealizacionAfectado, decimal? MontoRealizacionNoAfectado);
        Task ActualizarBienesRaicesMontoRealizacionEvaluador(int IdTramite, decimal MontoRealizacionAfectado, decimal? MontoRealizacionNoAfectado);
        Task<TramitesBalanceGeneralEvaluarDTO> InsertOrUpdateBalanceGeneralEvaluar(TramitesBalanceGeneralEvaluarDTO dto);
        Task<TramitesBalanceGeneralEvaluarDTO> GetBalanceGeneralEvaluar(int IdTramiteBalanceGeneral);
        Task<decimal> GetCoeficienteMatrizObrasEjecAsync(decimal PorcentajeCertificado, decimal PorcentajeTiempo);
        Task ActualizarEvaluadorAsignadoAsync(int IdTramite, string useridNuevoEvaludor);
        Task ActualizarTramiteNumeroGEDOAsync(int IdTramite, string numeroGEDO);
        Task ActualizarTramiteFileGEDOAsync(int IdTramite, int IdFileGEDO);
        Task ActualizarCamposBalanceGeneralEvaluadorAsync(TramitesBalanceGeneralDTO dto);
        Task<ObrasProvinciaLaPampaDTO> GetDatosObraLicitarAsync(int IdTramite);
    }
}
