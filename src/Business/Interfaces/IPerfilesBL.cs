using DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IPerfilesBL
    {
        Task<List<PerfilDTO>> GetPerfilesAsync(CancellationToken cancellationToken = default);
        Task<PerfilDTO> GetPerfilAsync(int IdPerfil, CancellationToken cancellationToken = default);
        Task ActualizarPermisosAsync(int IdPerfil, List<int> Permisos);
        Task<List<PermisoDTO>> GetPermisosByPerfilAsync(int IdPerfil, CancellationToken cancellationToken = default);
        Task<List<PermisoDTO>> GetPermisosAsync(CancellationToken cancellationToken = default);
        Task<List<PermisoDTO>> GetPermisosAsync(string userid,CancellationToken cancellationToken = default);
    }
}
