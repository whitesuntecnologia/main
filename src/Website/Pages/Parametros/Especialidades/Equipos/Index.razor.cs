using Business.Interfaces;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen;
using StaticClass;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace Website.Pages.Parametros.Especialidades.Equipos
{
    public partial class Index: ComponentBase
    {
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private DialogService DS { get; set; } = null!;
        [Inject] private IEspecilidadesBL especialidadesBL { get; set; } = null!;

        private List<EspecialidadEquipoDTO> lstEquipos { get; set; } = new();
        private RadzenDataGrid<EspecialidadEquipoDTO>? grdEquipos = null!;

        protected override async Task OnInitializedAsync()
        {
            await ReloadGrillaAsync();
            await base.OnInitializedAsync();
        }
        private async Task ReloadGrillaAsync()
        {
            lstEquipos = await especialidadesBL.GetEspecialidadesEquiposAsync();
            grdEquipos?.Reload();
        }
        protected async Task AgregarClick()
        {
            var respuesta = await DS.OpenAsync<AddOrEdit>("Agregar Equipo", null,
                                        new DialogOptions() { Width = "60%", Height = "auto", Resizable = true });

            if (respuesta ?? false)
            {
                await ReloadGrillaAsync();
            }
        }
        private async Task EditarClick(EspecialidadEquipoDTO row)
        {

            var dto = await especialidadesBL.GetEspecialidadEquipoAsync(row.IdEquipo);
            var respuesta = await DS.OpenAsync<AddOrEdit>("Editar Equipo",
                                        new Dictionary<string, object>() { { "IdEquipo", row.IdEquipo } },
                                        new DialogOptions() { Width = "60%", Height = "auto", Resizable = true });

            if (respuesta ?? false)
            {
                await ReloadGrillaAsync();
            }

        }
        protected async Task EliminarClick(EspecialidadEquipoDTO row)
        {
            try
            {
                var respuesta = await DS.Confirm("¿Está seguro de eliminar el registro?", "Confirmación",
                new ConfirmOptions() { CancelButtonText = "No", OkButtonText = "Sí" });

                if (respuesta ?? false)
                {
                    await especialidadesBL.EliminarEquipoAsync(row.IdEquipo);
                    await ReloadGrillaAsync();
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException is SqlException)
                {
                    SqlException sqlEx = (SqlException)ex.InnerException;
                    if(sqlEx.Number == Constants.SqlErroNumbers.ErrorRegistroRelacionado)
                    {
                        row.Baja = true;
                        await especialidadesBL.ActualizarEquipoAsync(row);
                        await ReloadGrillaAsync();
                    }
                }
                else
                    notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
            }
            
        }
    }
}
