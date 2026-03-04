using DataAccess.Entities;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork;

namespace Repository
{
    public class PermisosRepository : BaseRepository<Permisos>
    {
        public PermisosRepository(IUnitOfWork unit) : base(unit)
        {
        }
        public IQueryable<Permisos> GetPermisosForUser(string userid)
        {
            var domains = (from rel in Context.RelUsuariosPerfiles
                           join pp in Context.PerfilesPermisos on rel.IdPerfil equals pp.IdPerfil
                           join per in Context.Permisos on pp.IdPermiso equals per.IdPermiso
                           where rel.Userid == userid
                           select per);
            return domains;
        }
    }
}