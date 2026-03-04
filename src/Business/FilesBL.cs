using Ardalis.GuardClauses;
using AutoMapper;
using Business.Interfaces;
using DataAccess.Entities;
using DataTransferObject;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork;

namespace Business
{
    public class FilesBL : IFilesBL
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IMapper _mapper;

        public FilesBL(IUnitOfWorkFactory uowFactory)
        {
            _uowFactory = Guard.Against.Null(uowFactory, nameof(uowFactory));

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Files, FileDTO>().ReverseMap();
            });
            _mapper = config.CreateMapper();
        }
        public async Task<FileDTO> AddFileAsync(FileDTO file)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            
            FileDTO result = null;

            var repo = new FilesRepository(uow);
            file.Md5 = CreateMD5(file.ContentFile);

            if(!await repo.ExistsFileAsync(file.Md5))
            {
                var Entity = _mapper.Map<FileDTO, Files>(file);
                Entity.IdFile = 0;
                Entity = await repo.AddAsync(Entity);
                result = _mapper.Map<Files, FileDTO>(Entity);
            }
            else
            {
                int? IdFile = await repo.GetIdFileAsync(file.Md5);
                if (IdFile.HasValue)
                    file.IdFile = IdFile.Value;
                result = file;
            }

            return result;

        }
        public string CreateMD5(byte[] inputBytes)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return Convert.ToHexString(hashBytes); // .NET 5 +
            }
        }

        public Task<FileDTO> GetFileAsync(int IdFile)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var repo = new FilesRepository(uow);
            var Entity = repo.DbSet.FirstOrDefault(x => x.IdFile == IdFile);
            return Task.FromResult(_mapper.Map<Files, FileDTO>(Entity));
        }
        public Task<FileDTO> GetFileAsync(Guid rowid)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var repo = new FilesRepository(uow);
            var Entity = repo.DbSet.FirstOrDefault(x => x.Rowid == rowid);
            return Task.FromResult(_mapper.Map<Files, FileDTO>(Entity));
        }
        
    }
}
