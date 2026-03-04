using Business.Interfaces;
using Microsoft.AspNetCore.Components;
using DataTransferObject;
using DataTransferObject.BLs;
using Website.Models.Account;
using Microsoft.AspNetCore.Components.Forms;
using Website.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Newtonsoft.Json;
using StaticClass;

namespace Website.Pages.Account
{

    public partial class SeleccionarRepresentado: ComponentBase
    {

        [Parameter] public string userid { get; set; } = null!;
        [Parameter] public string token { get; set; } = null!;
        [Inject] private AuthenticationStateProvider authenticationStateProvider { get; set; } = null!;
        [Inject] private IUsuariosBL usuarioBL { get; set; } = null!;
        [Inject] private IEmpresasBL empresaBL { get; set; } = null!;
        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [Inject] private ProtectedLocalStorage _localStorage { get; set; } = null!;
        private List<GenericComboDTO> lstEmpresas = new();
        public SeleccionarRepresentadoModel Model { get; set; } = new();
        private bool usuarioValido { get; set; } = true;
        
        protected override async Task OnInitializedAsync()
        {

            string tokenGuardado = await usuarioBL.GetTokenAsync(userid);

            if (token != tokenGuardado)
                usuarioValido = false;
            else
            {

                Model.userid = userid;
                var empresas = await usuarioBL.GetUsuariosVinculados(userid);

                lstEmpresas.AddRange(empresas.Select(s => new GenericComboDTO()
                {
                    Id = s.IdEmpresa,
                    Descripcion = s.CuitEmpresa.ToString() + " - " + s.RazonSocial
                }).ToList());
            }

            await base.OnInitializedAsync();   
        }
        private async Task LoginIn(EditContext ed)
        {

            if (ed.Validate())
            {
                var user = await usuarioBL.GetUserByIdAsync(userid);
                var empresaDto = await empresaBL.GetEmpresaAsync(Model.IdEmpresa.GetValueOrDefault());

                //Obtiene los datos de la cookie
                var Cookie = await usuarioBL.GetCookieAuthorization(user.UserName, empresaDto);
                //establece la cookie
                await _localStorage.SetAsync(Constants.LocalStorageAuthKey, JsonConvert.SerializeObject(Cookie));
                //redirecciona a la pagina establecida
                navigationManager.NavigateTo("/", true);

            }
        }
    }
}

