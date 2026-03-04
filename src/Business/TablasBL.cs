using Ardalis.GuardClauses;
using AutoMapper;
using Business.Interfaces;
using DataAccess.Entities;
using DataAccess.Extends;
using DataTransferObject;
using DataTransferObject.BLs;
using DocumentFormat.OpenXml.Office2013.Excel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnitOfWork;

namespace Business
{
    public class TablasBL: ITablasBL
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IMapper _mapper;

        public TablasBL(IUnitOfWorkFactory uowFactory)
        {
            _uowFactory = Guard.Against.Null(uowFactory, nameof(uowFactory));

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Provincias, ProvinciaDTO>();
                cfg.CreateMap<TiposDeDocumentos, TiposDeDocumentoDTO>();
                
            });
            _mapper = config.CreateMapper();
        }
        public async Task<IEnumerable<ProvinciaDTO>> GetProvinciasAsync(CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            ProvinciasRepository repo = new(uow);
            var elements = await repo.DbSet.ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<Provincias>, IEnumerable<ProvinciaDTO>>(elements);
        }

        public async Task<IEnumerable<TiposDeDocumentoDTO>> GetTiposDeDocumentosAsync(int IdGrupoTramite, CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            TiposDeDocumentosRepository repo = new(uow);
            var elements = await repo.DbSet.Where(x=> x.IdGrupoTramite == IdGrupoTramite).ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<TiposDeDocumentos>, IEnumerable<TiposDeDocumentoDTO>>(elements);
        }
        public async Task<IEnumerable<TiposDeDocumentoDTO>> GetTiposDeDocumentosObligatoriosAsync(int IdGrupoTramite,CancellationToken cancellationToken = default)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            TiposDeDocumentosRepository repo = new(uow);
            var elements = await repo.DbSet.Where(x=> x.Obligatorio && x.IdGrupoTramite == IdGrupoTramite).ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<TiposDeDocumentos>, IEnumerable<TiposDeDocumentoDTO>>(elements);
        }

        public Task<DataTable> Execute(string sql)
        {
            var uow = _uowFactory.GetUnitOfWork();
            var Cnn = uow.Context.Database.GetDbConnection();
            Cnn.Open();
            DbDataReader dataReader= null;
            using (var cmd = Cnn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                dataReader = cmd.ExecuteReader();
            }
            
            var dataTable = new DataTable();
            dataTable.Load(dataReader);
            Cnn.Close();

            return Task.FromResult(dataTable);
        }
    }
}
