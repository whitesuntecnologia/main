using DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IFilesBL
    {
        Task<FileDTO> AddFileAsync(FileDTO file);
        Task<FileDTO> GetFileAsync(int IdFile);
        Task<FileDTO> GetFileAsync(Guid rowid);
        public string CreateMD5(byte[] inputBytes);
    }
}
