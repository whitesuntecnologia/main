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
    public class TramitesRepresentantesTecnicosRepository : BaseRepository<TramitesRepresentantesTecnicos>
    {
        public TramitesRepresentantesTecnicosRepository(IUnitOfWork unit) : base(unit)
        {
        }

        public IQueryable<InformeRepresentanteTecnicoCustom> GetInforme1(FiltroInformeResponsablesTecnicosDTO filtro)
        {

            var q = Context.TramitesRepresentantesTecnicos
                .GroupBy(rt => new {rt.Matricula, rt.Apellido, rt.Nombres, rt.Cuit, 
                                    rt.IdTramiteNavigation.IdEmpresaNavigation.RazonSocial, 
                                    rt.IdTramiteNavigation.IdEmpresa, 
                                    rt.FechaVencimientoMatricula, rt.FechaVencimientoContrato })
                .Select(grupo => new InformeRepresentanteTecnicoCustom
            {
                Apellido = grupo.Key.Apellido,
                Cuit = grupo.Key.Cuit,
                Nombres = grupo.Key.Nombres,
                RazonSocialEmpresa = grupo.Key.RazonSocial,
                FechaVencimientoMatricula = grupo.Key.FechaVencimientoMatricula,
                FechaVencimientoContrato = grupo.Key.FechaVencimientoContrato,
                Especialidades = Context.TramitesRepresentantesTecnicosEspecialidades
                                        .Where(x=> x.IdRepresentanteTecnicoNavigation.Cuit == grupo.Key.Cuit 
                                                && x.IdRepresentanteTecnicoNavigation.IdTramiteNavigation.IdEmpresa == grupo.Key.IdEmpresa )
                                        .Select(s=> s.IdTramiteEspecialidadNavigation.IdEspecialidadNavigation)
                                        .Distinct()
                                        .ToList(),
                Tramites = Context.TramitesRepresentantesTecnicos
                                        .Where(x=> x.Cuit == grupo.Key.Cuit
                                                && x.IdTramiteNavigation.IdEmpresa == grupo.Key.IdEmpresa)
                                        .Select(s=> s.IdTramiteNavigation)
                                        .ToList()
            });

            if (filtro.Cuit.HasValue)
                q = q.Where(x => x.Cuit == filtro.Cuit);

            if (!string.IsNullOrWhiteSpace(filtro.RazonSocial))
                q = q.Where(x => x.RazonSocialEmpresa.Contains(filtro.RazonSocial));

            if (!string.IsNullOrWhiteSpace(filtro.Apellido))
                q = q.Where(x => x.Apellido.Contains(filtro.Apellido));

            if (!string.IsNullOrWhiteSpace(filtro.Nombres))
                q = q.Where(x => x.Nombres.Contains(filtro.Nombres));

            if (filtro.FechaVencimientoMatriculaDesde.HasValue)
                q = q.Where(x => x.FechaVencimientoMatricula.Value >= filtro.FechaVencimientoMatriculaDesde.Value);

            if (filtro.FechaVencimientoMatriculaHasta.HasValue)
                q = q.Where(x => x.FechaVencimientoMatricula.Value <= filtro.FechaVencimientoMatriculaHasta);

            if (filtro.FechaVencimientoContratoDesde.HasValue)
                q = q.Where(x => x.FechaVencimientoContrato.Value >= filtro.FechaVencimientoContratoDesde.Value);

            if (filtro.FechaVencimientoContratoHasta.HasValue)
                q = q.Where(x => x.FechaVencimientoContrato.Value <= filtro.FechaVencimientoContratoHasta);
            
            return q;

        }
    }
}
