using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IParametrosBL
    {
        Task<string> GetParametroCharAsync(string CodigoParametro);
        Task<decimal?> GetParametroNumAsync(string CodigoParametro);
        Task<T> GetSettingAsync<T>(string AppSettingKey);
    }
}
