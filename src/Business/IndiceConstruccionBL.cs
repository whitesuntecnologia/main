using Ardalis.GuardClauses;
using AutoMapper;
using Business.Interfaces;
using DataAccess.Entities;
using DataTransferObject;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnitOfWork;

namespace Business
{
    public class IndiceConstruccionBL: IIndiceConstruccionBL
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IMapper _mapper;
        private readonly IUsuariosBL _usuarioBL;
        public IndiceConstruccionBL(IUnitOfWorkFactory uowFactory, IUsuariosBL usuarioBL)
        {
            _uowFactory = Guard.Against.Null(uowFactory, nameof(uowFactory));
            _usuarioBL = usuarioBL;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IndicesCostoConstruccion, IndiceCostoConstruccionDTO>().ReverseMap();
            });
            _mapper = config.CreateMapper();
        }
        public async Task<List<IndiceCostoConstruccionDTO>> GetIndicesAsync(CancellationToken cancellationToken = default)
        {
            var repo = new IndicesCostoConstruccionRepository(_uowFactory.GetUnitOfWork());
            var elements = await repo.DbSet.OrderBy(o=> o.Anio).ThenBy(o=> o.Mes).ToListAsync(cancellationToken);
            return _mapper.Map<List<IndicesCostoConstruccion>, List<IndiceCostoConstruccionDTO>>(elements);
        }

        public async Task<IndiceCostoConstruccionDTO> GetIndiceAsync(int IdICC)
        {
            var repo = new IndicesCostoConstruccionRepository(_uowFactory.GetUnitOfWork());
            var element = await repo.FirstOrDefaultAsync(x=> x.IdIcc == IdICC);
            return _mapper.Map<IndicesCostoConstruccion, IndiceCostoConstruccionDTO>(element);
        }

        public async Task AgregarIndiceAsync(IndiceCostoConstruccionDTO dto)
        {
            string userid = await _usuarioBL.GetCurrentUserid();

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new IndicesCostoConstruccionRepository(uow);

                    var entity = await repo.FirstOrDefaultAsync(x => x.Mes == dto.Mes && x.Anio == dto.Anio);

                    if (entity != null)
                        throw new Exception($"Ya existe un indice para el período indicado.");

                    entity = _mapper.Map<IndiceCostoConstruccionDTO, IndicesCostoConstruccion>(dto);
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

        public async Task ActualizarIndiceAsync(IndiceCostoConstruccionDTO dto)
        {
            string userid = await _usuarioBL.GetCurrentUserid();
            var repo = new IndicesCostoConstruccionRepository(_uowFactory.GetUnitOfWork());
            
            var EntityVal = await repo.FirstOrDefaultAsync(x => x.Anio == dto.Anio
                                            && x.Mes== dto.Mes
                                            && x.IdIcc != dto.IdIcc);

            if (EntityVal != null)
            {
                throw new Exception($"Ya existe un registro con el período indicado. No es posible realizar el cambio.");
            }

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    repo = new IndicesCostoConstruccionRepository(uow);

                    var entity = await repo.FirstOrDefaultAsync(x => x.IdIcc == dto.IdIcc);

                     _mapper.Map<IndiceCostoConstruccionDTO, IndicesCostoConstruccion>(dto, entity);
                    entity.CreateDate = DateTime.Now;
                    entity.CreateUser = userid;
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

        public async Task EliminarIndiceAsync(int IdIcc)
        {
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new IndicesCostoConstruccionRepository(uow);
                    
                    var entity = await repo.FirstOrDefaultAsync(x => x.IdIcc == IdIcc);
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
    }
}
