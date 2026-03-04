using AutoMapper;
using Business;
using Business.Extensions;
using Business.Interfaces;
using DataTransferObject;
using DataTransferObject.BLs;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using StaticClass;
using System.ComponentModel.DataAnnotations;
using Website.Models.Formulario;
using Website.Pages.Shared.Components;

namespace Website.Pages.Tramites.Formulario.Form7
{
    public partial class EquiposIndex: ComponentBase
    {
        [Parameter] public string Accion { get; set; } = null!;
        [Parameter] public string IdentificadorUnico { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] protected RouteHistoryService routeHistory { get; set; } = null!;

        [Inject] private ITramitesBL _TramiteBL { get; set; } = null!;
        [Inject] private ITramitesEvaluacionBL _TramiteEvaluacionBL { get; set; } = null!;
        [Inject] private IUsuariosBL _usuarioBL { get; set; } = null!;
        [Inject] private IFilesBL _filesBL { get; set; } = null!;
        private IMapper _mapper { get; set; } = null!;
        private bool EvaluandoTramite { get; set; } = false;
        private TramitesDTO tramite { get; set; } = new();
        private EquiposAddModel Model { get; set; } = new();
        private List<PerfilDTO> lstPerfiles { get; set; } = new();
        private bool isBusyAceptar { get; set; }

        private CustomFileUpload? uploadDE;
        private List<FileDTO> lstFilesDE = new();
        private bool isBusyFileUploadDE { get; set; }

        private CustomFileUpload? uploadCC;
        private List<FileDTO> lstFilesCC = new();
        private bool isBusyFileUploadCC { get; set; }

        private CustomFileUpload? uploadDoc;
        private List<FileDTO> lstFilesDoc = new();
        private bool isBusyFileUploadDoc { get; set; }


        private CustomFileUpload? uploadDE2 { get; set; } = null!;
        private List<FileDTO> lstFilesDE2 = new();
        private bool isBusyFileUploadDE2 { get; set; }

        private CustomFileUpload? uploadCC2 { get; set; } = null!;
        private List<FileDTO> lstFilesCC2 = new();
        private bool isBusyFileUploadCC2 { get; set; }

        private CustomFileUpload? uploadDoc2;
        private List<FileDTO> lstFilesDoc2 = new();
        private bool isBusyFileUploadDoc2 { get; set; }
        private bool PermiteEditar = false;

        private EditContext context = new EditContext(new EquiposAddModel());

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
            PermiteEditar = tramite.PermiteEditarEmpresa && tramite.PermiteEditarFormulario(Constants.Formularios.Equipos);


