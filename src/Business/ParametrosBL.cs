using Ardalis.GuardClauses;
using AutoMapper;
using Business.Interfaces;
using Microsoft.Extensions.Configuration;
using DataAccess.Entities;
using DataTransferObject.BLs;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork;

namespace Business
{
    public  class ParametrosBL: IParametrosBL
    {

        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IConfiguration _config;

        public ParametrosBL(IConfiguration config, IUnitOfWorkFactory uowFactory)
        {
            _uowFactory = uowFactory;
            _config = config;
        }
        public ParametrosBL(IUnitOfWorkFactory uowFactory)
        {
            _uowFactory = Guard.Against.Null(uowFactory, nameof(uowFactory));

            //var config = new MapperConfiguration(cfg =>
            //{
            //    cfg.CreateMap<Parametros, ProvinciaDTO>();

            //});
            //_mapper = config.CreateMapper();
        }
        public async Task<string> GetParametroCharAsync(string CodigoParametro)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var repo = new ParametrosRepository(uow);
            var entity = await repo.FirstOrDefaultAsync(x => x.CodigoParametro == CodigoParametro);
            
            return entity?.ValorChar;
        }
        public async Task<decimal?> GetParametroNumAsync(string CodigoParametro)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var repo = new ParametrosRepository(uow);
            var entity = await repo.FirstOrDefaultAsync(x => x.CodigoParametro == CodigoParametro);

            return entity?.ValorNum;
        }
        public Task<T> GetSettingAsync<T>(string AppSettingKey)
        {
            T result = default;
            if (_config["AppSettings:" + AppSettingKey] != null)
                result = (T)Convert.ChangeType(_config["AppSettings:" + AppSettingKey], typeof(T));

            return Task.FromResult(result);
        }

    }
}
