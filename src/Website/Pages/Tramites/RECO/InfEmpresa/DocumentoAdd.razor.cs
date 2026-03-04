using Business;
using Business.Interfaces;
using DataTransferObject;
using DataTransferObject.BLs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using StaticClass;
using Website.Models.Formulario;
using Website.Pages.Shared.Components;

namespace Website.Pages.Tramites.RECO.InfEmpresa
{
    public partial class DocumentoAdd: ComponentBase
    {
        [Parameter] public string IdentificadorUnico { get; set; } = null!;
        [Parameter] public int IdTipoDocumento { get; set; }
        [Parameter] public Func<InfEmpresaConsultoraDocumentoModel, Task>? OnUpdate { get; set; }


        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private DialogService dialogService { get; set; } = null!;
        [Inject] private ICombosBL _CombosBL { get; set; } = null!;
        [Inject] private ITramitesBL _TramiteBL { get; set; } = null!;
        [Inject] private IUsuariosBL _usuarioBL { get; set; } = null!;
        [Inject] private IFilesBL _filesBL { get; set; } = null!;
        [Inject] private ITablasBL _tablasBL { get; set; } = null!;

        private IEnumerable<GenericComboDTO> lstTiposDeDocumentos = new List<GenericComboDTO>();
        private InfEmpresaConsultoraDocumentoModel Model { get; set; } = new();
        private CustomFileUpload? upload;
        private List<FileDTO> lstFiles = new();
        private bool isBusyFileUpload { get;set; }
        private bool isBusyAceptar { get; set; }

        protected override async Task OnInitializedAsync()
        {
            lstTiposDeDocumentos = await _CombosBL.GetTiposDeDocumentosAsync();
            Model.IdTipoDocumento = IdTipoDocumento;
            await base.OnInitializedAsync();
        }

        private void HandleFileDeletionRequested(FileDTO fileModel)
        {
            Model.IdFile = null;
            Model.Filename = null;
            lstFiles = lstFiles.Where(x => x != fileModel).ToList();
        }

        private async Task HandleFileUploaded(FileDTO fileModel)
        {

            if (upload?.Accept?.Length > 0 && upload.Accept != fileModel.ContentType)
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido (Archivos Pdf).", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                fileModel.CreateUser = await _usuarioBL.GetCurrentUserid();
                fileModel.CreateDate = DateTime.Now;
                fileModel = await _filesBL.AddFileAsync(fileModel);
                Model.IdFile = fileModel.IdFile;
                Model.Filename = fileModel.FileName;
                Model.Size = fileModel.Size;
            }
            isBusyFileUpload = false;
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

        protected async Task OnClickAceptar(EditContext ed)
        {
            if (isBusyAceptar)
                return;

            isBusyAceptar = true;
            if (ed.Validate())
            {
                //Alta 
                try
                {
                    if (OnUpdate != null)
                    {
                        var lstTiposDeDocumentos = (await _tablasBL.GetTiposDeDocumentosAsync(Constants.GruposDeTramite.RegistroConsultores)).ToList();
                        var docConfig = lstTiposDeDocumentos.First(x => x.IdTipoDocumento == Model.IdTipoDocumento);
                        Model.Extension = docConfig.Extension;
                        Model.Descripcion = docConfig.Descripcion;
                        Model.TamanioMaximoMb = docConfig.TamanioMaximoMb;
                        Model.AcronimoGde = docConfig.AcronimoGde;
                        Model.Obligatorio = docConfig.Obligatorio;
                        Model.SePermiteVariasVeces = docConfig.SePermiteVariasVeces;
                                
                        await OnUpdate.Invoke(Model);
                    }

                    dialogService.Close(true);
                }
                catch (Exception ex)
                {
                    notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
                }
            }
            
            isBusyAceptar = false;
        }

      
    }
}
