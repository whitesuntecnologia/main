using DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IIndiceBancoCentralBL
    {
        Task<List<IndiceBancoCentralDTO>> GetIndicesAsync(CancellationToken cancellationToken = default);
        Task<IndiceBancoCentralDTO> GetIndiceAsync(int IdIndiceBcra);
        Task AgregarIndiceAsync(IndiceBancoCentralDTO dto);
        Task ActualizarIndiceAsync(IndiceBancoCentralDTO dto);
        Task EliminarIndiceAsync(int IdSituacionBcra);
    }
}
