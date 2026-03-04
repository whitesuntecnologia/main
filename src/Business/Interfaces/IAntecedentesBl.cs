using DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Business.Interfaces
{
    public interface IAntecedentesBl
    {
        Task<List<TramitesAntecedentesDto>> GetAntecedentesAsync(int IdTramite);
        Task<TramitesAntecedentesDto> GetAntecedenteAsync(int IdTramiteAntecedente);
        Task<List<TramitesAntecedentesDto>> GetAntecedentesxEspecialidad(int IdTramiteEspecialidad, int IdTramiteEspecialidadSeccion);
        Task<List<TramitesAntecedentesDdjjMensualDto>> GetAntecedentesDdjjMensualAsync(int IdTramiteEspecialidad, int IdTramiteEspecialidadSeccion);
        Task AgregarAntecedenteAsync(TramitesAntecedentesDto dto);
        Task ActualizarAntecedenteAsync(TramitesAntecedentesDto dto);
        Task EliminarAntecedenteAsync(int IdTramiteAntecedente);
        Task ActualizarAntecedenteDdjjMensualAsync(List<TramitesAntecedentesDdjjMensualDto> lstDto);
        Task<TramitesAntecedentesResumen12MesesDto> GetResumenMejores12Meses(List<TramitesAntecedentesDdjjFilaDto> lstData);
        Task<List<TramitesAntecedentesTotales12mesesDto>> GetTotalesMejores12MesesAsync(List<TramitesAntecedentesDdjjFilaDto> lstData, int MesInicio, int AnioInicio);
        Task<List<TramitesAntecedentesDdjjFilaDto>> GetDdjjMensual5AniosAsync(DateTime FechaDesde, int IdTramiteEspecialidad, int IdTramiteEspecialidadSeccion);
        Task<List<BalanceGeneralCapacidadProduccionDto>> GetCapacidadProduccionAsync(int IdTramite);


    }

}
