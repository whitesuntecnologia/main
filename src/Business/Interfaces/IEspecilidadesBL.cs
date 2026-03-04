using DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IEspecilidadesBL
    {
        Task<List<EspecialidadEquipoDTO>> GetEspecialidadesEquiposAsync(CancellationToken cancellationToken = default);
        Task<EspecialidadEquipoDTO> GetEspecialidadEquipoAsync(int IdEquipo);
        Task AgregarEquipoAsync(EspecialidadEquipoDTO dto);
        Task ActualizarEquipoAsync(EspecialidadEquipoDTO dto);
        Task EliminarEquipoAsync(int IdEquipo);
        Task<List<EspecialidadSeccionDTO>> GetEspecialidadesSeccionesAsync(CancellationToken cancellationToken = default);
        Task<EspecialidadSeccionDTO> GetEspecialidadSeccionAsync(int IdSeccion);
        Task AgregarSeccionAsync(EspecialidadSeccionDTO dto);
        Task ActualizarSeccionAsync(EspecialidadSeccionDTO dto);
        Task EliminarSeccionAsync(int IdSeccion);
        Task<List<EspecialidadDTO>> GetEspecialidadesAsync(CancellationToken cancellationToken = default);
        Task<EspecialidadDTO> GetEspecialidadAsync(int IdEspecialidad);
        Task AgregarEspecialidadAsync(EspecialidadDTO dto);
        Task ActualizarEspecialidadAsync(EspecialidadDTO dto);
        Task EliminarEspecialidadAsync(int IdEspecialidad);
        Task<List<EspecialidadTareaDTO>> GetEspecialidadesTareasAsync(CancellationToken cancellationToken = default);
        Task<EspecialidadTareaDTO> GetEspecialidadTareaAsync(int IdTarea);
        Task AgregarEspecialidadTareaAsync(EspecialidadTareaDTO dto);
        Task ActualizarEspecialidadTareaAsync(EspecialidadTareaDTO dto);
        Task EliminarEspecialidadTareaAsync(int IdTarea);
        Task<List<EspecialidadSeccionDTO>> GetSeccionesObrasComplementariasAsync(CancellationToken cancellationToken = default);
    }
}
