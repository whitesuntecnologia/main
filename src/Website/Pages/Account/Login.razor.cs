using Business.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using StaticClass;
using Website.Services;

namespace Website.Pages.Account
{
    public partial class Login: ComponentBase
    {
        [Inject] private AuthenticationStateProvider authenticationStateProvider { get; set; } = null!;
        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [Inject] private ILoginBL loginBL { get; set; } = null!;
        
        private string ErrorMessage { get; set; } = null!;
        private async Task LoginIn()
        {
            ErrorMessage = "";
            try
            {
                
                string urlLoginSUU = await loginBL.GetUrlLogin();
                navigationManager.NavigateTo(urlLoginSUU);
            }
            catch (Exception ex) 
            {
                this.ErrorMessage = Functions.GetErrorMessage(ex);
            }
        }
    }
}
