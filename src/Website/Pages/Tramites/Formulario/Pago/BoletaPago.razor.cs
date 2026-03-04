using AutoMapper;
using Azure.Core;
using Business;
using Business.Extensions;
using Business.Interfaces;
using DataAccess.Entities;
using DataTransferObject;
using DataTransferObject.BLs;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using StaticClass;
using Website.Models.Formulario;
using Website.Pages.Shared.Components;

namespace Website.Pages.Tramites.Formulario.Pago
{
    public partial class BoletaPago: ComponentBase
    {
        [Parameter] public string Accion { get; set; } = null!;
        [Parameter] public string IdentificadorUnico { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private DialogService DS { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] protected RouteHistoryService routeHistory { get; set; } = null!;

        [Inject] private ITramitesBL _TramiteBL { get; set; } = null!;
        [Inject] private ITramitesEvaluacionBL _TramiteEvaluacionBL { get; set; } = null!;
        [Inject] private IUsuariosBL _usuarioBL { get; set; } = null!;
        [Inject] private IFilesBL _filesBL { get; set; } = null!;

        private IMapper _mapper { get; set; } = null!;

        private TramitesDTO tramite { get; set; } = new();
        private BoletaPagoModel Model { get; set; } = new();
        private CustomFileUpload? upload;
        private List<FileDTO> lstFiles = new();
        private CustomFileUpload? uploadCF;
        private List<FileDTO> lstFilesCF = new();

        private bool EvaluandoTramite { get; set; } = false;
        private bool PermiteEditar = false;
        private bool isBusyFileUpload { get; set; }
        private bool IsBusyGuardar { get; set; }
        
        private EditContext? context;

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
                cfg.CreateMap<BoletaPagoModel, TramitesDTO>().ReverseMap();
            });
            _mapper = config.CreateMapper();
            //--

            tramite = await _TramiteBL.GetTramiteByGuidAsync(IdentificadorUnico);
            PermiteEditar = tramite.PermiteEditarEmpresa && tramite.PermiteEditarFormulario(Constants.Formularios.BoletaPago);

            Model = _mapper.Map<BoletaPagoModel>(tramite);
            if(Model.IdFileBoleta.HasValue)
                lstFiles.Add(await _filesBL.GetFileAsync(Model.IdFileBoleta.GetValueOrDefault()));

            if (Model.IdFileCumplimientoFiscal.HasValue)
                lstFilesCF.Add(await _filesBL.GetFileAsync(Model.IdFileCumplimientoFiscal.GetValueOrDefault()));

            await base.OnInitializedAsync();
        }

        #region Metodos File Boleta
        private void HandleFileDeletionRequested(FileDTO fileModel)
        {
            Model.IdFileBoleta = null;
            Model.FilenameBoleta = null;
            lstFiles = lstFiles.Where(x => x != fileModel).ToList();
        }

        private async Task HandleFileUploaded(FileDTO fileModel)
        {

            if (upload?.Accept?.Length > 0 && !upload.Accept.Split(",").Contains(fileModel.ContentType))
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido.", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                fileModel.CreateUser = await _usuarioBL.GetCurrentUserid();
                fileModel.CreateDate = DateTime.Now;
                fileModel = await _filesBL.AddFileAsync(fileModel);
                Model.IdFileBoleta = fileModel.IdFile;
                Model.FilenameBoleta = fileModel.FileName;
            }
            isBusyFileUpload = false;
            context?.NotifyFieldChanged(context.Field("IdFileBoleta"));

        }
        private void HandleError(string message)
        {
            isBusyFileUpload = false;
            notificationService.Notify(NotificationSeverity.Error, "Error", message, StaticClass.Constants.NotifDuration.Normal);
        }
        protected Task OnProgress(UploadProgressArgs args)
        {
            if (args.Progress > 0 && args.Progress != 100)
                isBusyFileUpload = true;

            return Task.CompletedTask;
        }
        #endregion
        #region Metodos File Cumplimiento Fiscal
        private void HandleFileDeletionRequestedCF(FileDTO fileModel)
        {
            Model.IdFileCumplimientoFiscal = null;
            Model.FilenameCumplimientoFiscal = null;
            lstFilesCF = lstFilesCF.Where(x => x != fileModel).ToList();
        }

        private async Task HandleFileUploadedCF(FileDTO fileModel)
        {

            if (uploadCF?.Accept?.Length > 0 && !uploadCF.Accept.Split(",").Contains(fileModel.ContentType))
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido.", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                fileModel.CreateUser = await _usuarioBL.GetCurrentUserid();
                fileModel.CreateDate = DateTime.Now;
                fileModel = await _filesBL.AddFileAsync(fileModel);
                Model.IdFileCumplimientoFiscal = fileModel.IdFile;
                Model.FilenameCumplimientoFiscal = fileModel.FileName;
            }
            isBusyFileUpload = false;
            context?.NotifyFieldChanged(context.Field("IdFileCumplimientoFiscal"));

        }
        private void HandleErrorCF(string message)
        {
            isBusyFileUpload = false;
            notificationService.Notify(NotificationSeverity.Error, "Error", message, StaticClass.Constants.NotifDuration.Normal);
        }
        protected Task OnProgressCF(UploadProgressArgs args)
        {
            if (args.Progress > 0 && args.Progress != 100)
                isBusyFileUpload = true;

            return Task.CompletedTask;
        }
        #endregion

        private Task ReValidate(string fieldName)
        {
            if (context != null)
            {
                FieldIdentifier field = context.Field(fieldName);
                context?.NotifyFieldChanged(field);
            }
            return Task.CompletedTask;
        }

        protected Task VolverAlVisor()
        {
            navigationManager.NavigateTo($"/Tramites/{Accion}/{IdentificadorUnico}", true); 
            return Task.CompletedTask;
        }

        protected async Task OnClickGuardar(EditContext Ed)
        {
            if (IsBusyGuardar)
                return;

            IsBusyGuardar = true;
            context = Ed;
            if (Ed != null && Ed.Validate())
            {
                try
                {
                    tramite.IdFileBoleta = Model.IdFileBoleta;
                    tramite.FilenameBoleta = Model.FilenameBoleta;
                    tramite.IdFileCumplimientoFiscal = Model.IdFileCumplimientoFiscal;
                    tramite.FilenameCumplimientoFiscal = Model.FilenameCumplimientoFiscal;
                    tramite.NroBoleta = Model.NroBoleta;

                    await _TramiteBL.ActualizarTramiteAsync(tramite);

                    VolverAlVisor();
                }
                catch (Exception ex)
                {
                    notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
                }
            }
            else
            {
                notificationService.Notify(NotificationSeverity.Error, "Aviso", "Revise los campos inválidos.", StaticClass.Constants.NotifDuration.Normal);
            }
            IsBusyGuardar = false;
        }
        protected async Task GuardarEvaluacionClick(TramiteFormularioEvaluadoDTO dto)
        {
            try
            {
                await _TramiteEvaluacionBL.GuardarEvaluacionAsync(dto);
                await VolverAlVisor();
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
