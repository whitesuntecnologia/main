using Business.Interfaces;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace Website.Pages.BandejaTramites
{
    public partial class Index: ComponentBase
    {
        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [Inject] private ITramitesBL _TramiteBL { get; set; } = null!;
        [Inject] private IUsuariosBL _usuariosBL { get; set; } = null!;

        private List<ItemGrillaBandejaDTO> lstTramites = new List<ItemGrillaBandejaDTO>();
        private RadzenDataGrid<ItemGrillaBandejaDTO>? grdTramites = null;
        private bool isBusyTomar { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await ReloadGrilla();
            await base.OnInitializedAsync();
        }

        private async Task ReloadGrilla()
        {
            lstTramites = await _TramiteBL.GetTramitesBandejaAsync(await _usuariosBL.GetCurrentUserid());
            lstTramites = lstTramites.OrderByDescending(o => o.IdTramite).ToList();
            grdTramites?.Reload();
        }

        protected async Task TomarClick(ItemGrillaBandejaDTO row)
        {
            if (!isBusyTomar)
            {
                isBusyTomar = true;
                await _TramiteBL.TomarTramiteAsync(row.IdTramite);
                await ReloadGrilla();
                isBusyTomar = false;
            }
        }
        protected Task VisualizarClick(ItemGrillaBandejaDTO row)
        {
            navigationManager.NavigateTo($"/Tramites/Visualizar/{row.IdentificadorUnico}");
            return Task.CompletedTask;
        }
    }
    
}
