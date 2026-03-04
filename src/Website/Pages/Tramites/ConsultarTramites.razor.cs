using Business;
using Business.Interfaces;
using DataAccess.Entities;
using DataTransferObject;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace Website.Pages.Tramites
{
    public partial class ConsultarTramites :ComponentBase
    {
        [Inject] private ITramitesBL _TramiteBL { get; set; } = null!;
        [Inject] private IUsuariosBL _usuariosBL { get; set; } = null!;

        private List<ItemGrillaConsultarTramitesDTO> lstTramites = new List<ItemGrillaConsultarTramitesDTO>();
        private RadzenDataGrid<ItemGrillaConsultarTramitesDTO>? grdTramites = null;

        protected override async Task OnInitializedAsync()
        {

            int userIdEmpresa = 0;
            int.TryParse(await _usuariosBL.GetCurrentIdEmpresa(), out userIdEmpresa);
            lstTramites = await _TramiteBL.GetTramitesGrillaByEmpresaAsync(userIdEmpresa);
            lstTramites = lstTramites.OrderByDescending(o => o.IdTramite).ToList();
            grdTramites?.Reload();
            await base.OnInitializedAsync();
        }
        
    }
}
