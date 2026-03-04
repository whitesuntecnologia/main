using AutoMapper;
using Business;
using Business.Extensions;
using Business.Interfaces;
using DataTransferObject;
using DataTransferObject.BLs;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using StaticClass;
using Website.Models.Formulario;
using Website.Pages.Shared.Components;

namespace Website.Pages.Tramites.Formulario.Form2y3
{
    public partial class InfEmpresa : ComponentBase
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
        [Inject] private IUsuariosBL _usuarioBL { get; set; } = null!;
        [Inject] private ITablasBL _tablasBL { get; set; } = null!;
        
        private IMapper _mapper { get; set; } = null!;
        private bool EvaluandoTramite { get; set; } = false;
        private TramitesDTO tramite { get; set; } = new();
        private List<InfEmpresaDocumentoModel> lstTiposDeDocumentos = new List<InfEmpresaDocumentoModel>();
        private RadzenDataGrid<InfEmpresaDocumentoModel>? grdDocumentos = null;
        private List<TramitesInfEmpDeudaDTO> lstDeudas = new List<TramitesInfEmpDeudaDTO>();
        private RadzenDataGrid<TramitesInfEmpDeudaDTO>? grdDeudas = null;

        private InfEmpresaModel Model { get; set; } = new();
        private bool IsBusyDownloadDoc { get; set; }
        private bool IsBusyGuardar { get; set; }
        
        private CustomFileUpload? uploadCC;
        private List<FileDTO> lstFilesCC = new();
        private bool IsBusyFileUploadCC { get; set; }

        private CustomFileUpload? uploadCB;
        private List<FileDTO> lstFilesCB = new();
        private bool IsBusyFileUploadCB { get; set; }

        private EditContext? context;
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

