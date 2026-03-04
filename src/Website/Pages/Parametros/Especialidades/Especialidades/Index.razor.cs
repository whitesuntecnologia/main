using Business.Interfaces;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Radzen.Blazor;
using Radzen;
using StaticClass;

namespace Website.Pages.Parametros.Especialidades.Especialidades
{
    public partial class Index: ComponentBase
    {
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private DialogService DS { get; set; } = null!;
        [Inject] private IEspecilidadesBL especialidadesBL { get; set; } = null!;

        private List<EspecialidadDTO> lstEspecialidades { get; set; } = new List<EspecialidadDTO>();
        private RadzenDataGrid<EspecialidadDTO>? grdEspecialidades = null!;

        protected override async Task OnInitializedAsync()
        {
            await ReloadGrillaAsync();
            await base.OnInitializedAsync();
        }
        private async Task ReloadGrillaAsync()
        {
            lstEspecialidades = await especialidadesBL.GetEspecialidadesAsync();
            grdEspecialidades?.Reload();
        }
        protected async Task AgregarClick()
        {
            var respuesta = await DS.OpenAsync<AddOrEdit>("Agregar Especialidad", null,
                                        new DialogOptions() { Width = "60%", Height = "auto", Resizable = true });

            if (respuesta ?? false)
            {
                await ReloadGrillaAsync();
            }
        }
        private async Task EditarClick(EspecialidadDTO row)
        {

            var dto = await especialidadesBL.GetEspecialidadAsync(row.IdEspecialidad);
            var respuesta = await DS.OpenAsync<AddOrEdit>("Editar Especialidad",
                                        new Dictionary<string, object>() { { "IdEspecialidad", row.IdEspecialidad } },
                                        new DialogOptions() { Width = "60%", Height = "auto", Resizable = true });

            if (respuesta ?? false)
            {
                await ReloadGrillaAsync();
            }

        }
        protected async Task EliminarClick(EspecialidadDTO row)
        {
            try
            {
                var respuesta = await DS.Confirm("¿Está seguro de eliminar el registro?", "Confirmación",
                new ConfirmOptions() { CancelButtonText = "No", OkButtonText = "Sí" });

                if (respuesta ?? false)
                {
                    await especialidadesBL.EliminarEspecialidadAsync(row.IdEspecialidad);
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
                        await especialidadesBL.ActualizarEspecialidadAsync(row);
                        await ReloadGrillaAsync();
                    }
                }
                else
                    notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
            }

        }

    }
}
