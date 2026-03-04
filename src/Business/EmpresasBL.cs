using Ardalis.GuardClauses;
using AutoMapper;
using Business.Interfaces;
using DataAccess;
using DataAccess.Entities;
using DataAccess.EntitiesCustom;
using DataTransferObject;
using DataTransferObject.BLs;
using Microsoft.AspNetCore.Identity;
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
    public class EmpresasBL : IEmpresasBL
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IMapper _mapper;
        private readonly IUsuariosBL _usuarioBL;
        public EmpresasBL(IUnitOfWorkFactory uowFactory, IUsuariosBL usuarioBL, UserManager<UserProfile> userManager)
        {
            _uowFactory = Guard.Against.Null(uowFactory, nameof(uowFactory));
            _usuarioBL = usuarioBL;

            var config = new MapperConfiguration(cfg =>
            {
                #region EmpresaDto
                cfg.CreateMap<Empresas, EmpresaDTO>()
                    .ForMember(dest => dest.CuitRepresentante, opt => opt.MapFrom(src => userManager.Users
                                                                                .Where(x => x.Id == src.UseridRepresentante)
                                                                                .Select(s => s.UserName)
                                                                                .FirstOrDefault()
                                                                                ))
                    .ForMember(dest => dest.Deudas, opt => opt.MapFrom(src => src.EmpresasDeudas))
                    .ForMember(dest => dest.Especialidades, opt => opt.MapFrom(src => src.EmpresasEspecialidades))
                    .ForMember(dest => dest.RepresentantesTecnicos, opt => opt.MapFrom(src => src.EmpresasRepresentantesTecnicos))
                    .ForMember(dest => dest.Sanciones, opt => opt.MapFrom(src => src.EmpresasSanciones))
                ;
                cfg.CreateMap<EmpresasDeudas, EmpresaDeudaDto>();
                cfg.CreateMap<EmpresasEspecialidades, EmpresaEspecialidadDto>()
                    .ForMember(dest => dest.NombreEspecialidad, opt => opt.MapFrom(src => src.IdEspecialidadNavigation.NombreEspecialidad))
                    .ForMember(dest => dest.Rama, opt => opt.MapFrom(src => src.IdEspecialidadNavigation.Rama))
                    .ForMember(dest => dest.Baja, opt => opt.MapFrom(src => src.IdEspecialidadNavigation.Baja))
                    .ForMember(dest => dest.Secciones, opt => opt.MapFrom(src => src.EmpresasEspecialidadesSecciones))
                ;
                cfg.CreateMap<EmpresasEspecialidadesSecciones, EmpresaSeccionDto>()
                    .ForMember(dest => dest.DescripcionSeccion, opt => opt.MapFrom(src => src.IdSeccionNavigation.DescripcionSeccion))
                    .ForMember(dest => dest.Baja, opt => opt.MapFrom(src => src.IdSeccionNavigation.Baja))
                    .ForMember(dest => dest.Tareas, opt => opt.MapFrom(src => src.EmpresasEspecialidadesTareas))
                ;
                cfg.CreateMap<EmpresasEspecialidadesTareas, EmpresaTareaDto>()
                    .ForMember(dest => dest.DescripcionTarea, opt => opt.MapFrom(src => src.IdTareaNavigation.DescripcionTarea))
                    .ForMember(dest => dest.Baja, opt => opt.MapFrom(src => src.IdTareaNavigation.Baja))
                    .ForMember(dest => dest.Equipos, opt => opt.MapFrom(src => src.EmpresasEspecialidadesEquipos))
                ;
                cfg.CreateMap<EmpresasEspecialidadesEquipos, EmpresaEquipoDto>()
                    .ForMember(dest => dest.DescripcionEquipo, opt => opt.MapFrom(src => src.IdEquipoNavigation.DescripcionEquipo))
                    .ForMember(dest => dest.Baja, opt => opt.MapFrom(src => src.IdEquipoNavigation.Baja))
                ;

                cfg.CreateMap<EmpresasRepresentantesTecnicos, EmpresaRepresentanteTecnicoDto>();
                cfg.CreateMap<EmpresasSanciones, EmpresaSancionDTO>()
                .ForMember(dest => dest.FilenameSancion, opt => opt.MapFrom(src => src.IdFileSancionNavigation.FileName))
                ;
                #endregion

                cfg.CreateMap<EmpresaDTO, Empresas>();
                cfg.CreateMap<EmpresaSancionDTO, EmpresasSanciones>();

                cfg.CreateMap<Empresas, InformeEmpresaDTO>()
                    .ForMember(dest => dest.CantidadRepresentantesTecnicos, opt => opt.MapFrom(src => src.EmpresasEspecialidades.Count()))
                    .ForMember(dest => dest.FechaDeRegistro, opt => opt.MapFrom(src => src.FechaInscripcion))
                    .ForMember(dest => dest.FechaDeVencimiento, opt => opt.MapFrom(src => src.Vencimiento))
                    .ForMember(dest => dest.Especialidades, opt => opt.MapFrom(src => src.EmpresasEspecialidades))
                ;

                cfg.CreateMap<EmpresasEspecialidades, EspecialidadDTO>()
                    .ForMember(dest => dest.NombreEspecialidad, opt => opt.MapFrom(src => src.IdEspecialidadNavigation.NombreEspecialidad))
                    .ForMember(dest => dest.Rama, opt => opt.MapFrom(src => src.IdEspecialidadNavigation.Rama))
                ;

                cfg.CreateMap<Tramites, TramitesDTO>()
                .ForMember(dest => dest.CuitEmpresa, opt => opt.MapFrom(src => src.IdEmpresaNavigation.CuitEmpresa))
                .ForMember(dest => dest.RazonSocial, opt => opt.MapFrom(src => src.IdEmpresaNavigation.RazonSocial))
                ;
                cfg.CreateMap<Especialidades, EspecialidadDTO>();

            });
            _mapper = config.CreateMapper();
        }
        public async Task<List<EmpresaDTO>> GetEmpresasAsync(CancellationToken cancellationToken = default)
        {
            var repo = new EmpresasRepository(_uowFactory.GetUnitOfWork());
            var elements = await repo.DbSet.ToListAsync(cancellationToken);
            return _mapper.Map<List<EmpresaDTO>>(elements);
        }
        public async Task<List<InformeEmpresaDTO>> GetInformeEmpresasAsync(FiltroInformeEmpresasDTO filtro, CancellationToken cancellationToken = default)
        {
            var repo = new EmpresasRepository(_uowFactory.GetUnitOfWork());
            var elements = await repo.GetInformeEmpresasAsync(filtro, cancellationToken);
            return _mapper.Map<List<InformeEmpresaDTO>>(elements);
        }

        public async Task<EmpresaDTO> GetEmpresaAsync(int IdEmpresa)
        {
            var repo = new EmpresasRepository(_uowFactory.GetUnitOfWork());
            var element = await repo.FirstOrDefaultAsync(x => x.IdEmpresa == IdEmpresa);
            return _mapper.Map<EmpresaDTO>(element);
        }

        public async Task AgregarEmpresaAsync(EmpresaDTO dto)
        {
            string userid = await _usuarioBL.GetCurrentUserid();

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new EmpresasRepository(uow);

                    var entity = await repo.FirstOrDefaultAsync(x => x.CuitEmpresa == dto.CuitEmpresa);

                    if (entity != null)
                        throw new Exception($"Ya existe una Empresa con el CUIT indicado.");

                    entity = _mapper.Map<Empresas>(dto);
                    entity.CreateDate = DateTime.Now;
                    entity.CreateUser = userid;
                    await repo.AddAsync(entity);
                    await uow.CommitAsync();

                    // Guardar sanciones si existen
                    if (dto.Sanciones != null && dto.Sanciones.Any())
                    {
                        var repoSanciones = new EmpresasSancionesRepository(uow);
                        foreach (var sancionDto in dto.Sanciones)
                        {
                            var sancion = _mapper.Map<EmpresasSanciones>(sancionDto);
                            sancion.IdEmpresa = entity.IdEmpresa;
                            sancion.CreateDate = DateTime.Now;
                            sancion.CreateUser = userid;
                            await repoSanciones.AddAsync(sancion);
                        }
                        await uow.CommitAsync();
                    }
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }

        public async Task ActualizarEmpresaAsync(EmpresaDTO dto)
        {
            string userid = await _usuarioBL.GetCurrentUserid();
            var repo = new EmpresasRepository(_uowFactory.GetUnitOfWork());

            var EntityVal = await repo.FirstOrDefaultAsync(x => x.CuitEmpresa == dto.CuitEmpresa
                                            && x.IdEmpresa != dto.IdEmpresa);

            if (EntityVal != null)
            {
                throw new Exception($"Ya existe un registro con el CUIT indicado. No es posible realizar el cambio.");
            }

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    repo = new EmpresasRepository(uow);

                    var entity = await repo.FirstOrDefaultAsync(x => x.IdEmpresa == dto.IdEmpresa);

                    _mapper.Map<EmpresaDTO, Empresas>(dto, entity);
                    entity.LastUpdateDate = DateTime.Now;
                    entity.LastUpdateUser = userid;
                    await repo.UpdateAsync(entity);

                    // ===== Gestión de Sanciones =====
                    var repoSanciones = new EmpresasSancionesRepository(uow);

                    // Obtener sanciones existentes en la base de datos
                    var sancionesExistentes = await repoSanciones.Where(x => x.IdEmpresa == dto.IdEmpresa).ToListAsync();
                    var sancionesExistentesDict = sancionesExistentes.ToDictionary(s => s.IdEmpresaSancion);

                    // Obtener IDs de las sanciones que vienen del DTO
                    var sancionesNuevasIds = dto.Sanciones?.Where(s => s.IdEmpresaSancion > 0)
                                                          .Select(s => s.IdEmpresaSancion)
                                                          .ToHashSet() ?? new HashSet<int>();

                    // 1. BAJAS: Eliminar sanciones que ya no están en el DTO
                    foreach (var sancionExistente in sancionesExistentes)
                    {
                        if (!sancionesNuevasIds.Contains(sancionExistente.IdEmpresaSancion))
                        {
                            await repoSanciones.RemoveAsync(sancionExistente);
                        }
                    }

                    // 2. ALTAS Y ACTUALIZACIONES: Procesar las sanciones del DTO
                    if (dto.Sanciones != null && dto.Sanciones.Any())
                    {
                        foreach (var sancionDto in dto.Sanciones)
                        {
                            if (sancionDto.IdEmpresaSancion > 0)
                            {
                                // ACTUALIZACIÓN: La sanción ya existe
                                if (sancionesExistentesDict.TryGetValue(sancionDto.IdEmpresaSancion, out var sancionExistente))
                                {
                                    sancionExistente.Nombre = sancionDto.Nombre;
                                    sancionExistente.IdFileSancion = sancionDto.IdFileSancion;
                                    sancionExistente.FechaDesdeSancion = sancionDto.FechaDesdeSancion;
                                    sancionExistente.FechaHastaSancion = sancionDto.FechaHastaSancion;

                                    await repoSanciones.UpdateAsync(sancionExistente);
                                }
                            }
                            else
                            {
                                // ALTA: Nueva sanción
                                var sancion = _mapper.Map<EmpresasSanciones>(sancionDto);
                                sancion.IdEmpresa = entity.IdEmpresa;
                                sancion.CreateDate = DateTime.Now;
                                sancion.CreateUser = userid;
                                await repoSanciones.AddAsync(sancion);
                            }
                        }
                    }

                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }

        public async Task EliminarEmpresaAsync(int IdEmpresa)
        {
            var uof = _uowFactory.GetUnitOfWork();
            var repoTramites = new TramitesRepository(uof);

            if (await repoTramites.AnyAsync(x => x.IdEmpresa == IdEmpresa))
            {
                throw new ArgumentException("La Empresa no se puede eliminar ya que la misma fue utilizada en algún trámite.");
            }

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new EmpresasRepository(uow);

                    var entity = await repo.FirstOrDefaultAsync(x => x.IdEmpresa == IdEmpresa);
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
        public async Task<(List<InformeCapacidadesxEmpresaDtoFlat> datos, List<EspecialidadInfo> especialidades)> GetCapacidadesxEmpresaFlatAsync(FiltroCapacidadesxEmpresaDto filtro)
        {
            var repo = new EmpresasRepository(_uowFactory.GetUnitOfWork());
            return await repo.GetCapacidadesxEmpresaFlatAsync(filtro);
        }

    }
}
