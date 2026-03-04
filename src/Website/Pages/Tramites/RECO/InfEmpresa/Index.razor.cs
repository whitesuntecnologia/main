using AutoMapper;
using Business.Extensions;
using Business.Interfaces;
using DataTransferObject;
using DataTransferObject.BLs;
using DocumentFormat.OpenXml.Office.SpreadSheetML.Y2023.MsForms;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using StaticClass;
using Website.Models.Formulario;
using Website.Pages.Shared.Components;
using Website.Pages.Tramites.Formulario.Form2y3;

namespace Website.Pages.Tramites.RECO.InfEmpresa
{
    public partial class Index : ComponentBase
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
        [Inject] private ICombosBL _combosBL { get; set; } = null!;

        private IMapper _mapper { get; set; } = null!;
        private bool EvaluandoTramite { get; set; } = false;
        private TramitesDTO tramite { get; set; } = new();
        private EditContext? context;
        private InfEmpresaConsultoraModel Model { get; set; } = new();
        private List<InfEmpresaConsultoraDocumentoModel> lstTiposDeDocumentos = new List<InfEmpresaConsultoraDocumentoModel>();
        private RadzenDataGrid<InfEmpresaConsultoraDocumentoModel>? grdDocumentos = null;
        private IEnumerable<GenericComboDTO> lstTiposDeSociedad = null!;
        private RadzenDataGrid<InfEmpresaConsultoraPersonaModel>? grdNominaPersonas = null;
        
        private RadzenDataGrid<InfEmpresaConsultoraDeudaModel>? grdDeudas = null;

        private bool PermiteEditar = false;
        
        private bool IsBusyGuardar { get; set; }
        
        private bool IsBusyDownloadDoc { get; set; }
        private CustomFileUpload? uploadCS;
        private List<FileDTO> lstFilesCS = new();
        private bool IsBusyFileUploadCS { get; set; }

        private CustomFileUpload? uploadEs;
        private List<FileDTO> lstFilesEs = new();
        private bool IsBusyFileUploadEs { get; set; }

        private CustomFileUpload? uploadRC;
        private List<FileDTO> lstFilesRC = new();
        private bool IsBusyFileUploadRC { get; set; }
        
        private CustomFileUpload? uploadAD;
        private List<FileDTO> lstFilesAD = new();
        private bool IsBusyFileUploadAD { get; set; }

        private CustomFileUpload? uploadCB;
        private List<FileDTO> lstFilesCB = new();
        private bool IsBusyFileUploadCB { get; set; }


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
                cfg.CreateMap<TiposDeDocumentoDTO, InfEmpresaConsultoraDocumentoModel>();
                cfg.CreateMap<TramitesInfEmpConsPersonaDto, InfEmpresaConsultoraPersonaModel>().ReverseMap();
                cfg.CreateMap<TramitesInfEmpConsDocumentoDto, InfEmpresaConsultoraDocumentoModel>().ReverseMap();
                cfg.CreateMap<InfEmpresaConsultoraDeudaModel, TramitesInfEmpConsDeudaDto>().ReverseMap();

                cfg.CreateMap<TramitesInfEmpConsDto, InfEmpresaConsultoraModel>()
                .ForMember(dest => dest.IdTipoSociedad, opt => opt.Condition(src => src.TipoEmpresa != 1))
                .ForMember(dest => dest.IdFileContrato, opt => opt.Condition(src => src.TipoEmpresa != 1))
                .ForMember(dest => dest.FilenameContrato, opt => opt.Condition(src => src.TipoEmpresa != 1))
                .ForMember(dest => dest.IdFileEstatuto, opt => opt.Condition(src => src.TipoEmpresa != 1))
                .ForMember(dest => dest.FilenameEstatuto, opt => opt.Condition(src => src.TipoEmpresa != 1))
                .ForMember(dest => dest.DeRegComercio, opt => opt.Condition(src => src.TipoEmpresa != 1))
                .ForMember(dest => dest.FechaRegComercio, opt => opt.Condition(src => src.TipoEmpresa != 1))
                .ForMember(dest => dest.LibroRegComercio, opt => opt.Condition(src => src.TipoEmpresa != 1))
                .ForMember(dest => dest.TomoRegComercio, opt => opt.Condition(src => src.TipoEmpresa != 1))
                .ForMember(dest => dest.FolioRegComercio, opt => opt.Condition(src => src.TipoEmpresa != 1))
                .ForMember(dest => dest.IdFileRegComercio, opt => opt.Condition(src => src.TipoEmpresa != 1))
                .ForMember(dest => dest.FilenameRegComercio, opt => opt.Condition(src => src.TipoEmpresa != 1))
                .ForMember(dest => dest.AniosDuracionSoc, opt => opt.Condition(src => src.TipoEmpresa != 1))
                .ForMember(dest => dest.FechaConstitucionSoc, opt => opt.Condition(src => src.TipoEmpresa != 1))
                .ForMember(dest => dest.FechaVencimientoSoc, opt => opt.Condition(src => src.TipoEmpresa != 1))
                .ForMember(dest => dest.ProrrogaDePlazoSoc, opt => opt.Condition(src => src.TipoEmpresa != 1))
                .ForMember(dest => dest.IdFileActaDesignacion, opt => opt.Condition(src => src.TipoEmpresa != 1))
                .ForMember(dest => dest.FilenameActaDesignacion, opt => opt.Condition(src => src.TipoEmpresa != 1))
                .ForMember(dest => dest.Personas, opt => opt.Condition(src => src.TipoEmpresa != 1));

