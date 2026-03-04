using AutoMapper;
using Business.Interfaces;
using DataTransferObject.BLs;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen.Blazor;
using Radzen;
using Website.Models.Formulario;
using Website.Pages.Tramites.Formulario.Form7;
using Microsoft.AspNetCore.Components.Forms;
using Website.Pages.Shared.Components;
using StaticClass;
using Business;
using Business.Extensions;

namespace Website.Pages.Tramites.Formulario.Form8
{
    public partial class BRaicesIndex: ComponentBase
    {
        [Parameter] public string Accion { get; set; } = null!;
        [Parameter] public string IdentificadorUnico { get; set; } = null!;
        [Inject] private DialogService dialogService { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] protected RouteHistoryService routeHistory { get; set; } = null!;

        [Inject] private ITramitesBL _TramiteBL { get; set; } = null!;
        [Inject] private ITramitesEvaluacionBL _TramiteEvaluacionBL { get; set; } = null!;
        [Inject] private ICombosBL _CombosBL { get; set; } = null!;
        [Inject] private IUsuariosBL _usuarioBL { get; set; } = null!;
        [Inject] private ITablasBL _tablasBL { get; set; } = null!;
        [Inject] private IFilesBL _filesBL { get; set; } = null!;
        
        private IMapper _mapper { get; set; } = null!;
        private bool EvaluandoTramite { get; set; } = false;
        private TramitesDTO tramite { get; set; } = new();
        private BienesRaicesModel Model { get; set; } = new();
        private bool isBusyAceptar { get; set; }
        private CustomFileUpload? uploadDE1;
        private List<FileDTO> lstFilesDE1 = new();
        private bool isBusyFileUploadDE1 { get; set; }

        private CustomFileUpload? uploadCC1;
        private List<FileDTO> lstFilesCC1 = new();
        private bool isBusyFileUploadCC1 { get; set; }


        private CustomFileUpload? uploadDE2;
        private List<FileDTO> lstFilesDE2 = new();
        private bool isBusyFileUploadDE2 { get; set; }

        private CustomFileUpload? uploadCC2;
        private List<FileDTO> lstFilesCC2 = new();
        private bool isBusyFileUploadCC2 { get; set; }

        private EditContext context = new EditContext(new BienesRaicesModel());
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
            PermiteEditar = tramite.PermiteEditarEmpresa && tramite.PermiteEditarFormulario(Constants.Formularios.BienesRaices);


