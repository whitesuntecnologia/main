using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System.Linq;
using System.Threading.Tasks;
using UnitOfWork;

namespace Repository
{
    public class EmpresasConstanciaCapRepository : BaseRepository<EmpresasConstanciaCap>
    {
        public EmpresasConstanciaCapRepository(IUnitOfWork unit) : base(unit)
        {
        }
    }
}