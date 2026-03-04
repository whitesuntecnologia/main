using Business.Interfaces;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Radzen;
using Radzen.Blazor;
using StaticClass;

namespace Website.Pages.Parametros.Especialidades.Secciones
{
    public partial class Index: ComponentBase
    {
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private DialogService DS { get; set; } = null!;
        [Inject] private IEspecilidadesBL especialidadesBL { get; set; } = null!;

        private List<EspecialidadSeccionDTO> lstSecciones { get; set; } = new();
        private RadzenDataGrid<EspecialidadSeccionDTO>? grdSecciones = null!;

        protected override async Task OnInitializedAsync()
        {
            await ReloadGrillaAsync();
            await base.OnInitializedAsync();
        }
        private async Task ReloadGrillaAsync()
        {
            lstSecciones = await especialidadesBL.GetEspecialidadesSeccionesAsync();
            grdSecciones?.Reload();
        }
        protected async Task AgregarClick()
        {
            var respuesta = await DS.OpenAsync<AddOrEdit>("Agregar Sección", null,
                                        new DialogOptions() { Width = "60%", Height = "auto", Resizable = true });

            if (respuesta ?? false)
            {
                await ReloadGrillaAsync();
            }
        }
        private async Task EditarClick(EspecialidadSeccionDTO row)
        {

            var dto = await especialidadesBL.GetEspecialidadEquipoAsync(row.IdSeccion);
            var respuesta = await DS.OpenAsync<AddOrEdit>("Editar Sección",
                                        new Dictionary<string, object>() { { "IdSeccion", row.IdSeccion } },
                                        new DialogOptions() { Width = "60%", Height = "auto", Resizable = true });

            if (respuesta ?? false)
            {
                await ReloadGrillaAsync();
            }

        }
        protected async Task EliminarClick(EspecialidadSeccionDTO row)
        {
            try
            {
                var respuesta = await DS.Confirm("¿Está seguro de eliminar el registro?", "Confirmación",
                new ConfirmOptions() { CancelButtonText = "No", OkButtonText = "Sí" });

                if (respuesta ?? false)
                {
                    await especialidadesBL.EliminarSeccionAsync(row.IdSeccion);
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
                        row.Baja = true;
                        await especialidadesBL.ActualizarSeccionAsync(row);
                        await ReloadGrillaAsync();
                    }
                }
                else
                    notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
            }

        }
    }
}
