using DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IIndiceConstruccionBL
    {
        Task<List<IndiceCostoConstruccionDTO>> GetIndicesAsync(CancellationToken cancellationToken = default);
        Task<IndiceCostoConstruccionDTO> GetIndiceAsync(int IdICC);
        Task AgregarIndiceAsync(IndiceCostoConstruccionDTO dto);
        Task ActualizarIndiceAsync(IndiceCostoConstruccionDTO dto);
        Task EliminarIndiceAsync(int IdIcc);
    }
}
