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
using System;

namespace Website.Pages.Tramites.Formulario.Form10
{
    public partial class ObrasIndex: ComponentBase
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
        private bool EvaluandoTramite { get; set; } = false;
        private TramitesDTO tramite { get; set; } = new();
        private List<TramitesObrasDTO> lstObras { get; set; } = new();
        private RadzenDataGrid<TramitesObrasDTO>? grdObras = null;
        private bool PermiteEditar = false;
        private bool IsBusyDownload = false;
        private bool _SinDatosForm = false;
        private bool SinDatosForm
        {
            get { return _SinDatosForm; }
            set
            {
                _SinDatosForm = value;
                tramite.SinDatosForm10 = value;
                _TramiteBL.ActualizarTramiteSinDatosFormAsync(tramite.IdTramite,10,value);
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
            
            tramite = await _TramiteBL.GetTramiteByGuidAsync(IdentificadorUnico);
            PermiteEditar = tramite.PermiteEditarEmpresa && tramite.PermiteEditarFormulario(Constants.Formularios.Obras);


            //Crear el mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ObrasModel, TramitesObrasDTO>().ReverseMap();
            });
            _mapper = config.CreateMapper();
            //--
            
            
            SinDatosForm = tramite.SinDatosForm10;

            await ReloadGrillaAsync();
            await base.OnInitializedAsync();
        }
        
        protected Task AgregarObraClick()
        {
            navigationManager.NavigateTo($"/Tramites/Obras/{Accion}/{IdentificadorUnico}/Agregar", true);
            return Task.CompletedTask;
        }
        protected Task EditarObraClick(TramitesObrasDTO row)
        {
            navigationManager.NavigateTo($"/Tramites/Obras/{Accion}/{IdentificadorUnico}/Editar/{row.IdTramiteObra}");
            return Task.CompletedTask;
        }

        private async Task ReloadGrillaAsync()
        {
            lstObras = await _TramiteBL.GetObrasAsync(tramite.IdTramite);
            grdObras?.Reload();
        }
        protected async Task EliminarObraClick(TramitesObrasDTO row)
        {
            var respuesta = await DS.Confirm("¿Está seguro de eliminar el registro?", "Confirmación",
                new ConfirmOptions() { CancelButtonText = "No", OkButtonText = "Sí" });

            if (respuesta ?? false)
            {
                await _TramiteBL.EliminarObraAsync(row.IdTramiteObra);
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

        protected Task OnChangechkSinDatos()
        {

            return Task.CompletedTask;
        }
    }
}
