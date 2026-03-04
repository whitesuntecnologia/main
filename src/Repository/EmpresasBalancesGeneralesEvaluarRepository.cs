using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System.Linq;
using System.Threading.Tasks;
using UnitOfWork;

namespace Repository
{
    public class EmpresasBalancesGeneralesEvaluarRepository : BaseRepository<EmpresasBalancesGeneralesEvaluar>
    {
        public EmpresasBalancesGeneralesEvaluarRepository(IUnitOfWork unit) : base(unit)
        {
        }
    }
}