using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System.Linq;
using System.Threading.Tasks;
using UnitOfWork;

namespace Repository
{
    public class EmpresasCapTecxEquipoRepository : BaseRepository<EmpresasCapTecxEquipo>
    {
        public EmpresasCapTecxEquipoRepository(IUnitOfWork unit) : base(unit)
        {
        }
    }
}