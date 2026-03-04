using Business;
using Business.Interfaces;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Radzen;
using Radzen.Blazor;
using StaticClass;
using Website.Models.ModuloObras;

namespace Website.Pages.ModuloObras
{
    public partial class Index: ComponentBase
    {
        [Inject] private IObrasProvinciaLaPampaBL obrasBL { get; set; } = null!;

        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private DialogService DS { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;

        private List<ObrasProvinciaLaPampaDTO> lstObras { get; set; } = new();
        private RadzenDataGrid<ObrasProvinciaLaPampaDTO>? grdObras= null!;
        private bool isBusyInitialLoading { get; set; } = true;
        private bool isBusyProcessUpdating { get; set; } = false;
        
        private bool isBusyGrid { get; set; }
        private BusquedaObrasPciaLPModel Model { get; set; } = new();
        private DateTime? UltimaFechaActualizacion { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            UltimaFechaActualizacion = await obrasBL.GetFechaUltimaActualizacionAsync();
            await ReloadGrillaAsync();
            await base.OnInitializedAsync();
            isBusyInitialLoading = false;
        }
        private async Task ReloadGrillaAsync()
        {
            isBusyGrid = true;
            lstObras = await obrasBL.GetObrasAsync();
            grdObras?.Reload();
            isBusyGrid = false;
        }
        protected Task AgregarClick()
        {
            navigationManager.NavigateTo("/ModuloObras/Agregar");
            return Task.CompletedTask;
        }
        private async Task BuscarClick()
        {
            isBusyGrid = true;
            lstObras = await obrasBL.GetObrasAsync(Model.Expediente,Model.Nombre, Model.CuitEmpresa,Model.Empresa);
            isBusyGrid = false;
        }
        private async Task LimpiarClick()
        {
            Model = new();
            await ReloadGrillaAsync();
        }
        protected async Task EliminarClick(ObrasProvinciaLaPampaDTO row)
        {
            try
            {
                var respuesta = await DS.Confirm("¿Está seguro de eliminar el registro?", "Confirmación",
                new ConfirmOptions() { CancelButtonText = "No", OkButtonText = "Sí" });

                if (respuesta ?? false)
                {
                    await obrasBL.EliminarObraAsync(row.IdObraPciaLp);
                    await ReloadGrillaAsync();
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException is SqlException)
                {
                    SqlException sqlEx = (SqlException)ex.InnerException;
                    if (sqlEx.Number == Constants.SqlErroNumbers.ErrorRegistroRelacionado)
                    {
                        row.BajaLogica = true;
                        await obrasBL.ActualizarObraAsync(row);
                        await ReloadGrillaAsync();
                    }
                }
                else
                    notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
            }

        }

        private Task EditarClick(ObrasProvinciaLaPampaDTO row)
        {
            navigationManager.NavigateTo($"/ModuloObras/Editar/{row.IdObraPciaLp}");
            return Task.CompletedTask;
        }

        private async Task EjecutarActualizacionClick()
        {
            try
            {
                isBusyProcessUpdating = true;
                await obrasBL.EjecutarActualizacionObrasAsync();
                UltimaFechaActualizacion = await obrasBL.GetFechaUltimaActualizacionAsync();
                await ReloadGrillaAsync();
            }
            catch (Exception ex)
            {
                notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
            }
            finally
            {
                isBusyProcessUpdating = false;
            }
        }
        
    }
}
