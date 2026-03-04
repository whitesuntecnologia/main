using Ardalis.GuardClauses;
using AutoMapper;
using Business.Interfaces;
using DataAccess.Entities;
using DataTransferObject;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnitOfWork;

namespace Business
{
    public class PerfilesBL: IPerfilesBL
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IMapper _mapper;
        private readonly IUsuariosBL _usuarioBL;


        public PerfilesBL(IUnitOfWorkFactory uowFactory, IUsuariosBL usuarioBL)
        {
            _uowFactory = Guard.Against.Null(uowFactory, nameof(uowFactory));
            _usuarioBL = usuarioBL;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Perfiles, PerfilDTO>();
                cfg.CreateMap<Permisos, PermisoDTO>()
                    .ForMember(dest=> dest.Menues, opt => opt.MapFrom( src => src.MenuesPermisos.Select(s=> s.IdMenu).ToList()))
                ;

            });
            _mapper = config.CreateMapper();
        }

        public async Task<List<PerfilDTO>> GetPerfilesAsync(CancellationToken cancellationToken = default)
        {
            var repo = new PerfilesRepository(_uowFactory.GetUnitOfWork());
            var elements = await repo.DbSet.ToListAsync(cancellationToken);
            return _mapper.Map<List<Perfiles>, List<PerfilDTO>>(elements.OrderBy(x=> x.IdPerfil).ToList());
        }
        public async Task<PerfilDTO> GetPerfilAsync(int IdPerfil, CancellationToken cancellationToken = default)
        {
            var repo = new PerfilesRepository(_uowFactory.GetUnitOfWork());
            var element = await repo.DbSet.FirstOrDefaultAsync(x=> x.IdPerfil == IdPerfil, cancellationToken);
            return _mapper.Map<Perfiles, PerfilDTO>(element);
        }
        public async Task ActualizarPermisosAsync(int IdPerfil, List<int> Permisos)
        {
            string userid = await _usuarioBL.GetCurrentUserid();
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new PerfilesPermisosRepository(uow);

                    var PermisosEntity = await repo.Where(x=>x.IdPerfil == IdPerfil).ToListAsync();
                    await repo.RemoveRangeAsync(PermisosEntity);

                    PermisosEntity.Clear();
                    PermisosEntity.AddRange(Permisos.Select(s => new PerfilesPermisos()
                    {
                        IdPerfil = IdPerfil,
                        IdPermiso = s,
                    }).ToList());

                    await repo.AddARangeAsync(PermisosEntity);
                    await uow.CommitAsync();

                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }

        public async Task<List<PermisoDTO>> GetPermisosByPerfilAsync(int IdPerfil, CancellationToken cancellationToken = default)
        {
            var repo = new PerfilesRepository(_uowFactory.GetUnitOfWork());
            var element = await repo.FirstOrDefaultAsync(x=> x.IdPerfil == IdPerfil);
            var permisos = element.PerfilesPermisos.Select(s => s.IdPermisoNavigation).ToList();
            
            return _mapper.Map<List<Permisos>, List<PermisoDTO>>(permisos);
        }
        public async Task<List<PermisoDTO>> GetPermisosAsync(CancellationToken cancellationToken = default)
        {
            var repo = new MenuesPermisosRepository(_uowFactory.GetUnitOfWork());
            var permisos = await repo.DbSet.Select(s=> s.IdPermisoNavigation).ToListAsync();
            return _mapper.Map<List<Permisos>, List<PermisoDTO>>(permisos);
        }
        public async Task<List<PermisoDTO>> GetPermisosAsync(string userid, CancellationToken cancellationToken = default)
        {
            var uof = _uowFactory.GetUnitOfWork();
            var repo = new RelUsuariosPerfilesRepository(uof);
            var repoPerfilesPermisos = new PerfilesPermisosRepository(uof);

            var lstPerfilesAsignados = repo.Where(x=> x.Userid == userid).Select(s=> s.IdPerfil).ToList();
            var permisos = await repoPerfilesPermisos.Where(x => lstPerfilesAsignados.Contains(x.IdPerfil)).Select(s => s.IdPermisoNavigation).Distinct().ToListAsync();

            return _mapper.Map<List<Permisos>, List<PermisoDTO>>(permisos);
        }

    }
}
