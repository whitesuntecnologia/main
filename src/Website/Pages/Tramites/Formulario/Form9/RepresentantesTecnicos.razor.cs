using Business.Interfaces;
using DataTransferObject.BLs;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen;
using Business;
using Microsoft.JSInterop;
using StaticClass;
using Business.Extensions;
using Website.Models.Formulario;

namespace Website.Pages.Tramites.Formulario.Form9
{
    public partial class RepresentantesTecnicos: ComponentBase
    {

        [Parameter] public string IdentificadorUnico { get; set; } = null!;
        [Parameter] public string Accion { get; set; } = null!; //Ingresar o Editar

        [Inject] private DialogService DS { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private ITramitesBL _TramiteBL { get; set; } = null!;
        [Inject] private ITramitesEvaluacionBL _TramiteEvaluacionBL { get; set; } = null!;
        [Inject] private IFilesBL _FilesBL { get; set; } = null!;
        [Inject] IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] protected RouteHistoryService routeHistory { get; set; } = null!;
        private bool EvaluandoTramite { get; set; } = false;
        private TramitesDTO tramite { get; set; } = new();
        private List<TramitesRepresentanteTecnicoDTO> lstRepresentantes = new List<TramitesRepresentanteTecnicoDTO>();
        private List<TramitesRepresentanteTecnicoDTO> lstRepresentantesDesvinculados = new List<TramitesRepresentanteTecnicoDTO>();
        private RadzenDataGrid<TramitesRepresentanteTecnicoDTO>? grdRepresentantes = null;
        private RadzenDataGrid<TramitesRepresentanteTecnicoDTO>? grdRepresentantesDesvinculados = null;
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
            PermiteEditar = tramite.PermiteEditarEmpresa && tramite.PermiteEditarFormulario(Constants.Formularios.RepresentantesTecnicos);

            await ReloadGrillaAsync();
            await base.OnInitializedAsync();
        }
        private async Task ReloadGrillaAsync()
        {
            var representantesTecnicos = await _TramiteBL.GetRepresentantesAsync(tramite.IdTramite);
            lstRepresentantes = representantesTecnicos.Where(x=> !x.EstaDesvinculado).ToList();
            lstRepresentantesDesvinculados = representantesTecnicos.Where(x => x.EstaDesvinculado).ToList();
            if (grdRepresentantes != null)
                await grdRepresentantes.Reload();
            else
                StateHasChanged();

            if (grdRepresentantesDesvinculados != null)
                await grdRepresentantesDesvinculados.Reload();
            else
                StateHasChanged();

        }
        protected Task AgregarClick()
        {
            navigationManager.NavigateTo($"/Tramites/RepresentantesTecnicos/{Accion}/{IdentificadorUnico}/Agregar", true);
            return Task.CompletedTask;
        }
        protected Task EditarClick(TramitesRepresentanteTecnicoDTO row)
        {
            navigationManager.NavigateTo($"/Tramites/RepresentantesTecnicos/{Accion}/{IdentificadorUnico}/Editar/{row.IdRepresentanteTecnico}");
            return Task.CompletedTask;
        }

        protected async Task EliminarClick(TramitesRepresentanteTecnicoDTO row)
        {
            var respuesta = await DS.Confirm("¿Está seguro de eliminar el registro?", "Confirmación",
                new ConfirmOptions() { CancelButtonText = "No", OkButtonText = "Sí" });

            if (respuesta ?? false)
            {
                await _TramiteBL.EliminarRepresentanteTecnicoAsync(row.IdRepresentanteTecnico);
                await ReloadGrillaAsync();
            }
        }

        private async Task OnDownloadFileAsync(int fileId)
        {
            var archivoDTO = await _FilesBL.GetFileAsync(fileId);
            byte[] file = archivoDTO.ContentFile;
            string fileName = archivoDTO.FileName;
            string contentType = archivoDTO.ContentType;

            await JSRuntime.InvokeVoidAsync("BlazorDownloadFile", fileName, contentType, file);
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
        protected async Task EliminarDesvinculadionClick(TramitesRepresentanteTecnicoDTO row)
        {
            var respuesta = await DS.Confirm("¿Está seguro de eliminar la desvinculación?", "Confirmación",
                new ConfirmOptions() { CancelButtonText = "No", OkButtonText = "Sí" });

            if (respuesta ?? false)
            {
                await _TramiteBL.EliminarDesvinculacionRepresentanteTecnicoAsync(row.IdRepresentanteTecnico);
                await ReloadGrillaAsync();
            }
        }
        protected async Task DesvincularClick(TramitesRepresentanteTecnicoDTO row)
        {

            var desvinculacion = new RepresentanteTecnicoDesvinculacionModel();
            desvinculacion.IdRepresentanteTecnico = row.IdRepresentanteTecnico;
            
            var result = await DS.OpenAsync<RepresentanteTecnicoDesvincular>("Desvincular Representante Técnico",
                new Dictionary<string, object> { { "Model", desvinculacion } },
                new DialogOptions()
                {
                    Width = "1000px",
                    Height = "auto",
                    Resizable = true,
                    Draggable = true
                });

            if (result != null && result is RepresentanteTecnicoDesvinculacionModel)
            {
                try
                {
                    // Crear el DTO para guardar la desvinculación
                    var desvinculacionDTO = new TramitesRepresentantesTecnicosDesvinculacionesDTO
                    {
                        IdRepresentanteTecnico = result!.IdRepresentanteTecnico,
                        IdFileDesvinculacion = result!.IdFileDesvinculacion,
                        FilenameDesvinculacion = result!.FilenameDesvinculacion,
                    };

                    // Llamar a la BL para guardar la desvinculación
                    await _TramiteBL.GuardarDesvinculacionRepresentanteTecnicoAsync(desvinculacionDTO);

                    // Recargar la grilla
                    await ReloadGrillaAsync();

                    // Mostrar notificación de éxito
                    notificationService.Notify(NotificationSeverity.Success,
                        "Éxito",
                        "El representante técnico ha sido desvinculado correctamente.",
                        StaticClass.Constants.NotifDuration.Normal);
                }
                catch (Exception ex)
                {
                    notificationService.Notify(NotificationSeverity.Error,
                        "Error",
                        Functions.GetErrorMessage(ex),
                        StaticClass.Constants.NotifDuration.Slow);
                }
            }

        }
    }
}
