using DataAccess;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using Radzen;
using StaticClass;
using Website.Models.Account;
using Website.Services;

namespace Website.Pages.Account
{
    public partial class Register: ComponentBase
    {
        [Inject] private AuthenticationStateProvider authenticationStateProvider { get; set; } = null!;
        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [Inject] private SignInManager<UserProfile> signInManager { get; set; } = null!;
        [Inject] private UserManager<UserProfile> userManager { get; set; } = null!;
        [Inject] private IJSRuntime jsRuntime { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        public RegisterModel registerModel { get; set; } = new();
        private string ErrorMessage { get; set; } = null!;
        private async Task Registering(EditContext ed)
        {
            ErrorMessage = "";
            if (ed.Validate())
            {
                try
                {
                
                    var user = await signInManager.UserManager.FindByNameAsync(registerModel.Username);
                    if (user != null)
                        throw new Exception( "Ya existe un usuario con ese nombre.");
                    
                    
                    var newUser = CreateUser();
                    newUser.UserName = registerModel.Username;
                    newUser.Email = registerModel.Email;

                    var result = await userManager.CreateAsync(newUser, registerModel.Password);

                    if (result.Succeeded)
                    {
                        var userId = await userManager.GetUserIdAsync(newUser);
                    }

                    if (result.Succeeded)
                    {
                        notificationService.Notify(NotificationSeverity.Success, "Aviso", $"El usuario {newUser.UserName} has sido creado con éxito");
                    }
                    else
                    {
                        if (result.Errors.Count() > 0)
                            ErrorMessage = result.Errors.First().Description;
                        else
                            ErrorMessage = "No se ha podido crear el usuario";
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = Functions.GetErrorMessage(ex);
                }
            }
        }
       
        private UserProfile CreateUser()
        {
            try
            {
                return Activator.CreateInstance<UserProfile>();
            }
            catch
            {
                throw new InvalidOperationException($"No se puede crear una instancia de '{nameof(UserProfile)}'. ");
            }
        }


    }
}
