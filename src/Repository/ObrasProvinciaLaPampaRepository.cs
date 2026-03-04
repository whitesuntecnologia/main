using DataAccess.Entities;
using DataAccess.EntitiesCustom;
using DataTransferObject;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork;

namespace Repository
{
    public class ObrasProvinciaLaPampaRepository : BaseRepository<ObrasProvinciaLaPampa>
    {
        public ObrasProvinciaLaPampaRepository(IUnitOfWork unit) : base(unit)
        {
        }
        public IQueryable<InformeObraCustom> GetInforme1(FiltroInformeObraDTO filtro)
        {

            var q = Context.ObrasProvinciaLaPampa
                .Select(s => new InformeObraCustom
                {
                    Nombre = s.ObraNombre,
                    Expediente = s.Expediente,
                    CuitEmpresa = s.CuitEmpresa,
                    RazonSocialEmpresa = s.Empresa,
                    MontoObra = s.MontoObra,
                    PorcentajeAvance = s.PorcentajeAvanceObra
                });

            if (filtro.Cuit.HasValue)
                q = q.Where(x => x.CuitEmpresa == filtro.Cuit.ToString());

            if (!string.IsNullOrWhiteSpace(filtro.RazonSocial))
                q = q.Where(x => x.RazonSocialEmpresa.Contains(filtro.RazonSocial));

            if (!string.IsNullOrWhiteSpace(filtro.Nombre))
                q = q.Where(x => x.Nombre.Contains(filtro.Nombre));

            return q;

        }
    }
}