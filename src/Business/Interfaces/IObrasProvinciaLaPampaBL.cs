using DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IObrasProvinciaLaPampaBL
    {
        Task<List<ObrasProvinciaLaPampaDTO>> GetObrasAsync(CancellationToken cancellationToken = default);
        Task<ObrasProvinciaLaPampaDTO> GetObraAsync(int IdObraPciaLp);
        Task<List<ObrasProvinciaLaPampaDTO>> GetObrasAsync(string Expediente, string Nombre, decimal? CuitEmpresa, string Empresa, CancellationToken cancellationToken = default);
        Task<ObrasProvinciaLaPampaDTO> AgregarObraAsync(ObrasProvinciaLaPampaDTO dto);
        Task ActualizarObraAsync(ObrasProvinciaLaPampaDTO dto);
        Task ActualizarObraAsync(int IdObraPciaLp, int CoeficienteConceptual, decimal PorcentajeAvance);
        Task EliminarObraAsync(int IdObraPciaLp);
        Task<DateTime?> GetFechaUltimaActualizacionAsync();
        Task EjecutarActualizacionObrasAsync();
        Task<List<InformeObraDTO>> GetInformeObras1Async(FiltroInformeObraDTO filtro, CancellationToken cancellationToken = default);
    }
}
