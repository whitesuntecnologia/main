
using ElmahCore;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Security.Claims;
using StaticClass;
using DataTransferObject;
using System.Net;


namespace Website.Services
{
    public class CustomAuthenticateStateProvider: AuthenticationStateProvider
    {
        private readonly IJSRuntime _JsRuntime;

        private readonly ProtectedLocalStorage _localStorage;
        private string strAuthData = "";
        private ClaimsPrincipal _claimsPrincipal;
        private readonly AppState _AppStateService;
        private readonly IConfiguration _configuration;
        private int seconds = 0;

        public CustomAuthenticateStateProvider(IJSRuntime jSRuntime, ProtectedLocalStorage localStorage, AppState appStateService, IConfiguration configuration)
        {
            _JsRuntime = jSRuntime;
            _localStorage = localStorage;
            _claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            _AppStateService = appStateService;
            _configuration = configuration;
            seconds = int.Parse(_configuration["AppSettings:CookieTotalSeconds"]!.ToString());
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {

            try
            {
                // Esperar hasta que se haya inicializado correctamente
                //se inicializa en el first rendet de app.razor
                while (!_AppStateService.IsAppLoaded)
                {
                    await Task.Yield();
                    await Task.Delay(100);
                }

                if (!_claimsPrincipal.Identity!.IsAuthenticated)
                {

                    TimeSpan timeout = TimeSpan.FromSeconds(10);

                    var ValueStorageresult = await Task.Run(() => _localStorage?.GetAsync<string>(Constants.LocalStorageAuthKey).Result).WaitAsync(timeout);

                    if (ValueStorageresult != null && ValueStorageresult.Value.Success)
                        strAuthData = (ValueStorageresult.Value.Value ?? "");

                    if (!string.IsNullOrWhiteSpace(strAuthData))
                    {
                        var CookieDto = JsonConvert.DeserializeObject<DataTransferObject.CookieDto>(strAuthData);

                        if (DateTime.Now > CookieDto.Expires)
                            _claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
                        else
                        {
                            var userDto = JsonConvert.DeserializeObject<UsuarioCookieDataDto>(CookieDto!.Value);
                            if (userDto.CuitEmpresa.HasValue)
                                userDto.CuitEmpresa = Math.Floor(userDto.CuitEmpresa.Value);

                            _claimsPrincipal = GetClaimsPrincipalFromUser(userDto!);
                            CookieDto.Expires = DateTime.Now.AddSeconds(seconds);
                            await _localStorage.SetAsync(Constants.LocalStorageAuthKey, JsonConvert.SerializeObject(CookieDto));
                        }
                        
                    }
                    else
                    {
                        _claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
                    }
                }

            }
            catch (TimeoutException ex)
            {
                await _JsRuntime.InvokeVoidAsync("console.log", ex.Message);
            }
            catch (TaskCanceledException ex)
            {
                ElmahExtensions.RaiseError(ex);
                await _JsRuntime.InvokeVoidAsync("console.log", ex.Message);
            }
            catch (Exception ex)
            {
                ElmahExtensions.RaiseError(ex);
                await _localStorage.DeleteAsync(Constants.LocalStorageAuthKey);
            }

            //cuando hay errores devuelve esto
            if (_claimsPrincipal.Claims.Count() == 0)
                _claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_claimsPrincipal)));
            return new AuthenticationState(_claimsPrincipal);
        }


        // 🚀 Método para forzar la actualización del estado
        public Task NotifyUserAuthentication(UsuarioCookieDataDto userDto)
        {
            _claimsPrincipal = GetClaimsPrincipalFromUser(userDto);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_claimsPrincipal)));
            return Task.CompletedTask;
        }

        private ClaimsPrincipal GetClaimsPrincipalFromUser(UsuarioCookieDataDto userSession)
        {
            ClaimsPrincipal result = null!;
            var lstClaims = new List<Claim>();
            lstClaims.Add(new Claim(ClaimTypes.NameIdentifier, userSession.Id ?? ""));
            lstClaims.Add(new Claim(ClaimTypes.Name, userSession.UserName ?? ""));
            lstClaims.Add(new Claim("NombreyApellido", userSession.NombreyApellido ?? ""));
            if (userSession.IdEmpresa.HasValue)
            {
                lstClaims.Add(new Claim("IdEmpresa", userSession.IdEmpresa.GetValueOrDefault().ToString()));
                lstClaims.Add(new Claim("CuitEmpresa", userSession.CuitEmpresa.GetValueOrDefault().ToString()));
                lstClaims.Add(new Claim("NombreEmpresa", userSession.NombreEmpresa ?? ""));
            }
            
            foreach (var rol in userSession.Roles)
            {
                lstClaims.Add(new Claim(ClaimTypes.Role, rol));
            }

            var identity = new ClaimsIdentity(lstClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            result = new ClaimsPrincipal(identity);

            return result;
        }
    }
}
