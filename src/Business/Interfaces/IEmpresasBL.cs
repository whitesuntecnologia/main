using DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public  interface IEmpresasBL
    {
        Task<List<EmpresaDTO>> GetEmpresasAsync(CancellationToken cancellationToken = default);
        Task<EmpresaDTO> GetEmpresaAsync(int IdEmpresa);
        Task<List<InformeEmpresaDTO>> GetInformeEmpresasAsync(FiltroInformeEmpresasDTO filtro, CancellationToken cancellationToken = default);
        Task AgregarEmpresaAsync(EmpresaDTO dto);
        Task ActualizarEmpresaAsync(EmpresaDTO dto);
        Task EliminarEmpresaAsync(int IdEmpresa);
        Task<(List<InformeCapacidadesxEmpresaDtoFlat> datos, List<EspecialidadInfo> especialidades)> GetCapacidadesxEmpresaFlatAsync(FiltroCapacidadesxEmpresaDto filtro);
    }
}
