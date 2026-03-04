using Business;
using Business.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using StaticClass;
using Website.Services;

namespace Website.Pages.Account
{
    public partial class Logout: ComponentBase
    {
        [Inject] AuthenticationStateProvider authenticationStateProvider { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private ILoginBL loginBL { get; set; } = null!;
        [Inject] private IUsuariosBL usuarioBL { get; set; } = null!;
        [Inject] private ProtectedLocalStorage _localStorage { get; set; } = null!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                var username = await usuarioBL.GetCurrentUserName();

                //Elimina la cookie de autorizacion del localsotrage
                await _localStorage.DeleteAsync(Constants.LocalStorageAuthKey);
                await _localStorage.DeleteAsync("Expiration");

                if (!string.IsNullOrWhiteSpace(username))
                {
                    //Obtiene la Url de la pantalla de login para mandarla al logout del SUU
                    string UrlLogin = $"{navigationManager.BaseUri}Account/Login";
                    //Obtiene la url de deslogueo del SUU
                    string urlLogOut = await loginBL.GetUrlLogout(username, UrlLogin);
                    navigationManager.NavigateTo(urlLogOut, true);
                }
                else
                {
                    navigationManager.NavigateTo("/", true);
                }
            }
            
        }
        
    }
}
