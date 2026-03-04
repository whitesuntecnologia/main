using Business.Interfaces;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen;
using Website.Pages.Shared;

namespace Website.Pages.Notificaciones
{
    public partial class Alerta: ComponentBase
    {
        [Inject] private INotificacionesBL notificacionesBL { get; set; } = null!;
        [Inject] private IUsuariosBL usuariosBL { get; set; } = null!;
        [Inject] private ITramitesBL tramitesBL { get; set; } = null!;
        [Inject] private DialogService dialogService { get; set; } = null!;
        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        private List<AlertaDTO> lstAlertas { get; set; } = new List<AlertaDTO>();
        private RadzenDataGrid<AlertaDTO> grdAlertas = null!;
        private string userid { get; set; } = null!;
        private bool isBusyInitialLoading { get; set; } = true;
        protected override async Task OnInitializedAsync()
        {
            userid = await usuariosBL.GetCurrentUserid();
            var lstPerfiles = await usuariosBL.GetPerfilesForUserAsync(userid);

            bool EsEmpresa = lstPerfiles.Select(s => s.IdPerfil).Contains((int)StaticClass.Constants.Perfiles.Empresa);
            if (EsEmpresa)
            {
                int IdEmpresa = int.Parse(await usuariosBL.GetCurrentIdEmpresa());
                lstAlertas = await notificacionesBL.GetAlertas(IdEmpresa);
            }
            else
                lstAlertas = await notificacionesBL.GetAlertas();

            await base.OnInitializedAsync();
            isBusyInitialLoading = false;
        }

    
    }
}
