using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork;

namespace Repository
{
    public class FilesRepository : BaseRepository<Files>
    {
        public FilesRepository(IUnitOfWork unit) : base(unit)
        {
        }

        public async Task<bool> ExistsFileAsync(string md5)
        {
            return await this.DbSet.CountAsync(x => x.Md5 == md5) > 0;
        }
        public async Task<int?> GetIdFileAsync(string md5)
        {
            return await this.DbSet.Where(x => x.Md5 == md5).Select(s=> s.IdFile).FirstOrDefaultAsync();
        }
    }
}
