using DataAccess.Entities;
using DataAccess.Extends;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnitOfWork;

namespace Repository
{
    public class CombosRepository : BaseRepository
    {
        public CombosRepository(IUnitOfWork unit) : base(unit)
        {
        }

        public async Task<IEnumerable<GenericComboEntity>> GetEspecialidadesAsync(CancellationToken cancellationToken = default)
        {
            return await Context.Especialidades
                .Select(x =>
                    new GenericComboEntity
                    {
                        Id = x.IdEspecialidad,
                        Descripcion = "RAMA " + x.Rama + " - " + x.NombreEspecialidad
                    })
                .OrderBy(o=> o.Descripcion)
                .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<GenericComboEntity>> GetEspecialidadesFromTramiteAsync(int IdTramite,CancellationToken cancellationToken = default)
        {
            return await Context.TramitesEspecialidades
                .Where(x=> x.IdTramite == IdTramite)
                .Select(x =>
                    new GenericComboEntity
                    {
                        Id = x.IdTramiteEspecialidad,
                        Descripcion = "RAMA " + x.IdEspecialidadNavigation.Rama + " - " + x.IdEspecialidadNavigation.NombreEspecialidad
                    })
                .OrderBy(o => o.Descripcion)
                .ToListAsync(cancellationToken);
        }
        
        public async Task<IEnumerable<GenericComboEntity>> GetSeccionesFromEspecialidadTramiteAsync(int IdTramiteEspecialidad, CancellationToken cancellationToken = default)
        {
            return await Context.TramitesEspecialidadesSecciones
                .Where(x => x.IdTramiteEspecialidad == IdTramiteEspecialidad)
                .Select(x =>
                    new GenericComboEntity
                    {
                        Id = x.IdTramiteEspecialidadSeccion,
                        Descripcion = x.IdSeccionNavigation.DescripcionSeccion
                    })
                .OrderBy(o => o.Descripcion)
                .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<GenericComboEntity>> GetTiposDeObraAsync(CancellationToken cancellationToken = default)
        {
            return await Context.TiposDeObras
                .Select(x =>
                    new GenericComboEntity
                    {
                        Id = x.IdTipoObra,
                        Descripcion = x.Nombre
                    })
                .OrderBy(o => o.Descripcion)
                .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<GenericComboEntity>> GetTareasByEspecielidadAsync(int IdEspecialidad, CancellationToken cancellationToken = default)
        {
            return await Context.EspecialidadesTareas
                .Where(x=> x.IdSeccionNavigation.IdEspecialidad == IdEspecialidad)
                .Select(x =>
                    new GenericComboEntity
                    {
                        Id = x.IdTarea,
                        Descripcion = x.DescripcionTarea 
                    })
                .OrderBy(o => o.Descripcion)
                .ToListAsync(cancellationToken);
        }
        
        public async Task<IEnumerable<GenericComboEntity>> GetTareasBySeccionAsync(int IdSeccion, CancellationToken cancellationToken = default)
        {
            return await Context.EspecialidadesTareas
                .Where(x => x.IdSeccion == IdSeccion)
                .Select(x =>
                    new GenericComboEntity
                    {
                        Id = x.IdTarea,
                        Descripcion = x.DescripcionTarea
                    })
                .OrderBy(o => o.Descripcion)
                .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<GenericComboEntity>> GetProvinciasAsync(CancellationToken cancellationToken = default)
        {
            return await Context.Provincias
                .Select(x =>
                    new GenericComboEntity
                    {
                        Id = x.IdProvincia,
                        Descripcion = x.Descripcion
                    })
                .OrderBy(o => o.Descripcion)
                .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<GenericComboEntity>> GetTiposDeDocumentosAsync(CancellationToken cancellationToken = default)
        {
            return await Context.TiposDeDocumentos
                .Select(x =>
                    new GenericComboEntity
                    {
                        Id = x.IdTipoDocumento,
                        Descripcion = x.Descripcion
                    })
                .OrderBy(o => o.Descripcion)
                .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<GenericComboEntity>> GetPerfilesAsync(CancellationToken cancellationToken = default)
        {
            return await Context.Perfiles
                .Select(x =>
                    new GenericComboEntity
                    {
                        Id = x.IdPerfil,
                        Descripcion = x.Nombre
                    })
                .OrderBy(o => o.Descripcion)
                .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<GenericComboEntity>> GetSituacionesBcraAsync(CancellationToken cancellationToken = default)
        {
            return await Context.SituacionesBcra
                .Select(x =>
                    new GenericComboEntity
                    {
                        Id = x.IdsituacionBcra,
                        Descripcion = x.Nombre
                    })
                .OrderBy(o => o.Descripcion)
                .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<GenericComboEntity>> GetEspecialidadesSeccionesAsync(CancellationToken cancellationToken = default)
        {
            return await Context.EspecialidadesSecciones
                .Select(x =>
                    new GenericComboEntity
                    {
                        Id = x.IdSeccion,
                        Descripcion = x.DescripcionSeccion
                    })
                .OrderBy(o => o.Descripcion)
                .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<GenericComboEntity>> GetEspecialidadesSeccionesAsync(int IdEspecialidad,CancellationToken cancellationToken = default)
        {
            IEnumerable<GenericComboEntity> result = null;

            result = await Context.EspecialidadesSecciones
                    .Where(x => x.IdEspecialidad == IdEspecialidad)
                .Select(x => new GenericComboEntity
                {
                    Id = x.IdSeccion,
                    Descripcion = x.DescripcionSeccion
                })
                .OrderBy(o => o.Descripcion)
                .ToListAsync(cancellationToken);

            return result;
            
        }
        public async Task<IEnumerable<GenericComboEntity>> GetEspecialidadesSeccionesFromRamaAybAsync(int IdTramite, int IdEspecialidad, CancellationToken cancellationToken = default)
        {
            IEnumerable<GenericComboEntity> result = null;

            // Cuando la Especialidad es de la RAMA C, tiene que traer para elegir solo las especialidaddes
            // ya ingresadas en el tramite correspondientes a las ramas A y B.
            string[] ramasBuscar = new string[] { "A", "B" };
            result = await (from tespsec in Context.TramitesEspecialidadesSecciones
                            join tesp in Context.TramitesEspecialidades on tespsec.IdTramiteEspecialidad equals tesp.IdTramiteEspecialidad
                            join esp in Context.Especialidades on tesp.IdEspecialidad equals esp.IdEspecialidad
                            join sec in Context.EspecialidadesSecciones on tespsec.IdSeccion equals sec.IdSeccion
                            where tesp.IdTramite == IdTramite && ramasBuscar.Contains(esp.Rama)
                            orderby sec.DescripcionSeccion
                            select new GenericComboEntity
                            {
                                Id = sec.IdSeccion,
                                Descripcion = sec.DescripcionSeccion
                            }
                            ).ToListAsync(cancellationToken);
            return result;

        }

        public async Task<IEnumerable<GenericComboEntity>> GetTareasBySeccionFromRamaAyBAsync(int IdTramite, int IdSeccion, CancellationToken cancellationToken = default)
        {

            IEnumerable<GenericComboEntity> result = null;

            // Cuando la Especialidad es de la RAMA C, tiene que traer para elegir solo las especialidaddes
            // ya ingresadas en el tramite correspondientes a las ramas A y B.
            string[] ramasBuscar = new string[] { "A", "B" };
            result = await (from tespsec in Context.TramitesEspecialidadesSecciones
                            join tesp in Context.TramitesEspecialidades on tespsec.IdTramiteEspecialidad equals tesp.IdTramiteEspecialidad
                            join ttar in Context.TramitesEspecialidadesTareas on tespsec.IdTramiteEspecialidadSeccion equals ttar.IdTramiteEspecialidadSeccion
                            join esp in Context.Especialidades on tesp.IdEspecialidad equals esp.IdEspecialidad
                            join tar in Context.EspecialidadesTareas on ttar.IdTarea equals tar.IdTarea
                            where tesp.IdTramite == IdTramite && ramasBuscar.Contains(esp.Rama) && tespsec.IdSeccion == IdSeccion
                            orderby tar.DescripcionTarea
                            select new GenericComboEntity
                            {
                                Id = tar.IdTarea,
                                Descripcion = tar.DescripcionTarea
                            }
                            ).Distinct().ToListAsync(cancellationToken);

            return result;
        }
        public async Task<IEnumerable<GenericComboEntity>> GetEstadosEvaluacionAsync(CancellationToken cancellationToken = default)
        {
            return await Context.EstadosEvaluacion
                .Select(x =>
                    new GenericComboEntity
                    {
                        Id = x.IdEstadoEvaluacion,
                        Descripcion = x.NombreEstadoEvaluacion
                    })
                .OrderBy(o => o.Descripcion)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<GenericComboEntity>> GetObrasPciaLP(bool soloActivas,string? CuitEmpresa = null, CancellationToken cancellationToken = default)
        {
            IQueryable<ObrasProvinciaLaPampa> q = null;

            if (soloActivas)
                q = Context.ObrasProvinciaLaPampa.Where(x => !x.BajaLogica);
            else
                q = Context.ObrasProvinciaLaPampa;

            if (!string.IsNullOrWhiteSpace(CuitEmpresa))
                q = q.Where(x => x.CuitEmpresa == CuitEmpresa);

            return await q.Select(x =>
                    new GenericComboEntity
                    {
                        Id = x.IdObraPciaLp,
                        Descripcion = x.ObraNombre + (x.EsAltaPorUsuario ? " (Empresa " + x.CuitEmpresa + ") " : " ( Expediente: " + x.Expediente + ")")
                    })
                .OrderBy(o => o.Descripcion)
                .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<GenericComboEntity>> GetObrasPciaLPParaLicitarAsync(CancellationToken cancellationToken = default)
        {
            IQueryable<ObrasProvinciaLaPampa> q = null;

            
            q = Context.ObrasProvinciaLaPampa.Where(x => !x.BajaLogica);
            
            return await q.Select(x =>
                    new GenericComboEntity
                    {
                        Id = x.IdObraPciaLp,
                        Descripcion = x.ObraNombre + " ( Expediente: " + x.Expediente + ")"
                    })
                .OrderBy(o => o.Descripcion)
                .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<GenericStringComboEntity>> GetEstadosObraAsync(CancellationToken cancellationToken = default)
        {
            return await Context.ObrasProvinciaLaPampa
              .Select(x =>
                  new GenericStringComboEntity
                  {
                      Id = x.EstadoObra,
                      Descripcion = x.EstadoObra
                  })
              .Distinct()
              .OrderBy(o => o.Descripcion)
              .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<GenericComboEntity>> GetTramitesEstadosAsync(CancellationToken cancellationToken = default)
        {
            return await Context.TramitesEstados
              .Select(x =>
                  new GenericComboEntity
                  {
                      Id = x.IdEstado,
                      Descripcion = x.NombreEstado
                  })
              .Distinct()
              .OrderBy(o => o.Descripcion)
              .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<GenericComboEntity>> GetEmpresasAsync(CancellationToken cancellationToken = default)
        {
            return await Context.Empresas
              .Select(x =>
                  new GenericComboEntity
                  {
                      Id = x.IdEmpresa,
                      Descripcion = x.RazonSocial + " (" + x.CuitEmpresa.ToString() + ")"
                  })
              .Distinct()
              .OrderBy(o => o.Descripcion)
              .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<GenericComboEntity>> GetTiposDeTramiteAsync(CancellationToken cancellationToken = default)
        {
            return await Context.TiposDeTramite
              .Select(x =>
                  new GenericComboEntity
                  {
                      Id = x.IdTipoTramite,
                      Descripcion = x.Descripcion
                  })
              .Distinct()
              .OrderBy(o => o.Descripcion)
              .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<GenericComboEntity>> GetTiposDeSociedadAsync(CancellationToken cancellationToken = default)
        {
            return await Context.TiposDeSociedad
              .Select(x =>
                  new GenericComboEntity
                  {
                      Id = x.IdTipoSociedad,
                      Descripcion = x.Descripcion
                  })
              .Distinct()
              .OrderBy(o => o.Descripcion)
              .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<GenericComboEntity>> GetTiposDeCaracterLegalAsync(CancellationToken cancellationToken = default)
        {
            return await Context.TiposDeCaracterLegal
              .Select(x =>
                  new GenericComboEntity
                  {
                      Id = x.IdTipoCaracter,
                      Descripcion = x.Descripcion
                  })
              .Distinct()
              .OrderBy(o => o.Descripcion)
              .ToListAsync(cancellationToken);
        }
    }
}
