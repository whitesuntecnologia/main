using DataAccess;
using DataTransferObject;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IUsuariosBL
    {
        Task<string> GetCurrentIdEmpresa();
        Task<string> GetCurrentUserNameEmpresa();
        Task<string> GetCurrentNombreEmpresa();
        Task<string> GetCurrentUserName();
        Task<string> GetCurrentUserid();
        Task<string> GetCurrentNombreyApellido();
        Task<UserProfile> GetUserByNameAsync(string username);
        Task<List<UsuarioDTO>> GetUsersAsync();
        Task<List<UsuarioDTO>> GetUsersAsync(string username, int? IdPerfil, int? IdEstado);
        Task<UsuarioDTO> GetUserByIdAsync(string Id);
        Task<UsuarioDTO> AgregarUsuarioAsync(UsuarioDTO dto);
        Task<UsuarioDTO> ActualizarUsuarioAsync(UsuarioDTO dto);
        Task<List<PermisoDTO>> GetPermisosForUserAsync(string userid);
        Task<List<PerfilDTO>> GetPerfilesForUserAsync(string userid);
        Task<List<EmpresaDTO>> GetUsuariosVinculados(string Id);
        Task<string> SetTokenAsync(string userid, string tokenName = "Login");
        Task<string> GetTokenAsync(string userid, string tokenName = "Login");
        Task RemoveTokenAsync(string userid, string tokenName = "Login");
        Task<CookieDto> GetCookieAuthorization(string UserName, EmpresaDTO empresa = null);
    }
}