                cfg.CreateMap<InfEmpresaConsultoraModel, TramitesInfEmpConsDto>()
                    .ForMember(dest => dest.IdTipoSociedad, opt => opt.Condition(src => src.TipoEmpresa != 1))
                    .ForMember(dest => dest.IdFileContrato, opt => opt.Condition(src => src.TipoEmpresa != 1))
                    .ForMember(dest => dest.FilenameContrato, opt => opt.Condition(src => src.TipoEmpresa != 1))
                    .ForMember(dest => dest.IdFileEstatuto, opt => opt.Condition(src => src.TipoEmpresa != 1))
                    .ForMember(dest => dest.FilenameEstatuto, opt => opt.Condition(src => src.TipoEmpresa != 1))
                    .ForMember(dest => dest.DeRegComercio, opt => opt.Condition(src => src.TipoEmpresa != 1))
                    .ForMember(dest => dest.FechaRegComercio, opt => opt.Condition(src => src.TipoEmpresa != 1))
                    .ForMember(dest => dest.LibroRegComercio, opt => opt.Condition(src => src.TipoEmpresa != 1))
                    .ForMember(dest => dest.TomoRegComercio, opt => opt.Condition(src => src.TipoEmpresa != 1))
                    .ForMember(dest => dest.FolioRegComercio, opt => opt.Condition(src => src.TipoEmpresa != 1))
                    .ForMember(dest => dest.IdFileRegComercio, opt => opt.Condition(src => src.TipoEmpresa != 1))
                    .ForMember(dest => dest.FilenameRegComercio, opt => opt.Condition(src => src.TipoEmpresa != 1))
                    .ForMember(dest => dest.AniosDuracionSoc, opt => opt.Condition(src => src.TipoEmpresa != 1))
                    .ForMember(dest => dest.FechaConstitucionSoc, opt => opt.Condition(src => src.TipoEmpresa != 1))
                    .ForMember(dest => dest.FechaVencimientoSoc, opt => opt.Condition(src => src.TipoEmpresa != 1))
                    .ForMember(dest => dest.ProrrogaDePlazoSoc, opt => opt.Condition(src => src.TipoEmpresa != 1))
                    .ForMember(dest => dest.IdFileActaDesignacion, opt => opt.Condition(src => src.TipoEmpresa != 1))
                    .ForMember(dest => dest.FilenameActaDesignacion, opt => opt.Condition(src => src.TipoEmpresa != 1))
                    .ForMember(dest => dest.Personas, opt => opt.Condition(src => src.TipoEmpresa != 1));

            });
            _mapper = config.CreateMapper();
            //--

            lstTiposDeSociedad = await _combosBL.GetTiposDeSociedadAsync();
            
            context = new EditContext(Model);
            tramite = await _TramiteBL.GetTramiteByGuidAsync(IdentificadorUnico);
            PermiteEditar = tramite.PermiteEditarEmpresa && tramite.PermiteEditarFormulario(Constants.Formularios.InformacionEmpresa);

            //Cargar Datos de la empresa consultora
            Model = _mapper.Map<TramitesInfEmpConsDto, InfEmpresaConsultoraModel>(await _TramiteBL.GetInfEmpConsAsync(tramite.IdTramite)) ?? new();
            //Model.Deudas = _mapper.Map<List<InfEmpresaConsultoraDeudaModel>>(await _TramiteBL.GetInfEmpConsDeudasAsync(tramite.IdTramite));

            if (Model.IdFileActaDesignacion.HasValue)
                lstFilesAD.Add(await _FilesBL.GetFileAsync(Model.IdFileActaDesignacion.GetValueOrDefault()));

            if (Model.IdFileEstatuto.HasValue)
                lstFilesEs.Add(await _FilesBL.GetFileAsync(Model.IdFileEstatuto.GetValueOrDefault()));

            if (Model.IdFileRegComercio.HasValue)
                lstFilesRC.Add(await _FilesBL.GetFileAsync(Model.IdFileRegComercio.GetValueOrDefault()));

            if (Model.IdFileContrato.HasValue)
                lstFilesCS.Add(await _FilesBL.GetFileAsync(Model.IdFileContrato.GetValueOrDefault()));

            if (Model.IdFileConstanciaBcra.HasValue)
                lstFilesCB.Add(await _FilesBL.GetFileAsync(Model.IdFileConstanciaBcra.GetValueOrDefault()));


            //--

            await ReloadGrillaDocumentosAsync();
            await ReloadGrillaDeudasAsync();

            await base.OnInitializedAsync();
        }
        private async Task ReloadGrillaDocumentosAsync()
        {

            lstTiposDeDocumentos = _mapper.Map<List<TiposDeDocumentoDTO>, List<InfEmpresaConsultoraDocumentoModel>>((await _tablasBL.GetTiposDeDocumentosAsync(Constants.GruposDeTramite.RegistroConsultores)).ToList());
            var lstDocumentosTramite = Model.Documentos;

            foreach (var item in lstDocumentosTramite)
            {
                var doc = lstTiposDeDocumentos.FirstOrDefault(x => x.IdTipoDocumento == item.IdTipoDocumento);
                if (doc != null && !doc.IdFile.HasValue)
                {
                    doc.IdTramiteInfEmpConsDocumento = item.IdTramiteInfEmpConsDocumento;
                    doc.IdFile = item.IdFile;
                    doc.Filename = item.Filename;
                    doc.Size = item.Size;
                }
                else
                {
                    //Cuando ya esta ocupado el tipo de documento crea otro registro con ese tipo de documento
                    //Esto solo sirve para "Otra documentación respaldatoria"
                    var newDoc = new
                        InfEmpresaConsultoraDocumentoModel()
                    {
                        IdTipoDocumento = item.IdTipoDocumento,
                        IdFile = item.IdFile,
                        Descripcion = item.Descripcion,
                        AcronimoGde = item.AcronimoGde,
                        Extension = item.Extension,
                        Filename = item.Filename,
                        IdTramiteInfEmpConsDocumento = item.IdTramiteInfEmpConsDocumento,
                        Obligatorio = item.Obligatorio,
                        Size = item.Size,
                        TamanioMaximoMb = item.TamanioMaximoMb,
                    };
                    lstTiposDeDocumentos.Add(newDoc);
                }
                
            }

            //Si todos los tipos de documentos correspondientes a los que se permiten varias veces ya fueron cargados.
            if (!lstTiposDeDocumentos.Any(x => x.SePermiteVariasVeces && !x.IdFile.HasValue))
            {
                var docs = lstTiposDeDocumentos.Where(x => x.SePermiteVariasVeces).ToList();
                foreach(var doc in docs)
                {
                    lstTiposDeDocumentos.Add(new InfEmpresaConsultoraDocumentoModel()
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
        void CellRender(DataGridCellRenderEventArgs<InfEmpresaConsultoraDocumentoModel> args)
        {

            if (args.Data.IdFile.HasValue)
                args.Attributes.Add("style", $"background-color: rgba(76, 175, 80, 0.16);");
            else if (args.Data.Obligatorio)
                args.Attributes.Add("style", $"background-color: rgba(244, 67, 54, 0.2);");
        }
        protected void VolverAlVisor()
        {
            navigationManager.NavigateTo($"/Tramites/{Accion}/{IdentificadorUnico}", true);
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
        
        protected async Task EliminarDocumentoClick(InfEmpresaConsultoraDocumentoModel row)
        {
            var respuesta = await DS.Confirm("¿Está seguro de eliminar el registro?", "Confirmación",
                new ConfirmOptions() { CancelButtonText = "No", OkButtonText = "Sí" });

            if (respuesta ?? false)
            {
                var docToDelete = Model.Documentos.FirstOrDefault(x => x.IdTipoDocumento == row.IdTipoDocumento);
                if (docToDelete != null)
                {
                    Model.Documentos.Remove(docToDelete);
                    await ReloadGrillaDocumentosAsync();
                }
            }
        }
        protected async Task AgregarDocumentoClick(InfEmpresaConsultoraDocumentoModel row)
        {
            var respuesta = await DS.OpenAsync<DocumentoAdd>("Agregar Documento",
                                        new Dictionary<string, object>() { { "IdentificadorUnico", IdentificadorUnico }, 
                                            { "IdTipoDocumento", row.IdTipoDocumento.GetValueOrDefault() },
                                            { "OnUpdate", new Func<InfEmpresaConsultoraDocumentoModel, Task>(OnUpdateDocumentoEvent) }
                                        },
                                        new DialogOptions() { Width = "60%", Height = "auto", Resizable = true });

            if (respuesta ?? false)
            {
                await ReloadGrillaDocumentosAsync();
            }
        }
        #region Metodos File Contrato
        private void HandleFileDeletionRequestedCS(FileDTO fileModel)
        {
            Model.IdFileContrato = null;
            Model.FilenameContrato = null;
            lstFilesCS = lstFilesCS.Where(x => x != fileModel).ToList();
        }

        private async Task HandleFileUploadedCS(FileDTO fileModel)
        {

            if (uploadCS?.Accept?.Length > 0 && !uploadCS.Accept.Split(",").Contains(fileModel.ContentType))
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido (Archivos Pdf).", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                fileModel.CreateUser = await _usuarioBL.GetCurrentUserid();
                fileModel.CreateDate = DateTime.Now;
                fileModel = await _FilesBL.AddFileAsync(fileModel);
                Model.IdFileContrato = fileModel.IdFile;
                Model.FilenameContrato = fileModel.FileName;
            }
            IsBusyFileUploadCS = false;
            context?.NotifyFieldChanged(context.Field("IdFileContrato"));
        }
        private void HandleErrorCS(string message)
        {
            IsBusyFileUploadCS = false;
            notificationService.Notify(NotificationSeverity.Error, "Error", message, StaticClass.Constants.NotifDuration.Normal);
        }
        protected Task OnProgressCS(UploadProgressArgs args)
        {
            if (args.Progress > 0 && args.Progress != 100)
                IsBusyFileUploadCS = true;

            return Task.CompletedTask;
        }
        #endregion
        #region Metodos File Estatuto
        private void HandleFileDeletionRequestedEs(FileDTO fileModel)
        {
            Model.IdFileEstatuto = null;
            Model.FilenameEstatuto = null;
            lstFilesEs = lstFilesEs.Where(x => x != fileModel).ToList();
        }

        private async Task HandleFileUploadedEs(FileDTO fileModel)
        {

            if (uploadEs?.Accept?.Length > 0 && !uploadEs.Accept.Split(",").Contains(fileModel.ContentType))
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido (Archivos Pdf).", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                fileModel.CreateUser = await _usuarioBL.GetCurrentUserid();
                fileModel.CreateDate = DateTime.Now;
                fileModel = await _FilesBL.AddFileAsync(fileModel);
                Model.IdFileEstatuto = fileModel.IdFile;
                Model.FilenameEstatuto = fileModel.FileName;
            }
            IsBusyFileUploadEs = false;
            context?.NotifyFieldChanged(context.Field("IdFileEstatuto"));
        }
        private void HandleErrorEs(string message)
        {
            IsBusyFileUploadEs = false;
            notificationService.Notify(NotificationSeverity.Error, "Error", message, StaticClass.Constants.NotifDuration.Normal);
        }
        protected Task OnProgressEs(UploadProgressArgs args)
        {
            if (args.Progress > 0 && args.Progress != 100)
                IsBusyFileUploadEs = true;

            return Task.CompletedTask;
        }
        #endregion
        #region Metodos File Registro de Comercio
        private void HandleFileDeletionRequestedRC(FileDTO fileModel)
        {
            Model.IdFileRegComercio = null;
            Model.FilenameRegComercio = null;
            lstFilesRC = lstFilesRC.Where(x => x != fileModel).ToList();
        }

        private async Task HandleFileUploadedRC(FileDTO fileModel)
        {

            if (uploadRC?.Accept?.Length > 0 && !uploadRC.Accept.Split(",").Contains(fileModel.ContentType))
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido (Archivos Pdf).", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                fileModel.CreateUser = await _usuarioBL.GetCurrentUserid();
                fileModel.CreateDate = DateTime.Now;
                fileModel = await _FilesBL.AddFileAsync(fileModel);
                Model.IdFileRegComercio = fileModel.IdFile;
                Model.FilenameRegComercio = fileModel.FileName;
            }
            IsBusyFileUploadRC = false;
            context?.NotifyFieldChanged(context.Field("IdFileRegComercio"));
        }
        private void HandleErrorRC(string message)
        {
            IsBusyFileUploadRC = false;
            notificationService.Notify(NotificationSeverity.Error, "Error", message, StaticClass.Constants.NotifDuration.Normal);
        }
        protected Task OnProgressRC(UploadProgressArgs args)
        {
            if (args.Progress > 0 && args.Progress != 100)
                IsBusyFileUploadRC = true;

            return Task.CompletedTask;
        }
        #endregion
        #region Metodos File Acta Designación
        private void HandleFileDeletionRequestedAD(FileDTO fileModel)
        {
            Model.IdFileActaDesignacion = null;
            Model.FilenameActaDesignacion = null;
            lstFilesAD = lstFilesAD.Where(x => x != fileModel).ToList();
        }

        private async Task HandleFileUploadedAD(FileDTO fileModel)
        {

            if (uploadAD?.Accept?.Length > 0 && !uploadAD.Accept.Split(",").Contains(fileModel.ContentType))
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido (Archivos Pdf).", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                fileModel.CreateUser = await _usuarioBL.GetCurrentUserid();
                fileModel.CreateDate = DateTime.Now;
                fileModel = await _FilesBL.AddFileAsync(fileModel);
                Model.IdFileActaDesignacion = fileModel.IdFile;
                Model.FilenameActaDesignacion = fileModel.FileName;
            }
            IsBusyFileUploadAD = false;
            context?.NotifyFieldChanged(context.Field("IdFileActaDesignacion"));
        }
        private void HandleErrorAD(string message)
        {
            IsBusyFileUploadAD = false;
            notificationService.Notify(NotificationSeverity.Error, "Error", message, StaticClass.Constants.NotifDuration.Normal);
        }
        protected Task OnProgressAD(UploadProgressArgs args)
        {
            if (args.Progress > 0 && args.Progress != 100)
                IsBusyFileUploadAD = true;

            return Task.CompletedTask;
        }
        #endregion

        #region Metodos File Constancia BCRA
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
        protected async Task AgregarNominaClick()
        {
            await DS.OpenAsync<Persona>("Agregar Persona",
                new Dictionary<string, object>() {
                    { "IdentificadorUnico", IdentificadorUnico },
                    { "OnUpdate", new Func<InfEmpresaConsultoraPersonaModel, Task>(OnUpdatePersonaEvent) }
                },
                new DialogOptions() { Width = "50%", Height = "auto", Resizable = true });

        }
        private async Task EliminarNominaClick(InfEmpresaConsultoraPersonaModel row)
        {
            var respuesta = await DS.Confirm("¿Está seguro de eliminar el registro?", "Confirmación",
             new ConfirmOptions() { CancelButtonText = "No", OkButtonText = "Sí" });

            if (respuesta ?? false)
            {
                Model.Personas.Remove(row);
                grdNominaPersonas?.Reload();
            }
        }
        protected Task OnUpdatePersonaEvent(InfEmpresaConsultoraPersonaModel item)
        {
            Model.Personas.Add(item);
            grdNominaPersonas?.Reload();
            return Task.CompletedTask;
        }

        protected async Task OnUpdateDocumentoEvent(InfEmpresaConsultoraDocumentoModel item)
        {
            item.IdTramiteInfEmpCons = Model.IdTramiteInfEmpCon;
            Model.Documentos.Add(item);
            await ReloadGrillaDocumentosAsync();
        }

        private async Task<bool> ValidacionesAlGuardarAsync()
        {
            bool result = false;
            List<string> lstDocumentosFaltantes = new List<string>();

            foreach (var doc in lstTiposDeDocumentos)
            {
                if (doc.Obligatorio && !doc.IdFile.HasValue)
                    lstDocumentosFaltantes.Add(doc.Descripcion);
            }

            result = (lstDocumentosFaltantes.Count == 0);

            if (!result)
                await DS.OpenAsync<ModalValidaciones>("Validaciones",
                                           new Dictionary<string, object>() { { "Mensajes", lstDocumentosFaltantes }, { "Title", "Los siguientes documentos son obligatorios:" } },
                                           new DialogOptions() { Width = "auto", Height = "auto", Resizable = false });


            return result;
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

                        var dto = _mapper.Map<InfEmpresaConsultoraModel, TramitesInfEmpConsDto>(Model);
                        dto.IdTramite = tramite.IdTramite;
                        dto.Cuit = dto.Cuit.Replace("-", "");
                        
                        await _TramiteBL.ActualizarInfEmpConsAsync(dto);

                        notificationService.Notify(NotificationSeverity.Success, "Aviso", "La información se ha almacenado correctamente.", StaticClass.Constants.NotifDuration.Normal);

                        VolverAlVisor();
                    }
                    catch (Exception ex)
                    {
                        IsBusyGuardar = false;
                        notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
                    }
                }
            }
            else
            {
                notificationService.Notify(NotificationSeverity.Error, "Aviso", "Revise las validaciones en pantalla", StaticClass.Constants.NotifDuration.Normal);
            }
            IsBusyGuardar = false;
        }
        protected async Task AgregarDeudaClick()
        {
            var respuesta = await DS.OpenAsync<DeudaAdd>("Agregar Deuda",
                                        new Dictionary<string, object>() { 
                                            { "IdentificadorUnico", IdentificadorUnico } ,
                                            { "OnUpdate", new Func<InfEmpresaConsultoraDeudaModel, Task>(OnUpdateDeudaEvent) }
                                        },
                                        new DialogOptions() { Width = "50%", Height = "auto", Resizable = true });

            if (respuesta ?? false)
            {
                await ReloadGrillaDeudasAsync();
            }
        }
        protected async Task EliminarDeudaClick(InfEmpresaConsultoraDeudaModel row)
        {
            var respuesta = await DS.Confirm("¿Está seguro de eliminar el registro?", "Confirmación",
                new ConfirmOptions() { CancelButtonText = "No", OkButtonText = "Sí" });

            if (respuesta ?? false)
            {
                Model.Deudas.Remove(row);
                //await _TramiteBL.EliminarInfEmpConsDeudaAsync(row.IdTramiteInfEmpConsDeuda);
                await ReloadGrillaDeudasAsync();
            }
        }


        protected async Task OnUpdateDeudaEvent(InfEmpresaConsultoraDeudaModel item)
        {
            try
            {
                var dto = _mapper.Map<InfEmpresaConsultoraDeudaModel, TramitesInfEmpConsDeudaDto>(item);
                dto.IdTramite = tramite.IdTramite;

                if (item.IdTramiteInfEmpDeuda.HasValue)
                {
                    // Actualizar en la colección local
                    var deudaExistente = Model.Deudas.FirstOrDefault(d => d.IdTramiteInfEmpDeuda == item.IdTramiteInfEmpDeuda);
                    if (deudaExistente != null)
                    {
                        var index = Model.Deudas.IndexOf(deudaExistente);
                        Model.Deudas[index] = item;
                    }
                }
                else
                {
                    // Agregar a la colección local con el ID generado
                    Model.Deudas.Add(item);
                }

                await ReloadGrillaDeudasAsync();
            }
            catch (Exception ex)
            {
                notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
            }
        }
        protected async Task EditarDeudaClick(InfEmpresaConsultoraDeudaModel row)
        {
            var respuesta = await DS.OpenAsync<DeudaAdd>("Editar Deuda",
                            new Dictionary<string, object>() { { "IdentificadorUnico", IdentificadorUnico },
                                                               { "Model", row} },
                            new DialogOptions() { Width = "50%", Height = "auto", Resizable = true });

            if (respuesta ?? false)
            {
                await ReloadGrillaDeudasAsync();
            }
        }
        private async Task ReloadGrillaDeudasAsync()
        {
            grdDeudas?.Reload();
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
        private Task OnTipoIibbChanged(int value)
        {
            if(value.Equals(0))
            {
                Model.NroIibb = null;
                Model.FechaInscripcionIibb = null;
            }
            Model.TipoIibb = value;
            return Task.CompletedTask;
        }
    }
}
