using Business.Interfaces;
using DataAccess;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using StaticClass;

namespace Website.Pages.Account
{
    public partial class Callback: ComponentBase
    {
        [Parameter]
        [SupplyParameterFromQuery]
        public string gam { get; set; } = string.Empty;
        [Parameter]
        [SupplyParameterFromQuery] 
        public string code { get; set; } = string.Empty;
        
        [Inject] private AuthenticationStateProvider authenticationStateProvider { get; set; } = null!;
        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [Inject] private ILoginBL loginBL { get; set; } = null!;
        [Inject] private IUsuariosBL usuarioBL { get; set; } = null!;
        [Inject] private IEmpresasBL empresaBL { get; set; } = null!;
        [Inject] private ProtectedLocalStorage _localStorage { get; set; } = null!;
        private string ErrorMessage { get; set; } = "";
        private string usuarioLogueado { get; set; } = "";
        private bool isLoading { get; set; } = true;
        [Inject] private SignInManager<UserProfile> signInManager { get; set; } = null!;
        [Inject] private UserManager<UserProfile> userManager { get; set; } = null!;
        private AppSettings? _appSettings { get; set; } = null;

        public Callback()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();
            _appSettings = config.GetSection("AppSettings").Get<AppSettings>();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                ErrorMessage = string.Empty;
                usuarioLogueado = string.Empty;
                try
                {
                    if (string.IsNullOrWhiteSpace(code))
                        throw new ArgumentException("No se ha enviado el parámetro code.");

                    var UserSuu = await loginBL.GetUsuarioLogueado(code,gam);
                    

                    if (UserSuu != null)
                    {
                        usuarioLogueado = UserSuu.usuario;
                        var user = await usuarioBL.GetUserByNameAsync(usuarioLogueado);
                        
                        if (user == null)
                        {
                            throw new ArgumentException($"El usuario {usuarioLogueado} no posee un usuario válido en el sistema de registro de licitadores. Debe pedir al Administrador el alta del mismo.");
                        }
                        else
                        {

                            //Bloqueo de Identity
                            if (user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.Now)
                            {
                                throw new ArgumentException($"El usuario {usuarioLogueado} se encuentra bloqueado");
                            }

                            //Bloqueo por estado del sistema
                            if (user.Estado == Constants.UsuariosEstados.Bloqueado)
                            {
                                throw new ArgumentException($"El usuario {usuarioLogueado} se encuentra bloqueado");
                            }
                            //Bloqueo por estado del sistema
                            if (user.Estado == Constants.UsuariosEstados.Baja)
                            {
                                throw new ArgumentException($"El usuario {usuarioLogueado} se encuentra dado de baja");
                            }


                            //Usuario Existente

                            var lstPerfiles = await usuarioBL.GetPerfilesForUserAsync(user.Id);
                            var lstEmpresas = await usuarioBL.GetUsuariosVinculados(user.Id);

                            if (lstPerfiles.Select(s => s.IdPerfil).Contains(Constants.Perfiles.Empresa) && lstEmpresas.Count == 0)
                            {
                                throw new ArgumentException($"El usuario {usuarioLogueado} tiene perfil empresa y no hay ninguna empresa vinculada al mismo," + Environment.NewLine + " comuniquese con el administrador para que vincule el usuario a una empresa.");
                            }


                            if (lstEmpresas.Count <= 1)
                            {
                                //Obtiene los datos de la cookie
                                var Cookie = await usuarioBL.GetCookieAuthorization(user.UserName, lstEmpresas.FirstOrDefault());
                                //establece la cookie
                                await _localStorage.SetAsync(Constants.LocalStorageAuthKey, JsonConvert.SerializeObject(Cookie));

                                isLoading = false;
                                StateHasChanged();

                                await Task.Delay(2000);
                                navigationManager.NavigateTo("/",true);

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
                    isLoading = false;
                    StateHasChanged();
                }


                if (!string.IsNullOrWhiteSpace(usuarioLogueado) && !string.IsNullOrWhiteSpace(ErrorMessage))
                {
                    //Se Logueo en SUU pero no cumple las validaciones internas de RELI y hay que desloguear del SUU
                        
                    //Obtiene la Url de la pantalla de login para mandarla al logout del SUU
                    string UrlLogin = $"{navigationManager.BaseUri}Account/Login";
                    //Elimina la cookie de autorizacion del localsotrage
                    await _localStorage.DeleteAsync(Constants.LocalStorageAuthKey);
                    //Obtiene la url de deslogueo del SUU
                    string urlLogOut = await loginBL.GetUrlLogout(usuarioLogueado, UrlLogin);
                    usuarioLogueado = string.Empty;
                    await Task.Delay(2000);
                    navigationManager.NavigateTo(urlLogOut, true);
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
