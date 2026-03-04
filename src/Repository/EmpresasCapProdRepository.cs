using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System.Linq;
using System.Threading.Tasks;
using UnitOfWork;

namespace Repository
{
    public class EmpresasCapProdRepository : BaseRepository<EmpresasCapProd>
    {
        public EmpresasCapProdRepository(IUnitOfWork unit) : base(unit)
        {
        }
    }
}