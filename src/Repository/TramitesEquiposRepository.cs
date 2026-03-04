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
    public class TramitesEquiposRepository : BaseRepository<TramitesEquipos>
    {
        public TramitesEquiposRepository(IUnitOfWork unit) : base(unit)
        {
        }
    }
}
