using Ardalis.GuardClauses;
using AutoMapper;
using Business.Interfaces;
using DataAccess.Entities;
using DataAccess.EntitiesCustom;
using DataTransferObject;
using DocumentFormat.OpenXml.Office.PowerPoint.Y2021.M06.Main;
using DocumentFormat.OpenXml.Vml.Office;
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
    public class ObrasProvinciaLaPampaBL: IObrasProvinciaLaPampaBL
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IMapper _mapper;
        private readonly IUsuariosBL _usuarioBL;

        public ObrasProvinciaLaPampaBL(IUnitOfWorkFactory uowFactory, IUsuariosBL usuarioBL)
        {
            _uowFactory = Guard.Against.Null(uowFactory, nameof(uowFactory));
            _usuarioBL = usuarioBL;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ObrasProvinciaLaPampa, ObrasProvinciaLaPampaDTO>().ReverseMap();
                cfg.CreateMap<InformeObraCustom, InformeObraDTO>().ReverseMap();
            });
            _mapper = config.CreateMapper();
        }

        public async Task<List<ObrasProvinciaLaPampaDTO>> GetObrasAsync(CancellationToken cancellationToken = default)
        {
            var repo = new ObrasProvinciaLaPampaRepository(_uowFactory.GetUnitOfWork());
            var elements = await repo.DbSet.ToListAsync(cancellationToken);
            return _mapper.Map<List<ObrasProvinciaLaPampaDTO>>(elements);
        }

        public async Task<List<ObrasProvinciaLaPampaDTO>> GetObrasAsync(string Expediente, string Nombre, decimal? CuitEmpresa, string Empresa, CancellationToken cancellationToken = default)
        {
            var repo = new ObrasProvinciaLaPampaRepository(_uowFactory.GetUnitOfWork());
            
            var q = repo.DbSet.AsQueryable();
            if (!string.IsNullOrWhiteSpace(Expediente))
                q = q.Where(x => x.Expediente.Contains(Expediente));

            if (!string.IsNullOrWhiteSpace(Nombre))
                q = q.Where(x => x.ObraNombre.Contains(Nombre));

            if (CuitEmpresa.HasValue)
                q = q.Where(x => x.CuitEmpresa == CuitEmpresa.GetValueOrDefault().ToString());

            if (!string.IsNullOrWhiteSpace(Empresa))
                q = q.Where(x => x.Empresa.Contains(Empresa));


            var elements = await q.ToListAsync(cancellationToken);
            return _mapper.Map<List<ObrasProvinciaLaPampaDTO>>(elements);
        }
        public async Task<ObrasProvinciaLaPampaDTO> GetObraAsync(int IdObraPciaLp)
        {
            var repo = new ObrasProvinciaLaPampaRepository(_uowFactory.GetUnitOfWork());
            var element = await repo.FirstOrDefaultAsync(x => x.IdObraPciaLp == IdObraPciaLp);
            return _mapper.Map<ObrasProvinciaLaPampaDTO>(element);
        }
        public async Task<ObrasProvinciaLaPampaDTO> AgregarObraAsync(ObrasProvinciaLaPampaDTO dto)
        {
            string userid = await _usuarioBL.GetCurrentUserid();

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new ObrasProvinciaLaPampaRepository(uow);
                    ObrasProvinciaLaPampa entity = null;
                    if (!string.IsNullOrWhiteSpace(dto.Expediente))
                    {
                        //Altas de obras desde el modulo de obras
                        entity = await repo.FirstOrDefaultAsync(x => x.Expediente == dto.Expediente);

                        if (entity != null)
                            throw new Exception($"Ya existe un registro de obra con el expediente {dto.Expediente}. No es posible agregar otro.");
                    }
                    else
                    {
                        //Altas de obras privadas desde el portal publico
                        entity = await repo.FirstOrDefaultAsync(x => x.ObraNombre.ToLower().Trim() == dto.ObraNombre.ToLower().Trim() 
                                                                     && x.CuitEmpresa == dto.CuitEmpresa);

                        if (entity != null)
                            throw new Exception($"Ya existe un registro de obra de la empresa {dto.CuitEmpresa} con la denominación: {dto.ObraNombre.ToUpper().Trim()}.");
                    }    

                    entity = _mapper.Map<ObrasProvinciaLaPampa>(dto);
                    entity.CreateDate = DateTime.Now;
                    entity.CreateUser = userid;
                    await repo.AddAsync(entity);
                    await uow.CommitAsync();

                    dto = _mapper.Map<ObrasProvinciaLaPampaDTO>(entity);
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }

            return dto;
        }

        public async Task ActualizarObraAsync(ObrasProvinciaLaPampaDTO dto)
        {
            string userid = await _usuarioBL.GetCurrentUserid();
            var repo = new ObrasProvinciaLaPampaRepository(_uowFactory.GetUnitOfWork());

            var EntityVal = await repo.FirstOrDefaultAsync(x => x.Expediente == dto.Expediente
                                                        && x.IdObraPciaLp != dto.IdObraPciaLp );


            if (EntityVal != null)
            {

                if(string.IsNullOrWhiteSpace(EntityVal.Expediente))
                {
                    if( EntityVal.ObraNombre.ToLower() ==  dto.ObraNombre.ToLower() )
                    {
                        throw new Exception($"Ya existe un registro sin expediente y con el mismo nombre de obra. No es posible realizar el cambio.");
                    }
                }
                else
                    throw new Exception($"Ya existe un registro con el Expediente {dto.Expediente}. No es posible realizar el cambio.");
            }

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    repo = new ObrasProvinciaLaPampaRepository(uow);

                    var entity = await repo.FirstOrDefaultAsync(x => x.IdObraPciaLp == dto.IdObraPciaLp);

                    _mapper.Map<ObrasProvinciaLaPampaDTO, ObrasProvinciaLaPampa>(dto, entity);
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
        public async Task ActualizarObraAsync(int IdObraPciaLp, int CoeficienteConceptual, decimal PorcentajeAvance)
        {
            string userid = await _usuarioBL.GetCurrentUserid();
            var repo = new ObrasProvinciaLaPampaRepository(_uowFactory.GetUnitOfWork());

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    repo = new ObrasProvinciaLaPampaRepository(uow);

                    var entity = await repo.FirstOrDefaultAsync(x => x.IdObraPciaLp == IdObraPciaLp);

                    entity.CoeficienteConceptual = CoeficienteConceptual;
                    entity.PorcentajeAvanceObra = PorcentajeAvance;
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
        public async Task EliminarObraAsync(int IdObraPciaLp)
        {
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new ObrasProvinciaLaPampaRepository(uow);

                    var entity = await repo.FirstOrDefaultAsync(x => x.IdObraPciaLp == IdObraPciaLp);
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
        public async Task<DateTime?> GetFechaUltimaActualizacionAsync()
        {
            DateTime? result = null;
            var repo = new ObrasProvinciaLaPampaProcessInfoRepository(_uowFactory.GetUnitOfWork());

            var element = await repo.DbSet.FirstOrDefaultAsync();
            if(element != null )
            {
                result = element.UltimaFechaActualizacion;
            }
            return result;
        }

        public async Task EjecutarActualizacionObrasAsync()
        {
            var repo = new ObrasProvinciaLaPampaRepository(_uowFactory.GetUnitOfWork());
            await repo.Context.Database.ExecuteSqlRawAsync("EXEC get_ExpedientesMop");
        }
        public async Task<List<InformeObraDTO>> GetInformeObras1Async(FiltroInformeObraDTO filtro, CancellationToken cancellationToken = default)
        {
            var repo = new ObrasProvinciaLaPampaRepository(_uowFactory.GetUnitOfWork());
            var elements = await repo.GetInforme1(filtro).ToListAsync(cancellationToken);
            return _mapper.Map<List<InformeObraDTO>>(elements);
        }


    }
}
