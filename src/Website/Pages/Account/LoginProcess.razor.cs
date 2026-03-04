using Business.Interfaces;
using DataAccess;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using StaticClass;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Xml;
using System.Text;
using Website.Services;

namespace Website.Pages.Account
{
    public partial class LoginProcess: ComponentBase
    {
        [Parameter] public string Tokenregreso { get; set; } = string.Empty;
        [Inject] private AuthenticationStateProvider authenticationStateProvider { get; set; } = null!;
        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [Inject] private ILoginBL loginBL { get; set; } = null!;
        [Inject] private IUsuariosBL usuarioBL { get; set; } = null!;
        [Inject] private IEmpresasBL empresaBL { get; set; } = null!;
        private string ErrorMessage { get; set; } = "";
        private string usuarioLogueado { get; set; } = "";
        [Inject] private SignInManager<UserProfile> signInManager { get; set; } = null!;
        [Inject] private UserManager<UserProfile> userManager { get; set; } = null!;
        [Inject] private IJSRuntime jsRuntime { get; set; } = null!;
        private bool isLoading { get; set; } = true;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(Tokenregreso))
                        throw new Exception("No se ha enviado el Token de regreso.");

                    
                    usuarioLogueado = await loginBL.GetUsuarioLogueado(Tokenregreso);
                    

                    if (!string.IsNullOrWhiteSpace(usuarioLogueado))
                    {
                        var user = await userManager.FindByNameAsync(usuarioLogueado);
                        if (user == null)
                        {
                            ErrorMessage = $"El usuario {usuarioLogueado} no posee un usuario válido en el sistema de registro de licitadores. Debe pedir al Administrador el alta del mismo.";
                            /*
                            //Usuario Nuevo
                            var newUser = CreateUser();
                            newUser.UserName = usuarioLogueado;
                            newUser.Estado = Constants.UsuariosEstados.Activo;
                            //newUser.Email = "noreply@mail.com";
                            

                            var result = await userManager.CreateAsync(newUser, loginBL.GeneratePassword());

                            if (result.Succeeded)
                            {
                                var userId = await userManager.GetUserIdAsync(newUser);


                                await LoginUser(userId, newUser.UserName, newUser.Email ?? "");
                            }

                            if (!result.Succeeded)
                            {
                                if (result.Errors.Count() > 0)
                                    ErrorMessage = result.Errors.First().Description;
                                else
                                    ErrorMessage = "No se ha podido crear el usuario";
                            }*/
                        }
                        else
                        {
                            //Usuario Existente

                            var lstPermisos = await usuarioBL.GetPermisosForUserAsync(user.Id);
                            var lstPerfiles = await usuarioBL.GetPerfilesForUserAsync(user.Id);

                            var lstEmpresas = await usuarioBL.GetUsuariosVinculados(user.Id);

                            if (lstPerfiles.Select(s => s.IdPerfil).Contains(Constants.Perfiles.Empresa) && lstEmpresas.Count == 0)
                            {
                                ErrorMessage = "El usuario tiene perfil empresa y no hay ninguna empresa vinculada al mismo," + Environment.NewLine + " comuniquese con el administrador para que vincule el usuario a una empresa.";
                                return;
                            }


                            if (lstEmpresas.Count <= 1)
                            {
                                List<string> roles = new List<string>();
                                roles.AddRange(lstPermisos.Select(s => s.Codigo).ToList());
                                roles.AddRange(lstPerfiles.Select(s => s.Nombre).ToList());

                                EmpresaDTO? empresa = null;

                                if (lstEmpresas.Count == 1)
                                    empresa = lstEmpresas.First();

                                var customAuth = (CustomAuthenticateStateProvider)authenticationStateProvider;
                                var userSession = new UsuarioCookieDataDto()
                                {
                                    Id = user.Id,
                                    UserName = user.UserName ?? "",
                                    NombreyApellido = user.NombreyApellido ?? "",
                                    Email = user.Email ?? "",
                                    Roles = roles,
                                    IdEmpresa = empresa?.IdEmpresa,
                                    CuitEmpresa = empresa?.CuitEmpresa,
                                    NombreEmpresa = empresa?.RazonSocial
                                };

                                //await customAuth.UpdateAuthenticationState(userSession);
                                navigationManager.NavigateTo("/", true);
                            }
                            else
                            {
                                string token = await usuarioBL.SetTokenAsync(user.Id);
                                navigationManager.NavigateTo($"/Account/SeleccionarRepresentado/{user.Id}/{token}", true);
                            }


                        }

                        
                    }

                }
                catch (Exception ex)
                {
                    ErrorMessage = Functions.GetErrorMessage(ex);
                    
                }

                isLoading = false;
                StateHasChanged();

                if (!string.IsNullOrWhiteSpace(usuarioLogueado))
                {
                    await Task.Delay(2000);
                    navigationManager.NavigateTo("/");
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
