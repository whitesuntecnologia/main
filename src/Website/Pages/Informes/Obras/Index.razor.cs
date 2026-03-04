using Business.Interfaces;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen.Blazor;
using Website.Extensions;
using Website.Models.Informes;
using static StaticClass.Constants;

namespace Website.Pages.Informes.Obras
{
    public partial class Index: ComponentBase
    {
        [Inject] private IObrasProvinciaLaPampaBL obrasBL { get; set; } = null!;
        [Inject] public IJSRuntime JSRuntime { get; set; } = null!;
        private bool isBusyInitialLoading { get; set; } = true;
        private bool isBusyGrid { get; set; }
        private ObrasBusquedaModel BusquedaModel { get; set; } = new();
        private List<InformeObraDTO> lstObras { get; set; } = new();
        private RadzenDataGrid<InformeObraDTO> grd = null!;

        protected override async Task OnInitializedAsync()
        {
            await ReloadGrillaAsync();
            isBusyInitialLoading = false;
            await base.OnInitializedAsync();
        }
        private async Task BuscarClick()
        {
            isBusyGrid = true;
            await ReloadGrillaAsync();
            isBusyGrid = false;
        }
        private async Task LimpiarClick()
        {
            BusquedaModel = new();
            await ReloadGrillaAsync();
        }
        private async Task ReloadGrillaAsync()
        {
            isBusyGrid = true;
            FiltroInformeObraDTO filtro = new();

            filtro.Cuit = BusquedaModel.Cuit;
            filtro.RazonSocial = BusquedaModel.RazonSocial;
            filtro.Nombre = BusquedaModel.Nombre;

            lstObras = await obrasBL.GetInformeObras1Async(filtro);
            grd?.Reload();
            isBusyGrid = false;
        }

        private async Task ExportarExcel()
        {
            if (grd != null)
            {
                await grd.ExportGridDataAsync(JSRuntime, ExportType.Excel, "Obras", new List<string>() { "Acciones" });
            }
        }
        private async Task ExportarCsv()
        {
            if (grd != null)
            {
                await grd.ExportGridDataAsync(JSRuntime, ExportType.Csv, "Obras", new List<string>() { "Acciones" });
            }
        }
        private async Task ExportarPdf()
        {
            if (grd != null)
            {
                await grd.ExportGridDataAsync(JSRuntime, ExportType.Pdf, "Obras", new List<string>() { "Acciones" });
            }
        }
    }
}
