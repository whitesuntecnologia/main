using Business;
using Business.Interfaces;
using DataAccess.Entities;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using Website.Models.Formulario;
using Website.Pages.Tramites.Formulario.Form8;

namespace Website.Pages.Parametros.ICC
{
    public partial class Index: ComponentBase
    {
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private DialogService DS { get; set; } = null!;
        [Inject] private IIndiceConstruccionBL indicesBL { get; set; } = null!;
        

        private List<IndiceCostoConstruccionDTO> lstIndices { get; set; } = new();
        private RadzenDataGrid<IndiceCostoConstruccionDTO>? grdIndices = null!;

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
        private async Task EditarClick(IndiceCostoConstruccionDTO row)
        {

            var dto = await indicesBL.GetIndiceAsync(row.IdIcc);
            var respuesta = await DS.OpenAsync<AddEditIndice>("Editar Índice",
                                        new Dictionary<string, object>() { { "IdIcc", row.IdIcc}},
                                        new DialogOptions() { Width = "60%", Height = "auto", Resizable = true });

            if (respuesta ?? false)
            {
                await ReloadGrillaAsync();
            }
            
        }
        protected async Task EliminarObraClick(IndiceCostoConstruccionDTO row)
        {
            var respuesta = await DS.Confirm("¿Está seguro de eliminar el registro?", "Confirmación",
                new ConfirmOptions() { CancelButtonText = "No", OkButtonText = "Sí" });

            if (respuesta ?? false)
            {
                await indicesBL.EliminarIndiceAsync(row.IdIcc);
                await ReloadGrillaAsync();
            }
        }
    }
}
