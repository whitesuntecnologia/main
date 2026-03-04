using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System.Linq;
using System.Threading.Tasks;
using UnitOfWork;

namespace Repository
{
    public class EmpresasDetalleObrasEjecucionRepository : BaseRepository<EmpresasDetalleObrasEjecucion>
    {
        public EmpresasDetalleObrasEjecucionRepository(IUnitOfWork unit) : base(unit)
        {
        }
    }
}