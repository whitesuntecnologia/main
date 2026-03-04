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
    public class EmpresasEspecialidadesRepository: BaseRepository<EmpresasEspecialidades>
    {
        public EmpresasEspecialidadesRepository(IUnitOfWork unit) : base(unit)
        {
            
        }
    }
}
