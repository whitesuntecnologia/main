using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System.Linq;
using System.Threading.Tasks;
using UnitOfWork;

namespace Repository
{
    public class EmpresasBalancesGeneralesRepository : BaseRepository<EmpresasBalancesGenerales>
    {
        public EmpresasBalancesGeneralesRepository(IUnitOfWork unit) : base(unit)
        {
        }
    }
}