            //Crear el mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TiposDeDocumentoDTO,InfEmpresaDocumentoModel>();
                cfg.CreateMap<TramitesInfEmpDTO, InfEmpresaModel>().ReverseMap();
            });
            _mapper = config.CreateMapper();
            //--
            context = new EditContext(Model);
            tramite = await _TramiteBL.GetTramiteByGuidAsync(IdentificadorUnico);
            PermiteEditar = tramite.PermiteEditarEmpresa && tramite.PermiteEditarFormulario(Constants.Formularios.InformacionEmpresa);

            Model = _mapper.Map< TramitesInfEmpDTO,InfEmpresaModel>( await _TramiteBL.GetInfEmpAsync(tramite.IdTramite)) ?? new();
            if(Model.IdFileConstanciaInscImpNacionales.HasValue)
                lstFilesCC.Add(await _FilesBL.GetFileAsync(Model.IdFileConstanciaInscImpNacionales.GetValueOrDefault()));

            if (Model.IdFileConstanciaBcra.HasValue)
                lstFilesCB.Add(await _FilesBL.GetFileAsync(Model.IdFileConstanciaBcra.GetValueOrDefault()));

            await ReloadGrillaDocumentosAsync();
            await ReloadGrillaDeudasAsync();
            await base.OnInitializedAsync();
        }
        
        protected async Task AgregarDocumentoClick(InfEmpresaDocumentoModel row)
        {
            var respuesta = await DS.OpenAsync<InfEmpresaDocumentoAdd>("Agregar Documento",
                                        new Dictionary<string, object>() { { "IdentificadorUnico", IdentificadorUnico }, { "IdTipoDocumento", row.IdTipoDocumento} },
                                        new DialogOptions() { Width = "60%", Height = "auto", Resizable = true });

            if (respuesta ?? false)
            {
                await ReloadGrillaDocumentosAsync();
            }
        }

        protected async Task EliminarDocumentoClick(InfEmpresaDocumentoModel row)
        {
            var respuesta = await DS.Confirm("¿Está seguro de eliminar el registro?", "Confirmación",
                new ConfirmOptions() { CancelButtonText = "No", OkButtonText = "Sí" });

            if (respuesta ?? false)
            {
                if (row.IdTramiteInfEmpDocumento.HasValue)
                {
                    await _TramiteBL.EliminarInfEmpDocumentoAsync(row.IdTramiteInfEmpDocumento.Value);
                    await ReloadGrillaDocumentosAsync();
                }
            }
        }
        private async Task ReloadGrillaDeudasAsync()
        {
            lstDeudas = await _TramiteBL.GetInfEmpDeudasAsync(tramite.IdTramite);
            grdDeudas?.Reload();
        }
        private async Task OnDownloadFileAsync(int fileId)
        {
            IsBusyDownloadDoc = true;
            
            var archivoDTO = await _FilesBL.GetFileAsync(fileId);
            byte[] file = archivoDTO.ContentFile;
            string fileName = archivoDTO.FileName;
            string contentType = archivoDTO.ContentType;

            await JSRuntime.InvokeVoidAsync("BlazorDownloadFile", fileName, contentType, file);
            IsBusyDownloadDoc = false;
        }
        protected async Task AgregarDeudaClick()
        {
            var respuesta = await DS.OpenAsync<InfEmpresaDeudaAdd>("Agregar Deuda",
                                        new Dictionary<string, object>() { { "IdentificadorUnico", IdentificadorUnico } },
                                        new DialogOptions() { Width = "50%", Height = "auto", Resizable = true });

            if (respuesta ?? false)
            {
                await ReloadGrillaDeudasAsync();
            }
        }

        protected async Task EliminarDeudaClick(TramitesInfEmpDeudaDTO row)
        {
            var respuesta = await DS.Confirm("¿Está seguro de eliminar el registro?", "Confirmación",
                new ConfirmOptions() { CancelButtonText = "No", OkButtonText = "Sí" });

            if (respuesta ?? false)
            {
                await _TramiteBL.EliminarInfEmpDeudaAsync(row.IdTramiteInfEmpDeuda);
                await ReloadGrillaDeudasAsync();
            }
        }
        protected async Task EditarDeudaClick(TramitesInfEmpDeudaDTO row)
        {
            var respuesta = await DS.OpenAsync<InfEmpresaDeudaAdd>("Editar Deuda",
                            new Dictionary<string, object>() { { "IdentificadorUnico", IdentificadorUnico },
                                                               { "IdTramiteInfEmpDeuda", row.IdTramiteInfEmpDeuda} },
                            new DialogOptions() { Width = "50%", Height = "auto", Resizable = true });

            if (respuesta ?? false)
            {
                await ReloadGrillaDeudasAsync();
            }
        }
        protected async Task OnClickGuardar(EditContext ed)
        {
            if (IsBusyGuardar)
                return;

            IsBusyGuardar = true;
            context = ed;
            if (ed.Validate())
            {
                if ((await ValidacionesAlGuardarAsync()))
                {
                    //Alta de información de la empresa
                    try
                    {

                        var dto = _mapper.Map<InfEmpresaModel, TramitesInfEmpDTO>(Model);
                        dto.IdTramite = tramite.IdTramite;
                      
                        await _TramiteBL.ActualizarInfEmpAsync(dto);

                        notificationService.Notify(NotificationSeverity.Success, "Aviso", "La información se ha almacenado correctamente.", StaticClass.Constants.NotifDuration.Normal);

                        VolverAlVisor();
                    }
                    catch (Exception ex)
                    {
                        notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
                    }
                }
                else
                {

                }
            }
            IsBusyGuardar = false;
        }

        private async Task<bool> ValidacionesAlGuardarAsync()
        {
            bool result = false;
            List<string> lstDocumentosFaltantes = new List<string>(); 

            foreach(var doc in lstTiposDeDocumentos)
            {
                if(doc.Obligatorio && !doc.IdFile.HasValue)
                    lstDocumentosFaltantes.Add(doc.Descripcion);
            }

            result = (lstDocumentosFaltantes.Count == 0);

            if(!result)
                await DS.OpenAsync<ModalValidaciones>("Validaciones",
                                           new Dictionary<string, object>() { { "Mensajes", lstDocumentosFaltantes } , {"Title", "Los siguientes documentos son obligatorios:" } },
                                           new DialogOptions() { Width = "auto", Height = "auto", Resizable = false });


            return result;
        }
        private async Task ReloadGrillaDocumentosAsync()
        {

            lstTiposDeDocumentos = _mapper.Map<List<TiposDeDocumentoDTO>,List<InfEmpresaDocumentoModel>>((await _tablasBL.GetTiposDeDocumentosAsync(Constants.GruposDeTramite.RegistroLicitadores)).ToList());
            var lstDocumentosTramite = await _TramiteBL.GetInfEmpDocumentosAsync(tramite.IdTramite);

            foreach(var item in lstDocumentosTramite)
            {
                var doc = lstTiposDeDocumentos.FirstOrDefault(x => x.IdTipoDocumento == item.IdTipoDocumento);
                if (doc != null && !doc.IdFile.HasValue )
                {
                    doc.IdTramiteInfEmpDocumento = item.IdTramiteInfEmpDocumento;
                    doc.IdFile = item.IdFile;
                    doc.Filename = item.Filename;
                    doc.Size = item.Size;
                }
                else
                {
                    //Cuando ya esta ocupado el tipo de documento crea otro registro con ese tipo de documento
                    //Esto solo sirve para "Otra documentación respaldatoria"
                    var newDoc = new InfEmpresaDocumentoModel()
                    {
                        IdTipoDocumento = item.IdTipoDocumento,
                        IdFile = item.IdFile,
                        Descripcion = item.DescripcionTipoDocumento,
                        AcronimoGde = doc.AcronimoGde,
                        Extension = doc.Extension,
                        Filename = item.Filename,
                        IdTramiteInfEmpDocumento = item.IdTramiteInfEmpDocumento,
                        Obligatorio = doc.Obligatorio,
                        Size = item.Size,
                        TamanioMaximoMb = doc.TamanioMaximoMb,
                    };
                    lstTiposDeDocumentos.Add(newDoc);

                }
            }

            //Si todos los tipos de documentos correspondientes a los que se permiten varias veces ya fueron cargados.
            if (!lstTiposDeDocumentos.Any(x => x.SePermiteVariasVeces && !x.IdFile.HasValue))
            {
                var docs = lstTiposDeDocumentos.Where(x => x.SePermiteVariasVeces).ToList();
                foreach (var doc in docs)
                {
                    lstTiposDeDocumentos.Add(new InfEmpresaDocumentoModel()
                    {
                        IdTipoDocumento = doc.IdTipoDocumento,
                        Descripcion = doc.Descripcion,
                        Obligatorio = doc.Obligatorio,
                        AcronimoGde = doc.AcronimoGde,
                        Extension = doc.Extension,
                        TamanioMaximoMb = doc.TamanioMaximoMb,
                    });
                }
            }

            lstTiposDeDocumentos = lstTiposDeDocumentos.OrderByDescending(o => o.Obligatorio).ToList();
            grdDocumentos?.Reload();
        }
        void CellRender(DataGridCellRenderEventArgs<InfEmpresaDocumentoModel> args)
        {
            
            if (args.Data.IdFile.HasValue)
                args.Attributes.Add("style", $"background-color: rgba(76, 175, 80, 0.16);");
            else if (args.Data.Obligatorio)
                args.Attributes.Add("style", $"background-color: rgba(244, 67, 54, 0.2);");
        }

        #region Metodos File Constancia de Inscripcion Impuestos Nacionales
        private void HandleFileDeletionRequestedCC(FileDTO fileModel)
        {
            Model.IdFileConstanciaInscImpNacionales = null;
            Model.FilenameConstanciaInscImpNacionales = null;
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
                fileModel = await _FilesBL.AddFileAsync(fileModel);
                Model.IdFileConstanciaInscImpNacionales = fileModel.IdFile;
                Model.FilenameConstanciaInscImpNacionales = fileModel.FileName;
            }
            IsBusyFileUploadCC = false;
            context?.NotifyFieldChanged(context.Field("IdFileContratoObra"));
        }
        private void HandleErrorCC(string message)
        {
            IsBusyFileUploadCC = false;
            notificationService.Notify(NotificationSeverity.Error, "Error", message, StaticClass.Constants.NotifDuration.Normal);
        }
        protected Task OnProgressCC(UploadProgressArgs args)
        {
            if (args.Progress > 0 && args.Progress != 100)
                IsBusyFileUploadCC = true;

            return Task.CompletedTask;
        }
        #endregion

        #region Metodos File Constancia Bcra
        private void HandleFileDeletionRequestedCB(FileDTO fileModel)
        {
            Model.IdFileConstanciaBcra = null;
            Model.FilenameConstanciaBcra = null;
            lstFilesCB = lstFilesCB.Where(x => x != fileModel).ToList();
        }

        private async Task HandleFileUploadedCB(FileDTO fileModel)
        {

            if (uploadCB?.Accept?.Length > 0 && !uploadCB.Accept.Split(",").Contains(fileModel.ContentType))
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido (Archivos Pdf).", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                fileModel.CreateUser = await _usuarioBL.GetCurrentUserid();
                fileModel.CreateDate = DateTime.Now;
                fileModel = await _FilesBL.AddFileAsync(fileModel);
                Model.IdFileConstanciaBcra = fileModel.IdFile;
                Model.FilenameConstanciaBcra = fileModel.FileName;
            }
            IsBusyFileUploadCB = false;
            context?.NotifyFieldChanged(context.Field("IdFileConstanciaBcra"));
        }
        private void HandleErrorCB(string message)
        {
            IsBusyFileUploadCB = false;
            notificationService.Notify(NotificationSeverity.Error, "Error", message, StaticClass.Constants.NotifDuration.Normal);
        }
        protected Task OnProgressCB(UploadProgressArgs args)
        {
            if (args.Progress > 0 && args.Progress != 100)
                IsBusyFileUploadCB = true;

            return Task.CompletedTask;
        }
        #endregion
        protected void VolverAlVisor()
        {
            
            navigationManager.NavigateTo($"/Tramites/{Accion}/{IdentificadorUnico}", true);
            
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
