using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using StaticClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnitOfWork;


namespace Repository
{
    public class TramitesRepository : BaseRepository<Tramites>
    {
        public TramitesRepository(IUnitOfWork unit) : base(unit)
        {
        }

        public async Task ActualizarSinDatosForm(int IdTramite, int NroFormulario, bool value)
        {
            var entity = await this.Context.Tramites.FirstOrDefaultAsync(x=> x.IdTramite == IdTramite);
            if (entity != null) 
            { 
                switch(NroFormulario)
                {
                    case 8:
                        entity.SinDatosForm8 = value;
                        break;
                    case 10:
                        entity.SinDatosForm10 = value;
                        break;
                    case 11:
                        entity.SinDatosForm11 = value;
                        break;
                    case 12:
                        entity.SinDatosForm12 = value;
                        break;
                }

                await Context.SaveChangesAsync();
            }
        }

        public async Task<DateTime> GetFechaPresentacionAsync(int IdTramite, CancellationToken cancellationToken = default)
        {
            DateTime? result = await Context.TramitesHistorialEstados.Where(x => x.IdTramite == IdTramite && x.CodEstadoNuevo == Constants.TramitesEstadosCodigos.EnEvaluacion)
                                                    .OrderBy(o => o.CreateDate)
                                                    .Select(s => s.CreateDate)
                                                    .FirstOrDefaultAsync();
            
            if (result == null || result.Value.Year.Equals(1))
                result = DateTime.Today;
            
            return result.Value;
        }

    }
}
