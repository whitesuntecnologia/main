using Business;
using Business.Interfaces;
using DataAccess;
using DataTransferObject;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;
using Microsoft.JSInterop;
using Website.Models.Account;
using Website.Services;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;

namespace Website.Pages.Account
{
    public partial class LoginAdm: ComponentBase
    {
        [Inject] private AuthenticationStateProvider authenticationStateProvider { get; set; } = null!;
        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [Inject] private SignInManager<UserProfile> signInManager { get; set; } = null!;
        [Inject] private IUsuariosBL usuarioBL { get; set; } = null!;
        [Inject] private IJSRuntime jsRuntime { get; set; } = null!;
        

        public LoginModel loginModel { get; set; }= new();
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
                    return ;
                }

                var result = await signInManager.CheckPasswordSignInAsync(user, loginModel.Password, false);
                if (result.Succeeded)
                {
                    var customAuth = (CustomAuthenticateStateProvider)authenticationStateProvider;
                    var userSession = new UsuarioCookieDataDto()
                    {
                        Id= user.Id,
                        UserName = user.UserName ?? "",
                        NombreyApellido = user.NombreyApellido ?? "",
                        Email = user.Email ?? "",
                        IdEmpresa = null,
                        CuitEmpresa = null,
                        NombreEmpresa = null
                    };
                    //await customAuth.UpdateAuthenticationState(userSession);
                    navigationManager.NavigateTo("/", true);
                }
                else
                {
                    ErrorMessage = "Usuario y/o contraseña inválidos.";
                }
            }
        }
    }
}
