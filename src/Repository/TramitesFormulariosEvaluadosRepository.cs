using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

using Repository.Interface;
using StaticClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using UnitOfWork;

namespace Repository
{
    public  class TramitesFormulariosEvaluadosRepository : BaseRepository<TramitesFormulariosEvaluados>
    {
        private readonly IUnitOfWork uow;

        public TramitesFormulariosEvaluadosRepository(IUnitOfWork unit) : base(unit)
        {
            this.uow = unit;
        }

        public async Task CopiarEvaluacionAnteriorAsync(int IdTramite, List<int> NrosFormulariosExceptuados = default)
        {
            var entityTramite = await uow.Context.Tramites.AsNoTracking().FirstOrDefaultAsync(x=> x.IdTramite == IdTramite);

            if (entityTramite == null)
                throw new ArgumentException("No se encontró el trámite especificado.");


            IQueryable<TramitesFormulariosEvaluados> UltimaEvaluacionTramiteAnterior = null;

            int NroNotificacion = await uow.Context.TramitesHistorialEstados
                                                       .Where(x => x.IdTramite == IdTramite
                                                               && x.CodEstadoNuevo == Constants.TramitesEstadosCodigos.EnEvaluacion)
                                                       .CountAsync();

            if (!entityTramite.TramitesFormulariosEvaluados.Any() && entityTramite.IdTramiteOrigen != null)
            {

                UltimaEvaluacionTramiteAnterior = (from t in uow.Context.TramitesFormulariosEvaluados
                                                       where t.IdTramite == entityTramite.IdTramiteOrigen
                                                       && t.NroNotificacion == (
                                                           uow.Context.TramitesFormulariosEvaluados
                                                                  .Where(x => x.IdTramite == t.IdTramite)
                                                                  .Max(x => x.NroNotificacion)
                                                       )
                                                       select t);
            }
            else if (entityTramite.TramitesFormulariosEvaluados.Any())
            {
               
                UltimaEvaluacionTramiteAnterior = (from t in uow.Context.TramitesFormulariosEvaluados
                                                   where t.IdTramite == IdTramite
                                                   && t.NroNotificacion == NroNotificacion
                                                   select t);

                
            }

            if(UltimaEvaluacionTramiteAnterior != null && await UltimaEvaluacionTramiteAnterior.AnyAsync())
            {
                if (NrosFormulariosExceptuados != null && NrosFormulariosExceptuados.Any())
                {
                    UltimaEvaluacionTramiteAnterior = UltimaEvaluacionTramiteAnterior
                                                        .Where(x => !NrosFormulariosExceptuados.Contains(x.NroFormulario));
                }
                
                await UltimaEvaluacionTramiteAnterior.ForEachAsync(f =>
                {
                    f.IdTramiteFormEvaluado = 0;
                    f.NroNotificacion = NroNotificacion + 1;
                    f.CreateDate = DateTime.Now;
                    f.CreateUser = entityTramite.CreateUser;
                    f.LastUpdateDate = null;
                    f.LastUpdateUser = null;
                    f.IdTramite = IdTramite;
                });

                await uow.Context.AddRangeAsync(UltimaEvaluacionTramiteAnterior.OrderBy(o=> o.NroFormulario));

            }


        }
    }
}
