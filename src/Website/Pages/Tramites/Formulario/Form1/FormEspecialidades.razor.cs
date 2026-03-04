using Business;
using Business.Extensions;
using Business.Interfaces;
using DataAccess.Entities;
using DataTransferObject;
using DataTransferObject.BLs;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using StaticClass;
using System;

namespace Website.Pages.Tramites.Formulario.Form1
{
    
    public partial class FormEspecialidades: ComponentBase
    {
        [Parameter] public string Accion { get; set; } = null!;
        [Parameter] public string IdentificadorUnico { get; set; } = null!;
        [Inject] private DialogService DS { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private ITramitesBL _TramiteBL { get; set; } = null!;
        [Inject] private ITramitesEvaluacionBL _TramiteEvaluacionBL { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] protected RouteHistoryService routeHistory { get; set; } = null!;
        private bool EvaluandoTramite { get; set; } = false;
        private TramitesDTO tramite { get; set; } = new();
        private List<ItemGrillaEspecialidadesDTO> lstEspecialidades = new List<ItemGrillaEspecialidadesDTO>();
        private RadzenDataGrid<ItemGrillaEspecialidadesDTO>? grdEspecialidades = null;
        private bool PermiteEditar = false;
        protected override async Task OnInitializedAsync()
        {
            EvaluandoTramite = await _TramiteBL.isEvaluandoTramite(IdentificadorUnico);
            
            // Comprueba la seguridad del trámite
            if (!await _TramiteBL.ComprobarSeguridad(IdentificadorUnico))
            {
                navigationManager.NavigateTo("/ValidationError/401", true);
                return;
            }
            //--

            tramite = await _TramiteBL.GetTramiteByGuidAsync(IdentificadorUnico);
            PermiteEditar = tramite.PermiteEditarEmpresa && tramite.PermiteEditarFormulario(Constants.Formularios.Especialidades);


            await ReloadGrillaAsync();
            await base.OnInitializedAsync();

        }
        private async Task ReloadGrillaAsync()
        {
            lstEspecialidades = await _TramiteBL.GetEspecialidadesAsync(tramite.IdTramite);
            grdEspecialidades?.Reload();
        }
        protected async Task AgregarEspecialidadesClick()
        {
            var respuesta = await DS.OpenAsync<FormEspecialidadesAddEdit>("Agregar Especialidad", 
                                        new Dictionary<string, object>() { { "IdentificadorUnico",IdentificadorUnico } },
                                        new DialogOptions() { Width = "75%", Height = "75%", Resizable = true });

            if (respuesta ?? false)
            {
                await ReloadGrillaAsync();
            }
        }
        protected async Task EditarEspecialidadClick(int IdTramiteEspecialidad, int IdSeccion)
        {
            var respuesta = await DS.OpenAsync<FormEspecialidadesAddEdit>("Datos de la Especialidad",
                                        new Dictionary<string, object>() { { "IdentificadorUnico", IdentificadorUnico },
                                                                           { "IdTramiteEspecialidad", IdTramiteEspecialidad },
                                                                           { "IdSeccion", IdSeccion } },
                                        new DialogOptions() { Width = "75%", Height = "75%", Resizable = true });

            if (respuesta ?? false)
            {
                await ReloadGrillaAsync();
            }
        }
        protected async Task EliminarEspecialidadClick(ItemGrillaEspecialidadesSeccionesDTO row)
        {
            var respuesta = await DS.Confirm("¿Está seguro de eliminar el registro?", "Confirmación", 
                new ConfirmOptions() { CancelButtonText = "No", OkButtonText = "Sí" });

            if(respuesta ?? false)
            {
                try
                {
                    await _TramiteBL.EliminarEspecialidadSeccionAsync(row.IdTramiteEspecialidadSeccion);
                    await ReloadGrillaAsync();
                }
                catch (Exception ex)
                {
                    notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.VerySlow);
                }
                
            }
        }
        protected void VolverAlVisor()
        {
                navigationManager.NavigateTo($"/Tramites/Visualizar/{IdentificadorUnico}", true);
        }

        protected async Task GuardarEvaluacionClick(TramiteFormularioEvaluadoDTO dto)
        {
            try
            {
                await _TramiteEvaluacionBL.GuardarEvaluacionAsync(dto);
                VolverAlVisor();
            }
            catch (Exception ex)
            {
                notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.VerySlow);
            }
        }
        protected Task CancelarEvaluacionClick()
        {
            VolverAlVisor();
            return Task.CompletedTask;
        }
        
    }
}
