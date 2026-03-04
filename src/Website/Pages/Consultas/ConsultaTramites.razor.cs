using Business.Interfaces;
using DataTransferObject;
using DataTransferObject.BLs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Radzen;
using Radzen.Blazor;
using StaticClass;

namespace Website.Pages.Consultas
{
    public partial class ConsultaTramites: ComponentBase
    {
        [Inject] private NavigationManager navigationManager { get; set; } = null!;
        [Inject] private ITramitesBL _TramiteBL { get; set; } = null!;
        [Inject] private IUsuariosBL _UsuarioBL { get; set; } = null!;
        [Inject] private ICombosBL _combosBL { get; set; } = null!;

        private FilterConsultaTramitesDTO filter { get; set; } = new();
        private List<ItemGrillaConsultaTramiteDTO> lstTramites = new List<ItemGrillaConsultaTramiteDTO>();
        private RadzenDataGrid<ItemGrillaConsultaTramiteDTO> grdTramites = null!;
        private IEnumerable<GenericComboDTO> lstEstados { get; set; } = null!;
        private IEnumerable<GenericComboDTO> lstEmpresas { get; set; } = null!;
        private bool OcultarBorradores { get; set; } = true;
        private bool isBusyTomar { get; set; }
        private bool isBusyGrid { get; set; }
        private int count;
        

        protected override async Task OnInitializedAsync()
        {
            lstEstados = await _combosBL.GetTramitesEstadosAsync();
            lstEmpresas = await _combosBL.GetEmpresasAsync();
            await base.OnInitializedAsync();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
                await grdTramites.Reload();
            
            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task LoadData(LoadDataArgs args)
        {
            isBusyGrid = true;
            lstTramites = await _TramiteBL.GetTramitesAsync(filter, 
                                                            args.Skip.GetValueOrDefault(), 
                                                            args.Top.GetValueOrDefault(),
                                                            out count);
            if(OcultarBorradores)
                lstTramites = lstTramites.Where(w => w.IdEstado != Constants.TramitesEstados.EditarInformacion).ToList();
            
            isBusyGrid = false;
        }
      
        protected Task VisualizarClick(ItemGrillaConsultaTramiteDTO row)
        {
            navigationManager.NavigateTo($"/Tramites/Visualizar/{row.IdentificadorUnico}");
            return Task.CompletedTask;
        }
        private async Task BuscarClick()
        {
            isBusyGrid = true;
            grdTramites.Reset();
            await grdTramites.FirstPage(true);
            isBusyGrid = false;
        }
        private async Task LimpiarClick()
        {
            filter = new();
            grdTramites.Reset();
            await grdTramites.FirstPage(true);
        }
        private async Task chkOcultarBorradoresChange(ChangeEventArgs e)
        {
            OcultarBorradores = Convert.ToBoolean(e.Value);
            await grdTramites.Reload();
        }

    }
}
