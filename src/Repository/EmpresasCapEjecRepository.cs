using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System.Linq;
using System.Threading.Tasks;
using UnitOfWork;

namespace Repository
{
    public class EmpresasCapEjecRepository : BaseRepository<EmpresasCapEjec>
    {
        public EmpresasCapEjecRepository(IUnitOfWork unit) : base(unit)
        {
        }
    }
}