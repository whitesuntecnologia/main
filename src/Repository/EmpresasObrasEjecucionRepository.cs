using DataAccess.Entities;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork;

namespace Repository
{
    public class EmpresasObrasEjecucionRepository : BaseRepository<EmpresasObrasEjecucion>
    {
        public EmpresasObrasEjecucionRepository(IUnitOfWork unit) : base(unit)
        {
        }
    }
}