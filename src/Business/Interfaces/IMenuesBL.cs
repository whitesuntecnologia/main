using DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IMenuesBL
    {

        Task<List<MenuDTO>> GetMenuesAsync(int IdMenuPadre, CancellationToken cancellationToken = default);
        Task<List<MenuDTO>> GetMenuesAsync(CancellationToken cancellationToken = default);
    }
}
