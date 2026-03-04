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
    public class IndiceBancoCentralBL: IIndiceBancoCentralBL
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IMapper _mapper;
        private readonly IUsuariosBL _usuarioBL;

        public IndiceBancoCentralBL(IUnitOfWorkFactory uowFactory, IUsuariosBL usuarioBL)
        {
            _uowFactory = Guard.Against.Null(uowFactory, nameof(uowFactory));
            _usuarioBL = usuarioBL;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IndicesBancoCentral, IndiceBancoCentralDTO>()
                .ForMember(dest => dest.NombreSituacionBcra, opt => opt.MapFrom(src => src.IdSituacionBcraNavigation.Nombre))
                ;
                cfg.CreateMap<IndiceBancoCentralDTO,IndicesBancoCentral>();
            });
            _mapper = config.CreateMapper();
        }
        public async Task<List<IndiceBancoCentralDTO>> GetIndicesAsync(CancellationToken cancellationToken = default)
        {
            var repo = new IndicesBancoCentralRepository(_uowFactory.GetUnitOfWork());
            var elements = await repo.DbSet.ToListAsync(cancellationToken);
            return _mapper.Map<List<IndicesBancoCentral>, List<IndiceBancoCentralDTO>>(elements);
        }

        public async Task<IndiceBancoCentralDTO> GetIndiceAsync(int IdIndiceBcra)
        {
            var repo = new IndicesBancoCentralRepository(_uowFactory.GetUnitOfWork());
            var element = await repo.FirstOrDefaultAsync(x => x.IdIndiceBcra == IdIndiceBcra);
            return _mapper.Map<IndicesBancoCentral, IndiceBancoCentralDTO>(element);
        }

        public async Task AgregarIndiceAsync(IndiceBancoCentralDTO dto)
        {
            string userid = await _usuarioBL.GetCurrentUserid();

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new IndicesBancoCentralRepository(uow);

                    var entity = await repo.FirstOrDefaultAsync(x => x.IdSituacionBcra == dto.IdSituacionBcra);

                    if (entity != null)
                        throw new Exception($"Ya existe un registro con la situación {dto.IdSituacionBcra}. No es posible agregar otro.");

                    entity = _mapper.Map<IndiceBancoCentralDTO, IndicesBancoCentral>(dto);
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

        public async Task ActualizarIndiceAsync(IndiceBancoCentralDTO dto)
        {
            string userid = await _usuarioBL.GetCurrentUserid();
            var repo = new IndicesBancoCentralRepository(_uowFactory.GetUnitOfWork());

            var EntityVal = await repo.FirstOrDefaultAsync(x => x.IdSituacionBcra == dto.IdSituacionBcra
                                                        && x.IdIndiceBcra != dto.IdIndiceBcra);

            if (EntityVal != null)
            {
                throw new Exception($"Ya existe un registro con la situación {dto.IdSituacionBcra}. No es posible realizar el cambio.");
            }

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    repo = new IndicesBancoCentralRepository(uow);

                    var entity = await repo.FirstOrDefaultAsync(x => x.IdIndiceBcra == dto.IdIndiceBcra);

                    _mapper.Map<IndiceBancoCentralDTO, IndicesBancoCentral>(dto, entity);
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

        public async Task EliminarIndiceAsync(int IdIndiceBcra)
        {
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new IndicesBancoCentralRepository(uow);

                    var entity = await repo.FirstOrDefaultAsync(x => x.IdIndiceBcra == IdIndiceBcra);
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