            //Crear el mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BienesRaicesItemModel, TramitesBienesRaicesDTO>().ReverseMap();
            });
            _mapper = config.CreateMapper();
            //--

            context = new EditContext(Model);
            Model.OnChageSinDatosForm += HandleCheckboxChange;
            Model.Afectado =  _mapper.Map<TramitesBienesRaicesDTO, BienesRaicesItemModel>( await _TramiteBL.GetBienesRaicesAsync(tramite.IdTramite,true) );
            Model.NoAfectado =  _mapper.Map<TramitesBienesRaicesDTO, BienesRaicesItemModel>(await _TramiteBL.GetBienesRaicesAsync(tramite.IdTramite, false));
            Model.SinDatosForm8 = tramite.SinDatosForm8;

            if (Model.Afectado != null)
            {
                lstFilesDE1.Add(await _filesBL.GetFileAsync(Model.Afectado.IdFileDetalleInmueble.GetValueOrDefault()));
                lstFilesCC1.Add(await _filesBL.GetFileAsync(Model.Afectado.IdFileCertificadoContable.GetValueOrDefault()));
            }
            else
                Model.Afectado = new();

            if (Model.NoAfectado != null)
            {
                lstFilesDE2.Add(await _filesBL.GetFileAsync(Model.NoAfectado.IdFileDetalleInmueble.GetValueOrDefault()));
                lstFilesCC2.Add(await _filesBL.GetFileAsync(Model.NoAfectado.IdFileCertificadoContable.GetValueOrDefault()));
            }
            else
                Model.NoAfectado = new();

            Model.Afectado.isEvaluandoTramite = EvaluandoTramite;
            Model.NoAfectado.isEvaluandoTramite = EvaluandoTramite;

            await base.OnInitializedAsync();
        }
        private void HandleCheckboxChange(bool newValue)
        {
            if (newValue)
            {
                lstFilesDE1.Clear();
                //lstFilesDE2.Clear();
                lstFilesCC1.Clear();
                //lstFilesCC2.Clear();
            }
        }
        

        #region Metodos File Detelle de los Equipos - Afectado
        private void HandleFileDeletionRequestedDE(FileDTO fileModel)
        {
            Model.Afectado.IdFileDetalleInmueble = null;
            Model.Afectado.FilenameDetalleInmueble = null;
            lstFilesDE1 = lstFilesDE1.Where(x => x != fileModel).ToList();
        }

        private async Task HandleFileUploadedDE(FileDTO fileModel)
        {

            if (uploadDE1?.Accept?.Length > 0 && !uploadDE1.Accept.Split(",").Contains(fileModel.ContentType))
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido (Archivos Excel).", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                fileModel.CreateUser = await _usuarioBL.GetCurrentUserid();
                fileModel.CreateDate = DateTime.Now;
                fileModel = await _filesBL.AddFileAsync(fileModel);
                Model.Afectado.IdFileDetalleInmueble = fileModel.IdFile;
                Model.Afectado.FilenameDetalleInmueble = fileModel.FileName;
            }
            isBusyFileUploadDE1 = false;
            context?.Validate();

        }
        private void HandleErrorDE(string message)
        {
            isBusyFileUploadDE1 = false;
            notificationService.Notify(NotificationSeverity.Error, "Error", message, StaticClass.Constants.NotifDuration.Normal);
        }
        protected Task OnProgressDE(UploadProgressArgs args)
        {
            if (args.Progress > 0 && args.Progress != 100)
                isBusyFileUploadDE1 = true;

            return Task.CompletedTask;
        }
        #endregion

        #region Metodos File Certificado Contable - Afectado
        private void HandleFileDeletionRequestedCC(FileDTO fileModel)
        {
            Model.Afectado.IdFileCertificadoContable = null;
            Model.Afectado.FilenameCertificadoContable = null;
            lstFilesCC1 = lstFilesCC1.Where(x => x != fileModel).ToList();
        }

        private async Task HandleFileUploadedCC(FileDTO fileModel)
        {

            if (uploadCC1?.Accept?.Length > 0 && !uploadCC1.Accept.Split(",").Contains(fileModel.ContentType))
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
            isBusyFileUploadCC1 = false;
            context?.Validate();
        }
        private void HandleErrorCC(string message)
        {
            isBusyFileUploadCC1 = false;
            notificationService.Notify(NotificationSeverity.Error, "Error", message, StaticClass.Constants.NotifDuration.Normal);
        }
        protected Task OnProgressCC(UploadProgressArgs args)
        {
            if (args.Progress > 0 && args.Progress != 100)
                isBusyFileUploadCC1 = true;

            return Task.CompletedTask;
        }
        #endregion

        #region Metodos File Detelle de los Equipos - NO Afectado
        private void HandleFileDeletionRequestedDE2(FileDTO fileModel)
        {
            Model.NoAfectado.IdFileDetalleInmueble = null;
            Model.NoAfectado.FilenameDetalleInmueble = null;
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
                Model.NoAfectado.IdFileDetalleInmueble = fileModel.IdFile;
                Model.NoAfectado.FilenameDetalleInmueble = fileModel.FileName;
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

        #region Metodos File Certificado Contable - NO Afectado
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

        protected async Task OnClickAceptar( EditContext Ed)
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
                    var lstdto = new List<TramitesBienesRaicesDTO>();
                    var dto1 = _mapper.Map<BienesRaicesItemModel, TramitesBienesRaicesDTO>(Model.Afectado);
                    dto1.Afectado = true;
                    dto1.MontoRealizacionEvaluador = dto1.MontoRealizacion;
                    dto1.IdTramite = tramite.IdTramite;

                    var dto2 = _mapper.Map<BienesRaicesItemModel, TramitesBienesRaicesDTO>(Model.NoAfectado);
                    dto2.Afectado = false;
                    dto2.MontoRealizacionEvaluador = dto2.MontoRealizacion;
                    dto2.IdTramite = tramite.IdTramite;


                    if(!Model.SinDatosForm8)
                        lstdto.Add(dto1);

                    if (dto2.MontoRealizacion > 0 && dto2.IdFileCertificadoContable > 0 && dto2.IdFileDetalleInmueble > 0)
                        lstdto.Add(dto2);
                    //else
                    //    await _TramiteBL.EliminarBienRaizAsync(tramite.IdTramite, false);

                    await _TramiteBL.ActualizarBienRaizAsync(lstdto,tramite.IdTramite, Model.SinDatosForm8);

                    notificationService.Notify(NotificationSeverity.Success, "Aviso", "La información se ha almacenado correctamente.", StaticClass.Constants.NotifDuration.Normal);
                    VolverAlVisor();
                }
                catch (Exception ex)
                {
                    notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
                    isBusyAceptar = false;
                }
            }
            else
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
                    await _TramiteEvaluacionBL.ActualizarBienesRaicesMontoRealizacionEvaluador(tramite.IdTramite,
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
        protected Task CancelarEvaluacionClick()
        {
            VolverAlVisor();
            return Task.CompletedTask;
        }

    }
}
