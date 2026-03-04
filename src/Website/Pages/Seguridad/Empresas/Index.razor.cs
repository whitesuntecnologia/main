using Business.Interfaces;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen;
using StaticClass;

namespace Website.Pages.Seguridad.Empresas
{
    public partial class Index: ComponentBase
    {
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private DialogService DS { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private IEmpresasBL relEmpresaRepresentanteBL { get; set; } = null!;

        private List<EmpresaDTO> lstEmpresas { get; set; } = new();
        private RadzenDataGrid<EmpresaDTO>? grdEmpresas = null!;

        protected override async Task OnInitializedAsync()
        {
            await ReloadGrillaAsync();
            await base.OnInitializedAsync();
        }
        private async Task ReloadGrillaAsync()
        {
            lstEmpresas = await relEmpresaRepresentanteBL.GetEmpresasAsync();
            grdEmpresas?.Reload();
        }
        protected async Task AgregarClick()
        {
            navigationManager.NavigateTo($"/Seguridad/Empresas/Agregar",true);
        }
        private async Task EditarClick(EmpresaDTO row)
        {


            navigationManager.NavigateTo($"/Seguridad/Empresas/Editar/{row.IdEmpresa}", true);

        }
        protected async Task EliminarClick(EmpresaDTO row)
        {
            var respuesta = await DS.Confirm("¿Está seguro de eliminar el registro?", "Confirmación",
                new ConfirmOptions() { CancelButtonText = "No", OkButtonText = "Sí" });

            if (respuesta ?? false)
            {
                try
                {
                    await relEmpresaRepresentanteBL.EliminarEmpresaAsync(row.IdEmpresa);
                    await ReloadGrillaAsync();
                }
                catch(Exception ex)
                {
                    notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
                }
            }
        }
    }
}
