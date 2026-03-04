using Business.Interfaces;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen;
using StaticClass;

namespace Website.Pages.Parametros.UVI
{
    public partial class Index: ComponentBase
    {
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private DialogService DS { get; set; } = null!;
        [Inject] private IIndiceUnidadViviendaBL indicesBL { get; set; } = null!;

        private List<IndiceUnidadViviendaDTO> lstIndices { get; set; } = new();
        private RadzenDataGrid<IndiceUnidadViviendaDTO>? grdIndices = null!;

        protected override async Task OnInitializedAsync()
        {
            await ReloadGrillaAsync();
            await base.OnInitializedAsync();
        }
        private async Task ReloadGrillaAsync()
        {
            lstIndices = await indicesBL.GetIndicesAsync();
            grdIndices?.Reload();
        }
        protected async Task AgregarClick()
        {
            var respuesta = await DS.OpenAsync<AddEditIndice>("Agregar Índice", null,
                                        new DialogOptions() { Width = "60%", Height = "auto", Resizable = true });

            if (respuesta ?? false)
            {
                await ReloadGrillaAsync();
            }
        }
        private async Task EditarClick(IndiceUnidadViviendaDTO row)
        {

            var dto = await indicesBL.GetIndiceAsync(row.IdUvi);
            var respuesta = await DS.OpenAsync<AddEditIndice>("Editar Índice",
                                        new Dictionary<string, object>() { { "IdUvi", row.IdUvi } },
                                        new DialogOptions() { Width = "60%", Height = "auto", Resizable = true });

            if (respuesta ?? false)
            {
                await ReloadGrillaAsync();
            }

        }
        protected async Task EliminarClick(IndiceUnidadViviendaDTO row)
        {
            try
            {
                var respuesta = await DS.Confirm("¿Está seguro de eliminar el registro?", "Confirmación",
                new ConfirmOptions() { CancelButtonText = "No", OkButtonText = "Sí" });

                if (respuesta ?? false)
                {
                    await indicesBL.EliminarIndiceAsync(row.IdUvi);
                    await ReloadGrillaAsync();
                }
            }
            catch (Exception ex)
            {
                notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
            }
            
        }
    }
}
