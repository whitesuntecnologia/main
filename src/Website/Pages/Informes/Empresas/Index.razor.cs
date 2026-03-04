using Business;
using Business.Interfaces;
using DataAccess.Entities;
using DataTransferObject;
using DataTransferObject.BLs;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using System;
using Website.Extensions;
using Website.Models.Informes;
using static StaticClass.Constants;

namespace Website.Pages.Informes.Empresas
{
    public partial class Index: ComponentBase
    {
        [Inject] private IEmpresasBL empresasBL { get; set; } = null!;
        [Inject] public IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] private ICombosBL _CombosBL { get; set; } = null!;
        private bool isBusyInitialLoading { get; set; } = true;
        private bool isBusyGrid { get; set; }
        private EmpresasBusquedaModel BusquedaModel { get; set; } = new();
        private List<InformeEmpresaDTO> lstEmpresas { get; set; } = new();
        private IEnumerable<GenericComboDTO> lstEspecialidades = new List<GenericComboDTO>();
        private RadzenDataGrid<InformeEmpresaDTO> grd = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            lstEspecialidades = await _CombosBL.GetEspecialidadesAsync();
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
            FiltroInformeEmpresasDTO filtro = new();
            filtro.EmpresasRegistradas = BusquedaModel.TiposEmpresasSelected == 1;
            filtro.Cuit = BusquedaModel.Cuit;
            filtro.RazonSocial = BusquedaModel.RazonSocial;
            filtro.FechaRegistroDesde = BusquedaModel.FechaRegistroDesde;
            filtro.FechaRegistroHasta = (BusquedaModel.FechaRegistroHasta.HasValue ? BusquedaModel.FechaRegistroHasta.Value.AddHours(23).AddMinutes(59).AddSeconds(59) : null);
            filtro.FechaVencimientoDesde = BusquedaModel.FechaVencimientoDesde;
            filtro.FechaVencimientoHasta = (BusquedaModel.FechaVencimientoHasta.HasValue ? BusquedaModel.FechaVencimientoHasta.Value.AddHours(23).AddMinutes(59).AddSeconds(59) : null);
            filtro.EspecialidadesSelected = BusquedaModel.EspecialidadesSelected;
            filtro.CapacidadTecnicaDesde = BusquedaModel.CapacidadTecnicaDesde;
            filtro.CapacidadTecnicaHasta = BusquedaModel.CapacidadTecnicaHasta;
            filtro.CapacidadEconomicaDesde = BusquedaModel.CapacidadEconomicaDesde;
            filtro.CapacidadEconomicaHasta = BusquedaModel.CapacidadEconomicaHasta;

            lstEmpresas = await empresasBL.GetInformeEmpresasAsync(filtro);
            grd?.Reload();
            isBusyGrid = false;
        }

        private async Task ExportarExcel()
        {
            if (grd != null)
            {
                await grd.ExportGridDataAsync(JSRuntime, ExportType.Excel, "Empresas", new List<string>() { "Acciones" });
            }
        }
        private async Task ExportarCsv()
        {
            if (grd != null)
            {
                await grd.ExportGridDataAsync(JSRuntime, ExportType.Csv, "Empresas", new List<string>() { "Acciones" });
            }
        }
        private async Task ExportarPdf()
        {
            if (grd != null)
            {
                await grd.ExportGridDataAsync(JSRuntime, ExportType.Pdf, "Empresas", new List<string>() { "Acciones" });
            }
        }
        protected Task VisualizarClick(InformeEmpresaDTO row)
        {
            navigationManager.NavigateTo($"/Consultas/Empresas/Detalle/{row.IdEmpresa}");
            return Task.CompletedTask;
        }

    }
}
