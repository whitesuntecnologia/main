using DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IIndiceUnidadViviendaBL
    {
        Task<List<IndiceUnidadViviendaDTO>> GetIndicesAsync(CancellationToken cancellationToken = default);
        Task<IndiceUnidadViviendaDTO> GetIndiceAsync(int IdICC);
        Task<IndiceUnidadViviendaDTO> GetIndiceAsync(int Mes, int Anio);
        Task AgregarIndiceAsync(IndiceUnidadViviendaDTO dto);
        Task ActualizarIndiceAsync(IndiceUnidadViviendaDTO dto);
        Task EliminarIndiceAsync(int IdUvi);
    }
}
