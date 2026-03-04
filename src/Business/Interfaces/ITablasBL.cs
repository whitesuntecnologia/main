using DataTransferObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface ITablasBL
    {
        Task<IEnumerable<ProvinciaDTO>> GetProvinciasAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<TiposDeDocumentoDTO>> GetTiposDeDocumentosAsync(int IdGrupoTramite, CancellationToken cancellationToken = default);
        Task<DataTable> Execute(string sql);
        Task<IEnumerable<TiposDeDocumentoDTO>> GetTiposDeDocumentosObligatoriosAsync(int IdGrupoTramite, CancellationToken cancellationToken = default);
    }
}
