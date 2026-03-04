using Business.Interfaces;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen;
using Website.Pages.Parametros.ICC;

namespace Website.Pages.Parametros.BCRA
{
    public partial class Index: ComponentBase
    {
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private DialogService DS { get; set; } = null!;
        [Inject] private IIndiceBancoCentralBL indicesBL { get; set; } = null!;

        private List<IndiceBancoCentralDTO> lstIndices { get; set; } = new();
        private RadzenDataGrid<IndiceBancoCentralDTO>? grdIndices = null!;

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
        private async Task EditarClick(IndiceBancoCentralDTO row)
        {

            var dto = await indicesBL.GetIndiceAsync(row.IdIndiceBcra);
            var respuesta = await DS.OpenAsync<AddEditIndice>("Editar Índice",
                                        new Dictionary<string, object>() { { "IdIndiceBcra", row.IdIndiceBcra } },
                                        new DialogOptions() { Width = "60%", Height = "auto", Resizable = true });

            if (respuesta ?? false)
            {
                await ReloadGrillaAsync();
            }

        }
        protected async Task EliminarObraClick(IndiceBancoCentralDTO row)
        {
            var respuesta = await DS.Confirm("¿Está seguro de eliminar el registro?", "Confirmación",
                new ConfirmOptions() { CancelButtonText = "No", OkButtonText = "Sí" });

            if (respuesta ?? false)
            {
                await indicesBL.EliminarIndiceAsync(row.IdIndiceBcra);
                await ReloadGrillaAsync();
            }
        }

    }
}
