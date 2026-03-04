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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnitOfWork;

namespace Business
{
    public class IndiceUnidadViviendaBL: IIndiceUnidadViviendaBL
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IMapper _mapper;
        private readonly IUsuariosBL _usuarioBL;
        public IndiceUnidadViviendaBL(IUnitOfWorkFactory uowFactory, IUsuariosBL usuarioBL)
        {
            _uowFactory = Guard.Against.Null(uowFactory, nameof(uowFactory));
            _usuarioBL = usuarioBL;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IndicesUnidadVivienda, IndiceUnidadViviendaDTO>().ReverseMap();
            });
            _mapper = config.CreateMapper();
        }
        public async Task<List<IndiceUnidadViviendaDTO>> GetIndicesAsync(CancellationToken cancellationToken = default)
        {
            var repo = new IndicesUnidadViviendaRepository(_uowFactory.GetUnitOfWork());
            var elements = await repo.DbSet.OrderBy(o => o.Anio).ThenBy(o => o.Mes).ToListAsync(cancellationToken);
            return _mapper.Map<List<IndicesUnidadVivienda>, List<IndiceUnidadViviendaDTO>>(elements);
        }

        public async Task<IndiceUnidadViviendaDTO> GetIndiceAsync(int IdUvi)
        {
            var repo = new IndicesUnidadViviendaRepository(_uowFactory.GetUnitOfWork());
            var element = await repo.FirstOrDefaultAsync(x => x.IdUvi == IdUvi);
            return _mapper.Map<IndicesUnidadVivienda, IndiceUnidadViviendaDTO>(element);
        }
        public async Task<IndiceUnidadViviendaDTO> GetIndiceAsync(int Mes, int Anio)
        {
            var repo = new IndicesUnidadViviendaRepository(_uowFactory.GetUnitOfWork());
            var element = await repo.FirstOrDefaultAsync(x => x.Mes == Mes && x.Anio == Anio);
            return _mapper.Map<IndicesUnidadVivienda, IndiceUnidadViviendaDTO>(element);
        }
        public async Task AgregarIndiceAsync(IndiceUnidadViviendaDTO dto)
        {
            string userid = await _usuarioBL.GetCurrentUserid();

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new IndicesUnidadViviendaRepository(uow);

                    var entity = await repo.FirstOrDefaultAsync(x => x.Mes == dto.Mes && x.Anio == dto.Anio);

                    if (entity != null)
                        throw new Exception($"Ya existe un indice para el período indicado.");

                    entity = _mapper.Map<IndiceUnidadViviendaDTO, IndicesUnidadVivienda>(dto);
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

        public async Task ActualizarIndiceAsync(IndiceUnidadViviendaDTO dto)
        {
            string userid = await _usuarioBL.GetCurrentUserid();
            var repo = new IndicesUnidadViviendaRepository(_uowFactory.GetUnitOfWork());

            var EntityVal = await repo.FirstOrDefaultAsync(x => x.Anio == dto.Anio
                                            && x.Mes == dto.Mes
                                            && x.IdUvi != dto.IdUvi);

            if (EntityVal != null)
            {
                throw new Exception($"Ya existe un registro con el período indicado. No es posible realizar el cambio.");
            }

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    repo = new IndicesUnidadViviendaRepository(uow);

                    var entity = await repo.FirstOrDefaultAsync(x => x.IdUvi == dto.IdUvi);

                    _mapper.Map<IndiceUnidadViviendaDTO, IndicesUnidadVivienda>(dto, entity);
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

        public async Task EliminarIndiceAsync(int IdUvi)
        {
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new IndicesUnidadViviendaRepository(uow);

                    var entity = await repo.FirstOrDefaultAsync(x => x.IdUvi == IdUvi);
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
