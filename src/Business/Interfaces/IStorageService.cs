using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IStorageService
    {
        Task SaveAsync(string key, string value);
        Task<string?> GetAsync(string key);
    }
}
