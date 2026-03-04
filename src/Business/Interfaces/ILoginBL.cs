using DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface ILoginBL
    {
        Task<string> GetUrlLogin();
        Task<UserInfoDto> GetUsuarioLogueado(string code, string state);
        Task<string> GetUrlLogout(string username, string UrlRedirect);
    }
}
