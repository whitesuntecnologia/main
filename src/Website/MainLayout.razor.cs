using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen;
using DataTransferObject;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Globalization;
using System.Security.Claims;
using Microsoft.VisualBasic;
using DataAccess.Entities;
using Business.Interfaces;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Timers;
using System.Threading;
using DataAccess;
using Business.Extensions;
using Website.Services;

namespace Website
{

    public partial class MainLayout : IDisposable
    {

        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; } = null!;
        [Inject] private IUsuariosBL _usuariosBL { get; set; } = null!;
        [Inject] private INotificacionesBL _notificacionesBL { get; set; } = null!;
        [Inject] private NavigationManager _navigationManager { get; set; } = null!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] private SignInManager<UserProfile> _signInManager { get; set; } = null!;
        [Inject] private AuthenticationStateProvider _authStateProvider { get; set; } = null!;
        [Inject] private RouteHistoryService history { get; set; } = null!;
        private System.Timers.Timer _timer { get; set; } = null!;
        private bool sidebar1Expanded { get; set; } = true;
        private List<ItemMenuSidebarDTO> lstMenues = new List<ItemMenuSidebarDTO>();
        private string usernameLogueado { get; set; } = "";
        private string usernameEmpresa { get; set; } = "";
        private string NombreEmpresa { get; set; } = "";
        private string NombreyApellido { get; set; } = "";
        private int CantAlertas = 0;

        protected override async Task OnInitializedAsync()
        {
            var authenticationState = await authenticationStateTask;

            if ((authenticationState.User.Identity?.IsAuthenticated ?? false))
            {
                usernameLogueado = authenticationStateTask.Result.User.Identity?.Name ?? "";
                usernameEmpresa = await _usuariosBL.GetCurrentUserNameEmpresa();
                NombreEmpresa = await _usuariosBL.GetCurrentNombreEmpresa();
                NombreyApellido = await _usuariosBL.GetCurrentNombreyApellido();

                _timer = new System.Timers.Timer();
                _timer.Interval = 5000;
                _timer.Elapsed += OnTimmerEllapsed;
                _timer.Enabled = true;

                int IdEmpresa = 0;
                int.TryParse(await _usuariosBL.GetCurrentIdEmpresa(), out IdEmpresa);

                if(IdEmpresa > 0)
                {
                    CantAlertas = (await _notificacionesBL.GetAlertas(IdEmpresa)).Count();
                }

                lstMenues.Add(new ItemMenuSidebarDTO() { Icon = "home", Text = "Inicio", Path = "/" });

                lstMenues.Add(new ItemMenuSidebarDTO() { Icon = "touch_app", Text = "Mis Mensajes", Path = "/MisMensajes" });

                if (CantAlertas > 0)
                    lstMenues.Add(new ItemMenuSidebarDTO() { Icon = "mail", Text = $"Mis Alertas ({CantAlertas})", Path = "/MisAlertas" }); 
                else
                    lstMenues.Add(new ItemMenuSidebarDTO() { Icon = "mail", Text = "Mis Alertas", Path = "/MisAlertas" });

                lstMenues.Add(new ItemMenuSidebarDTO() { Icon = "touch_app", Text = "Mis Trámites", Path = "/Tramites/ConsultarTramites" });
                lstMenues.Add(new ItemMenuSidebarDTO() { Icon = "touch_app", Text = "Ayuda", Path = "/Ayuda" });
                lstMenues.Add(new ItemMenuSidebarDTO() { Icon = "touch_app", Text = "Salir", Path = "/Account/Logout" });


            }

            await base.OnInitializedAsync();
            
        }
        private void OnTimmerEllapsed(object? sender , System.Timers.ElapsedEventArgs e)
        {
            _timer.Enabled = false;
            var authenticationState = authenticationStateTask.Result;
            if (authenticationState.User.Identity?.IsAuthenticated ?? false)
            {
                var result  =_authStateProvider.GetAuthenticationStateAsync().Result;
                _timer.Enabled = true;
            }
            else
                nm.NavigateTo("/Account/Login");
            
        }

        public void Dispose()
        {
            if(_timer != null)
                _timer.Enabled = false;
        }

    }
}

