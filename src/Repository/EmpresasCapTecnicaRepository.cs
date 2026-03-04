using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System.Linq;
using System.Threading.Tasks;
using UnitOfWork;

namespace Repository
{
    public class EmpresasCapTecnicaRepository : BaseRepository<EmpresasCapTecnica>
    {
        public EmpresasCapTecnicaRepository(IUnitOfWork unit) : base(unit)
        {
        }
    }
}