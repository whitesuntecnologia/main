using DataAccess.Entities;
using DataAccess.EntitiesCustom;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork;

namespace Repository
{
    public class TramitesNotificacionesRepository : BaseRepository<TramitesNotificaciones>
    {
        public TramitesNotificacionesRepository(IUnitOfWork unit) : base(unit)
        {
        }

        public IQueryable<TramitesNotificaciones> GetNotificacionPorUsuario(string userid)
        {
            var domains = (from t in Context.Tramites
                           join n in Context.TramitesNotificaciones on t.IdTramite equals n.IdTramite
                           where t.CreateUser == userid
                           select n);

            return domains;
        }
    }
}

