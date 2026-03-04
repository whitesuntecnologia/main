using Business.Interfaces;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen.Blazor;
using static StaticClass.Constants;
using Website.Models.Informes;
using Website.Extensions;

namespace Website.Pages.Informes.ResponsablesTecnicos
{
    public partial class Index : ComponentBase
    {
        [Inject] private  ITramitesBL tramitesBL { get; set; } = null!;
        [Inject] public IJSRuntime JSRuntime { get; set; } = null!;
        private bool isBusyInitialLoading { get; set; } = true;
        private bool isBusyGrid { get; set; }
        private ResponsablesTecnicosBusquedaModel BusquedaModel { get; set; } = new();
        private List<InformeRepresentanteTecnicoDTO> lstTecnicos { get; set; } = new();
        private RadzenDataGrid<InformeRepresentanteTecnicoDTO> grd = null!;

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
            FiltroInformeResponsablesTecnicosDTO filtro = new();
            
            filtro.Cuit = BusquedaModel.Cuit;
            filtro.RazonSocial = BusquedaModel.RazonSocial;
            filtro.Apellido = BusquedaModel.Apellido;
            filtro.Nombres = BusquedaModel.Nombres;
            filtro.FechaVencimientoMatriculaDesde = BusquedaModel.FechaVencimientoMatriculaDesde;
            filtro.FechaVencimientoMatriculaHasta = BusquedaModel.FechaVencimientoMatriculaHasta;
            filtro.FechaVencimientoContratoDesde = BusquedaModel.FechaVencimientoContratoDesde;
            filtro.FechaVencimientoContratoHasta = BusquedaModel.FechaVencimientoContratoHasta;

            lstTecnicos = await tramitesBL.GetInformeRepresentantesTecnicos1Async(filtro);
            grd?.Reload();
            isBusyGrid = false;
        }

        private async Task ExportarExcel()
        {
            if (grd != null)
            {
                await grd.ExportGridDataAsync(JSRuntime, ExportType.Excel, "Tecnicos", new List<string>() { "Acciones" });
            }
        }
        private async Task ExportarCsv()
        {
            if (grd != null)
            {
                await grd.ExportGridDataAsync(JSRuntime, ExportType.Csv, "Tecnicos", new List<string>() { "Acciones" });
            }
        }
        private async Task ExportarPdf()
        {
            if (grd != null)
            {
                await grd.ExportGridDataAsync(JSRuntime, ExportType.Pdf, "Tecnicos", new List<string>() { "Acciones" });
            }
        }
    }
}