            //Crear el mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EquiposItemModel, TramitesEquipoDTO>().ReverseMap();
            });
            _mapper = config.CreateMapper();
            //--

            context = new EditContext(Model);

            string userid = await _usuarioBL.GetCurrentUserid();
            lstPerfiles = await _usuarioBL.GetPerfilesForUserAsync(userid);



            Model.Afectado = _mapper.Map<TramitesEquipoDTO, EquiposItemModel>(await _TramiteBL.GetEquiposAsync(tramite.IdTramite,true));
            Model.NoAfectado = _mapper.Map<TramitesEquipoDTO, EquiposItemModel>(await _TramiteBL.GetEquiposAsync(tramite.IdTramite, false));

            if (Model.Afectado != null)
            {
                lstFilesDE.Add(await _filesBL.GetFileAsync(Model.Afectado.IdFileDetalleEquipo.GetValueOrDefault()));
                lstFilesCC.Add(await _filesBL.GetFileAsync(Model.Afectado.IdFileCertificadoContable.GetValueOrDefault()));
                if(Model.Afectado.IdFileDocumentacionEquipo.HasValue)
                    lstFilesDoc.Add(await _filesBL.GetFileAsync(Model.Afectado.IdFileDocumentacionEquipo.GetValueOrDefault()));
            }
            else
                Model.Afectado = new();

            if (Model.NoAfectado != null)
            {
                lstFilesDE2.Add(await _filesBL.GetFileAsync(Model.NoAfectado.IdFileDetalleEquipo.GetValueOrDefault()));
                lstFilesCC2.Add(await _filesBL.GetFileAsync(Model.NoAfectado.IdFileCertificadoContable.GetValueOrDefault()));
                if (Model.NoAfectado.IdFileDocumentacionEquipo.HasValue)
                    lstFilesDoc2.Add(await _filesBL.GetFileAsync(Model.NoAfectado.IdFileDocumentacionEquipo.GetValueOrDefault()));
            }
            else
                Model.NoAfectado = new();

            Model.Afectado.isEvaluandoTramite = EvaluandoTramite;
            Model.NoAfectado.isEvaluandoTramite = EvaluandoTramite;

            await base.OnInitializedAsync();
        }

        #region AFECTADO
        #region Metodos File Detelle de los Equipos - AFECTADO
        private void HandleFileDeletionRequestedDE(FileDTO fileModel)
        {
            Model.Afectado.IdFileDetalleEquipo = null;
            Model.Afectado.FilenameDetalleEquipo = null;
            lstFilesDE = lstFilesDE.Where(x => x != fileModel).ToList();
        }

        private async Task HandleFileUploadedDE(FileDTO fileModel)
        {


            if (uploadDE?.Accept?.Length > 0 && !uploadDE.Accept.Split(",").Contains(fileModel.ContentType))
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido (Archivos Excel).", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                fileModel.CreateUser = await _usuarioBL.GetCurrentUserid();
                fileModel.CreateDate = DateTime.Now;
                fileModel = await _filesBL.AddFileAsync(fileModel);
                Model.Afectado.IdFileDetalleEquipo = fileModel.IdFile;
                Model.Afectado.FilenameDetalleEquipo = fileModel.FileName;
            }
            isBusyFileUploadDE = false;
            context?.Validate();

        }
        private void HandleErrorDE(string message)
        {
            isBusyFileUploadDE = false;
            notificationService.Notify(NotificationSeverity.Error, "Error", message, StaticClass.Constants.NotifDuration.Normal);
        }
        protected Task OnProgressDE(UploadProgressArgs args)
        {
            if (args.Progress > 0 && args.Progress != 100)
                isBusyFileUploadDE = true;

            return Task.CompletedTask;
        }
        #endregion

        #region Metodos File Certificado Contable - AFECTADO
        private void HandleFileDeletionRequestedCC(FileDTO fileModel)
        {
            Model.Afectado.IdFileCertificadoContable = null;
            Model.Afectado.FilenameCertificadoContable = null;
            lstFilesCC = lstFilesCC.Where(x => x != fileModel).ToList();
        }

        private async Task HandleFileUploadedCC(FileDTO fileModel)
        {

            if (uploadCC?.Accept?.Length > 0 && !uploadCC.Accept.Split(",").Contains(fileModel.ContentType))
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido (Archivos Pdf).", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                fileModel.CreateUser = await _usuarioBL.GetCurrentUserid();
                fileModel.CreateDate = DateTime.Now;
                fileModel = await _filesBL.AddFileAsync(fileModel);
                Model.Afectado.IdFileCertificadoContable = fileModel.IdFile;
                Model.Afectado.FilenameCertificadoContable = fileModel.FileName;
            }
            isBusyFileUploadCC = false;
            context?.Validate();
        }
        private void HandleErrorCC(string message)
        {
            isBusyFileUploadCC = false;
            notificationService.Notify(NotificationSeverity.Error, "Error", message, StaticClass.Constants.NotifDuration.Normal);
        }
        protected Task OnProgressCC(UploadProgressArgs args)
        {
            if (args.Progress > 0 && args.Progress != 100)
                isBusyFileUploadCC = true;

            return Task.CompletedTask;
        }
        #endregion
        #region Metodos File Documentación Equipos - AFECTADO
        private void HandleFileDeletionRequestedDoc(FileDTO fileModel)
        {
            Model.Afectado.IdFileDocumentacionEquipo = null;
            Model.Afectado.FilenameDocumentacionEquipo = null;
            lstFilesDoc = lstFilesDoc.Where(x => x != fileModel).ToList();
        }

        private async Task HandleFileUploadedDoc(FileDTO fileModel)
        {

            if (uploadDoc?.Accept?.Length > 0 && !uploadDoc.Accept.Split(",").Contains(fileModel.ContentType))
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido (Archivos Pdf).", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                fileModel.CreateUser = await _usuarioBL.GetCurrentUserid();
                fileModel.CreateDate = DateTime.Now;
                fileModel = await _filesBL.AddFileAsync(fileModel);
                Model.Afectado.IdFileDocumentacionEquipo= fileModel.IdFile;
                Model.Afectado.FilenameDocumentacionEquipo= fileModel.FileName;
            }
            isBusyFileUploadDoc = false;
            context?.Validate();
        }
        private void HandleErrorDoc(string message)
        {
            isBusyFileUploadDoc = false;
            notificationService.Notify(NotificationSeverity.Error, "Error", message, StaticClass.Constants.NotifDuration.Normal);
        }
        protected Task OnProgressDoc(UploadProgressArgs args)
        {
            if (args.Progress > 0 && args.Progress != 100)
                isBusyFileUploadDoc = true;

            return Task.CompletedTask;
        }
        #endregion
        #endregion
        #region NO AFECTADO
        #region Metodos File Detelle de los Equipos - NO AFECTADO
        private void HandleFileDeletionRequestedDE2(FileDTO fileModel)
        {
            Model.NoAfectado.IdFileDetalleEquipo = null;
            Model.NoAfectado.FilenameDetalleEquipo = null;
            lstFilesDE2 = lstFilesDE2.Where(x => x != fileModel).ToList();
        }

        private async Task HandleFileUploadedDE2(FileDTO fileModel)
        {


            if (uploadDE2?.Accept?.Length > 0 && !uploadDE2.Accept.Split(",").Contains(fileModel.ContentType))
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido (Archivos Excel).", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                fileModel.CreateUser = await _usuarioBL.GetCurrentUserid();
                fileModel.CreateDate = DateTime.Now;
                fileModel = await _filesBL.AddFileAsync(fileModel);
                Model.NoAfectado.IdFileDetalleEquipo = fileModel.IdFile;
                Model.NoAfectado.FilenameDetalleEquipo = fileModel.FileName;
            }
            isBusyFileUploadDE2 = false;
            context?.Validate();

        }
        private void HandleErrorDE2(string message)
        {
            isBusyFileUploadDE2 = false;
            notificationService.Notify(NotificationSeverity.Error, "Error", message, StaticClass.Constants.NotifDuration.Normal);
        }
        protected Task OnProgressDE2(UploadProgressArgs args)
        {
            if (args.Progress > 0 && args.Progress != 100)
                isBusyFileUploadDE2 = true;

            return Task.CompletedTask;
        }
        #endregion

        #region Metodos File Certificado Contable - NO AFECTADO
        private void HandleFileDeletionRequestedCC2(FileDTO fileModel)
        {
            Model.NoAfectado.IdFileCertificadoContable = null;
            Model.NoAfectado.FilenameCertificadoContable = null;
            lstFilesCC2 = lstFilesCC2.Where(x => x != fileModel).ToList();
        }

        private async Task HandleFileUploadedCC2(FileDTO fileModel)
        {

            if (uploadCC2?.Accept?.Length > 0 && !uploadCC2.Accept.Split(",").Contains(fileModel.ContentType))
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido (Archivos Pdf).", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                fileModel.CreateUser = await _usuarioBL.GetCurrentUserid();
                fileModel.CreateDate = DateTime.Now;
                fileModel = await _filesBL.AddFileAsync(fileModel);
                Model.NoAfectado.IdFileCertificadoContable = fileModel.IdFile;
                Model.NoAfectado.FilenameCertificadoContable = fileModel.FileName;
            }
            isBusyFileUploadCC2 = false;
            context?.Validate();
        }
        private void HandleErrorCC2(string message)
        {
            isBusyFileUploadCC2 = false;
            notificationService.Notify(NotificationSeverity.Error, "Error", message, StaticClass.Constants.NotifDuration.Normal);
        }
        protected Task OnProgressCC2(UploadProgressArgs args)
        {
            if (args.Progress > 0 && args.Progress != 100)
                isBusyFileUploadCC2 = true;

            return Task.CompletedTask;
        }
        #endregion
        #region Metodos File Documentación Equipos - NO AFECTADO
        private void HandleFileDeletionRequestedDoc2(FileDTO fileModel)
        {
            Model.NoAfectado.IdFileDocumentacionEquipo = null;
            Model.NoAfectado.FilenameDocumentacionEquipo = null;
            lstFilesDoc2 = lstFilesDoc2.Where(x => x != fileModel).ToList();
        }

        private async Task HandleFileUploadedDoc2(FileDTO fileModel)
        {

            if (uploadDoc2?.Accept?.Length > 0 && !uploadDoc2.Accept.Split(",").Contains(fileModel.ContentType))
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido (Archivos Pdf).", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                fileModel.CreateUser = await _usuarioBL.GetCurrentUserid();
                fileModel.CreateDate = DateTime.Now;
                fileModel = await _filesBL.AddFileAsync(fileModel);
                Model.NoAfectado.IdFileDocumentacionEquipo = fileModel.IdFile;
                Model.NoAfectado.FilenameDocumentacionEquipo = fileModel.FileName;
            }
            isBusyFileUploadDoc = false;
            context?.Validate();
        }
        private void HandleErrorDoc2(string message)
        {
            isBusyFileUploadDoc2 = false;
            notificationService.Notify(NotificationSeverity.Error, "Error", message, StaticClass.Constants.NotifDuration.Normal);
        }
        protected Task OnProgressDoc2(UploadProgressArgs args)
        {
            if (args.Progress > 0 && args.Progress != 100)
                isBusyFileUploadDoc2 = true;

            return Task.CompletedTask;
        }
        #endregion
        #endregion

        protected async Task OnClickAceptar(EditContext Ed)
        {
            if (isBusyAceptar)
                return;

            context = Ed;
            isBusyAceptar = true;
            if (Ed != null && Ed.Validate())
            {
                
                //Alta
                try
                {
                    var lstdto = new List<TramitesEquipoDTO>();
                    var dto1 = _mapper.Map<EquiposItemModel, TramitesEquipoDTO>(Model.Afectado);
                    dto1.Afectado = true;
                    dto1.MontoRealizacionEvaluador = dto1.MontoRealizacion;
                    dto1.IdTramite = tramite.IdTramite;

                    var dto2 = _mapper.Map<EquiposItemModel, TramitesEquipoDTO>(Model.NoAfectado);
                    dto2.Afectado = false;
                    dto2.MontoRealizacionEvaluador = dto2.MontoRealizacion;
                    dto2.IdTramite = tramite.IdTramite;
                    
                    lstdto.Add(dto1);

                    if (dto2.MontoRealizacion > 0 && dto2.IdFileCertificadoContable > 0 && dto2.IdFileDetalleEquipo > 0)
                        lstdto.Add(dto2);
                    else
                        await _TramiteBL.EliminarEquipoAsync(tramite.IdTramite, false);

                    await _TramiteBL.ActualizarEquipoAsync(lstdto);

                    notificationService.Notify(NotificationSeverity.Success, "Aviso", "La información se ha almacenado correctamente.", StaticClass.Constants.NotifDuration.Normal);
                    VolverAlVisor();

                }
                catch (Exception ex)
                {
                    notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
                }
            }
            isBusyAceptar = false;
        }
        protected void VolverAlVisor()
        {
                navigationManager.NavigateTo($"/Tramites/Visualizar/{IdentificadorUnico}", true);
        }
        protected async Task GuardarEvaluacionClick(TramiteFormularioEvaluadoDTO dto)
        {
            try
            {
                if (context.Validate())
                {

                    await _TramiteEvaluacionBL.ActualizarEquiposMontoRealizacionEvaluador(tramite.IdTramite,
                                                                           Model.Afectado.MontoRealizacionEvaluador.GetValueOrDefault(),
                                                                           Model.NoAfectado.MontoRealizacionEvaluador);

                    await _TramiteEvaluacionBL.GuardarEvaluacionAsync(dto);
                    VolverAlVisor();
                }
            }
            catch (Exception ex)
            {
                notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.VerySlow);
            }
        }
 
        protected  Task CancelarEvaluacionClick()
        {
            VolverAlVisor();
            return Task.CompletedTask;
        }
    }
}
