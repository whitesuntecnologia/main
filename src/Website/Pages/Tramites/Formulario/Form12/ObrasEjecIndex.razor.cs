using AutoMapper;
using Business.Interfaces;
using DataTransferObject.BLs;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen.Blazor;
using Radzen;
using Website.Models.Formulario;
using StaticClass;
using Business.Extensions;

namespace Website.Pages.Tramites.Formulario.Form12
{
    public partial class ObrasEjecIndex: ComponentBase
    {
        [Parameter] public string Accion { get; set; } = null!;
        [Parameter] public string IdentificadorUnico { get; set; } = null!;
        [Inject] private DialogService DS { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] protected RouteHistoryService routeHistory { get; set; } = null!;

        [Inject] private ITramitesBL _TramiteBL { get; set; } = null!;
        [Inject] private ITramitesEvaluacionBL _TramiteEvaluacionBL { get; set; } = null!;
        [Inject] private IFilesBL _FilesBL { get; set; } = null!;
        private IMapper _mapper { get; set; } = null!;
        private TramitesDTO tramite { get; set; } = new();
        private bool EvaluandoTramite { get; set; } = false;
        private List<TramitesObraEjecucionDTO> lstObras { get; set; } = new();
        private RadzenDataGrid<TramitesObraEjecucionDTO>? grdObras = null;
        private bool IsBusyDownload = false;
        private bool PermiteEditar = false;
        private bool _SinDatosForm = false;
        private bool SinDatosForm
        {
            get { return _SinDatosForm; }
            set
            {
                _SinDatosForm = value;
                tramite.SinDatosForm12 = value;
                _TramiteBL.ActualizarTramiteSinDatosFormAsync(tramite.IdTramite, 12, value);
            }
        }
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

            //Crear el mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ObraEjecucionModel, TramitesObraEjecucionDTO>().ReverseMap();
            });
            _mapper = config.CreateMapper();
            //--

            tramite = await _TramiteBL.GetTramiteByGuidAsync(IdentificadorUnico);
            PermiteEditar = tramite.PermiteEditarEmpresa && tramite.PermiteEditarFormulario(Constants.Formularios.ObrasEnEjecucion);
            SinDatosForm = tramite.SinDatosForm12;
            await ReloadGrillaAsync();
            await base.OnInitializedAsync();
        }

        protected Task AgregarObraClick()
        {
            navigationManager.NavigateTo($"/Tramites/ObrasEjecucion/{Accion}/{IdentificadorUnico}/Agregar", true);
            return Task.CompletedTask;
        }
        protected Task EditarObraClick(TramitesObraEjecucionDTO row)
        {
            navigationManager.NavigateTo($"/Tramites/ObrasEjecucion/{Accion}/{IdentificadorUnico}/Editar/{row.IdTramiteObraEjec}");
            return Task.CompletedTask;
        }

        private async Task ReloadGrillaAsync()
        {
            lstObras = await _TramiteBL.GetObrasEjecucionAsync(tramite.IdTramite);
            grdObras?.Reload();
        }
        protected async Task EliminarObraClick(TramitesObraEjecucionDTO row)
        {
            var respuesta = await DS.Confirm("¿Está seguro de eliminar el registro?", "Confirmación",
                new ConfirmOptions() { CancelButtonText = "No", OkButtonText = "Sí" });

            if (respuesta ?? false)
            {
                await _TramiteBL.EliminarObraEjecucionAsync(row.IdTramiteObraEjec);
                await ReloadGrillaAsync();
            }
        }

        private async Task OnDownloadFileAsync(int fileId)
        {
            IsBusyDownload = true;

            var archivoDTO = await _FilesBL.GetFileAsync(fileId);
            byte[] file = archivoDTO.ContentFile;
            string fileName = archivoDTO.FileName;
            string contentType = archivoDTO.ContentType;

            await JSRuntime.InvokeVoidAsync("BlazorDownloadFile", fileName, contentType, file);
            IsBusyDownload = false;
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
