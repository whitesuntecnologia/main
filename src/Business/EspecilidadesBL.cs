using Ardalis.GuardClauses;
using AutoMapper;
using Business.Interfaces;
using DataAccess.Entities;
using DataAccess.Extends;
using DataTransferObject;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnitOfWork;

namespace Business
{
    public  class EspecilidadesBL: IEspecilidadesBL
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IMapper _mapper;
        private readonly IUsuariosBL _usuarioBL;
        public EspecilidadesBL(IUnitOfWorkFactory uowFactory, IUsuariosBL usuarioBL)
        {
            _uowFactory = Guard.Against.Null(uowFactory, nameof(uowFactory));
            _usuarioBL = usuarioBL;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EspecialidadesEquipos, EspecialidadEquipoDTO>().ReverseMap();
                cfg.CreateMap<EspecialidadesSecciones, EspecialidadSeccionDTO>()
                    .ForMember(dest => dest.DescripcionEspecialidad, opt => opt.MapFrom(src => src.IdEspecialidadNavigation.NombreEspecialidad));
                cfg.CreateMap<EspecialidadSeccionDTO, EspecialidadesSecciones >();
                cfg.CreateMap<Especialidades, EspecialidadDTO>().ReverseMap();
                cfg.CreateMap<EspecialidadesTareas, EspecialidadTareaDTO>()
                    .ForMember(dest => dest.DescripcionSeccion, opt => opt.MapFrom(src => src.IdSeccionNavigation.DescripcionSeccion));
                cfg.CreateMap<EspecialidadTareaDTO, EspecialidadesTareas>();
            });
            _mapper = config.CreateMapper();
        }

        #region Equipos
        public async Task<List<EspecialidadEquipoDTO>> GetEspecialidadesEquiposAsync(CancellationToken cancellationToken = default)
        {
            var repo = new EspecialidadesEquiposRepository(_uowFactory.GetUnitOfWork());
            var elements = await repo.DbSet.ToListAsync(cancellationToken);
            return _mapper.Map<List<EspecialidadesEquipos>, List<EspecialidadEquipoDTO>>(elements);
        }
        public async Task<EspecialidadEquipoDTO> GetEspecialidadEquipoAsync(int IdEquipo)
        {
            var repo = new EspecialidadesEquiposRepository(_uowFactory.GetUnitOfWork());
            var element = await repo.FirstOrDefaultAsync(x => x.IdEquipo == IdEquipo);
            return _mapper.Map<EspecialidadesEquipos, EspecialidadEquipoDTO>(element);
        }
        public async Task AgregarEquipoAsync(EspecialidadEquipoDTO dto)
        {
            string userid = await _usuarioBL.GetCurrentUserid();

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new EspecialidadesEquiposRepository(uow);

                    var entity = await repo.FirstOrDefaultAsync(x => x.DescripcionEquipo == dto.DescripcionEquipo);

                    if (entity != null)
                        throw new Exception($"Ya existe un equipo con la misma descripcion.");

                    entity = _mapper.Map<EspecialidadEquipoDTO, EspecialidadesEquipos>(dto);
                    entity.CreateDate = DateTime.Now;
                    entity.CreateUser = userid;
                    await repo.AddAsync(entity);
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }
        public async Task ActualizarEquipoAsync(EspecialidadEquipoDTO dto)
        {
            string userid = await _usuarioBL.GetCurrentUserid();
            var repo = new EspecialidadesEquiposRepository(_uowFactory.GetUnitOfWork());

            var EntityVal = await repo.FirstOrDefaultAsync(x => x.DescripcionEquipo == dto.DescripcionEquipo
                                            && x.IdEquipo != dto.IdEquipo);

            if (EntityVal != null)
            {
                throw new Exception($"Ya existe un registro con la misma descripción. No es posible realizar el cambio.");
            }

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    repo = new EspecialidadesEquiposRepository(uow);

                    var entity = await repo.FirstOrDefaultAsync(x => x.IdEquipo == dto.IdEquipo);

                    _mapper.Map<EspecialidadEquipoDTO, EspecialidadesEquipos>(dto, entity);
                    entity.LastUpdateDate = DateTime.Now;
                    entity.LastUpdateUser = userid;
                    await repo.UpdateAsync(entity);
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }
        public async Task EliminarEquipoAsync(int IdEquipo)
        {
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new EspecialidadesEquiposRepository(uow);

                    var entity = await repo.FirstOrDefaultAsync(x => x.IdEquipo == IdEquipo);
                    await repo.RemoveAsync(entity);
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }

            }
        }
        #endregion

        #region Secciones
        public async Task<List<EspecialidadSeccionDTO>> GetEspecialidadesSeccionesAsync(CancellationToken cancellationToken = default)
        {
            var repo = new EspecialidadesSeccionesRepository(_uowFactory.GetUnitOfWork());
            var elements = await repo.DbSet.ToListAsync(cancellationToken);
            return _mapper.Map<List<EspecialidadesSecciones>, List<EspecialidadSeccionDTO>>(elements);
        }
        public async Task<List<EspecialidadSeccionDTO>> GetSeccionesObrasComplementariasAsync(CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var repo = new EspecialidadesSeccionesRepository(uow);
            var repoEspecialidades = new EspecialidadesRepository(uow);
            
            string[] ramasABuscar = new string[] { "A", "B" };
            var elements = await repo.DbSet
                                    .Where(x=> ramasABuscar.Contains(x.IdEspecialidadNavigation.Rama))
                                    .ToListAsync(cancellationToken);
            var entityObrasComplementarias = await repoEspecialidades.DbSet
                                    .FirstOrDefaultAsync(x => x.Rama == "C",cancellationToken);

            var result = _mapper.Map<List<EspecialidadesSecciones>, List<EspecialidadSeccionDTO>>(elements);
            result.ForEach(f => 
            {
                f.DescripcionSeccion = $"Obras Comp. {f.DescripcionSeccion}";
                f.IdEspecialidad = entityObrasComplementarias.IdEspecialidad;
            });
            return result;
        }
        public async Task<EspecialidadSeccionDTO> GetEspecialidadSeccionAsync(int IdSeccion)
        {
            var repo = new EspecialidadesSeccionesRepository(_uowFactory.GetUnitOfWork());
            var element = await repo.FirstOrDefaultAsync(x => x.IdSeccion == IdSeccion);
            return _mapper.Map<EspecialidadesSecciones, EspecialidadSeccionDTO>(element);
        }
        public async Task AgregarSeccionAsync(EspecialidadSeccionDTO dto)
        {
            string userid = await _usuarioBL.GetCurrentUserid();

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new EspecialidadesSeccionesRepository(uow);

                    var entity = await repo.FirstOrDefaultAsync(x => x.DescripcionSeccion == dto.DescripcionSeccion
                                                                    && x.IdEspecialidad == dto.IdEspecialidad);

                    if (entity != null)
                        throw new Exception($"Ya existe un registro con la misma descripcion.");

                    entity = _mapper.Map<EspecialidadSeccionDTO, EspecialidadesSecciones>(dto);
                    entity.CreateDate = DateTime.Now;
                    entity.CreateUser = userid;
                    await repo.AddAsync(entity);
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }
        public async Task ActualizarSeccionAsync(EspecialidadSeccionDTO dto)
        {
            string userid = await _usuarioBL.GetCurrentUserid();
            var repo = new EspecialidadesSeccionesRepository(_uowFactory.GetUnitOfWork());

            var EntityVal = await repo.FirstOrDefaultAsync(x => x.DescripcionSeccion == dto.DescripcionSeccion
                                                        && x.IdEspecialidad == dto.IdEspecialidad
                                                        && x.IdSeccion != dto.IdSeccion);

            if (EntityVal != null)
            {
                throw new Exception($"Ya existe un registro con la misma descripción y la misma especialidad. No es posible realizar el cambio.");
            }

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    repo = new EspecialidadesSeccionesRepository(uow);

                    var entity = await repo.FirstOrDefaultAsync(x => x.IdSeccion == dto.IdSeccion);

                    _mapper.Map<EspecialidadSeccionDTO, EspecialidadesSecciones>(dto, entity);
                    entity.LastUpdateDate = DateTime.Now;
                    entity.LastUpdateUser = userid;
                    await repo.UpdateAsync(entity);
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }
        public async Task EliminarSeccionAsync(int IdSeccion)
        {
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new EspecialidadesSeccionesRepository(uow);

                    var entity = await repo.FirstOrDefaultAsync(x => x.IdSeccion == IdSeccion);
                    await repo.RemoveAsync(entity);
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }

            }
        }
        #endregion
        
        #region Especialidades
        public async Task<List<EspecialidadDTO>> GetEspecialidadesAsync(CancellationToken cancellationToken = default)
        {
            
            var repo = new EspecialidadesRepository(_uowFactory.GetUnitOfWork());
            var elements = await repo.DbSet.ToListAsync(cancellationToken);
            var lst = _mapper.Map<List<Especialidades>, List<EspecialidadDTO>>(elements);

            return lst;
            
        }
        public async Task<EspecialidadDTO> GetEspecialidadAsync(int IdEspecialidad)
        {
            var repo = new EspecialidadesRepository(_uowFactory.GetUnitOfWork());
            var element = await repo.FirstOrDefaultAsync(x => x.IdEspecialidad== IdEspecialidad);
            return _mapper.Map<Especialidades, EspecialidadDTO>(element);
        }
        public async Task AgregarEspecialidadAsync(EspecialidadDTO dto)
        {
            string userid = await _usuarioBL.GetCurrentUserid();

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new EspecialidadesRepository(uow);

                    var entity = await repo.FirstOrDefaultAsync(x => x.NombreEspecialidad == dto.NombreEspecialidad);

                    if (entity != null)
                        throw new Exception($"Ya existe un registro con la misma descripcion.");

                    entity = _mapper.Map<EspecialidadDTO, Especialidades>(dto);
                    entity.CreateDate = DateTime.Now;
                    entity.CreateUser = userid;
                    await repo.AddAsync(entity);
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }
        public async Task ActualizarEspecialidadAsync(EspecialidadDTO dto)
        {
            string userid = await _usuarioBL.GetCurrentUserid();
            var repo = new EspecialidadesRepository(_uowFactory.GetUnitOfWork());

            var EntityVal = await repo.FirstOrDefaultAsync(x => x.NombreEspecialidad == dto.NombreEspecialidad
                                            && x.IdEspecialidad != dto.IdEspecialidad);

            if (EntityVal != null)
            {
                throw new Exception($"Ya existe un registro con la misma descripción. No es posible realizar el cambio.");
            }

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    repo = new EspecialidadesRepository(uow);

                    var entity = await repo.FirstOrDefaultAsync(x => x.IdEspecialidad== dto.IdEspecialidad);

                    _mapper.Map<EspecialidadDTO, Especialidades>(dto, entity);
                    entity.LastUpdateDate = DateTime.Now;
                    entity.LastUpdateUser = userid;
                    await repo.UpdateAsync(entity);
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }
        public async Task EliminarEspecialidadAsync(int IdEspecialidad)
        {
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new EspecialidadesRepository(uow);

                    var entity = await repo.FirstOrDefaultAsync(x => x.IdEspecialidad== IdEspecialidad);
                    await repo.RemoveAsync(entity);
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }

            }
        }
        #endregion

        #region Tareas
        public async Task<List<EspecialidadTareaDTO>> GetEspecialidadesTareasAsync(CancellationToken cancellationToken = default)
        {
            var repo = new EspecialidadesTareasRepository(_uowFactory.GetUnitOfWork());
            var elements = await repo.DbSet.ToListAsync(cancellationToken);
            return _mapper.Map<List<EspecialidadesTareas>, List<EspecialidadTareaDTO>>(elements);
        }
        public async Task<EspecialidadTareaDTO> GetEspecialidadTareaAsync(int IdTarea)
        {
            var repo = new EspecialidadesTareasRepository(_uowFactory.GetUnitOfWork());
            var element = await repo.FirstOrDefaultAsync(x => x.IdTarea == IdTarea);
            return _mapper.Map<EspecialidadesTareas, EspecialidadTareaDTO>(element);
        }
        public async Task AgregarEspecialidadTareaAsync(EspecialidadTareaDTO dto)
        {
            string userid = await _usuarioBL.GetCurrentUserid();

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new EspecialidadesTareasRepository(uow);

                    var entity = await repo.FirstOrDefaultAsync(x => x.DescripcionTarea == dto.DescripcionTarea
                                                                && x.IdSeccion == dto.IdSeccion
                                                                );

                    if (entity != null)
                        throw new Exception($"Ya existe un registro con la misma información.");

                    entity = _mapper.Map<EspecialidadTareaDTO, EspecialidadesTareas>(dto);
                    entity.CreateDate = DateTime.Now;
                    entity.CreateUser = userid;
                    await repo.AddAsync(entity);
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }
        public async Task ActualizarEspecialidadTareaAsync(EspecialidadTareaDTO dto)
        {
            string userid = await _usuarioBL.GetCurrentUserid();
            var repo = new EspecialidadesTareasRepository(_uowFactory.GetUnitOfWork());

            var EntityVal = await repo.FirstOrDefaultAsync(x => x.DescripcionTarea == dto.DescripcionTarea
                                                                && x.IdSeccion == dto.IdSeccion
                                                                && x.IdTarea!= dto.IdTarea);

            if (EntityVal != null)
            {
                throw new Exception($"Ya existe un registro con la misma información. No es posible realizar el cambio.");
            }

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    repo = new EspecialidadesTareasRepository(uow);

                    var entity = await repo.FirstOrDefaultAsync(x => x.IdTarea == dto.IdTarea);

                    _mapper.Map<EspecialidadTareaDTO, EspecialidadesTareas>(dto, entity);
                    entity.LastUpdateDate = DateTime.Now;
                    entity.LastUpdateUser = userid;
                    await repo.UpdateAsync(entity);
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }
        public async Task EliminarEspecialidadTareaAsync(int IdTarea)
        {
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new EspecialidadesTareasRepository(uow);

                    var entity = await repo.FirstOrDefaultAsync(x => x.IdTarea == IdTarea);
                    await repo.RemoveAsync(entity);
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }

            }
        }
        #endregion
    }
}
