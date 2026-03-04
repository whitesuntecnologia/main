using Business.Interfaces;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using StaticClass;
using System.ComponentModel;
using Website.Models.Formulario;
using Website.Models.Seguridad;
using Website.Pages.Shared.Components;

namespace Website.Pages.Tramites.Formulario.Form9
{
    public partial class RepresentanteTecnicoDesvincular: ComponentBase
    {
        [Parameter] public RepresentanteTecnicoDesvinculacionModel Model { get; set; } = new();

        [Inject] private DialogService dialogService { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private IFilesBL _filesBL { get; set; } = null!;
        [Inject] private IUsuariosBL _usuarioBL { get; set; } = null!;

        private bool isBusyAceptar { get; set; }

        private CustomFileUpload? upload;
        private List<FileDTO> lstFiles = new();

        protected override async Task OnInitializedAsync()
        {

            if (Model.IdFileDesvinculacion.HasValue)
                lstFiles.Add(await _filesBL.GetFileAsync(Model.IdFileDesvinculacion.GetValueOrDefault()));

            await base.OnInitializedAsync();
        }


        private async Task OnClickAceptar(EditContext Ed)
        {
            if (isBusyAceptar)
                return;

            isBusyAceptar = true;

            if (Ed != null && Ed.Validate())
            {
                try
                {
                    dialogService.Close(Model);
                }
                catch (Exception ex)
                {
                    notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), Constants.NotifDuration.Normal);
                }
            }

            isBusyAceptar = false;
        }

        private void OnClickCancelar()
        {
            dialogService.Close(null);
        }

        #region File Sancion
        private void HandleFileDeletionRequested(FileDTO fileModel)
        {
            Model.IdFileDesvinculacion = null;
            Model.FilenameDesvinculacion = null;
            lstFiles = lstFiles.Where(x => x != fileModel).ToList();
        }

        private async Task HandleFileUploaded(FileDTO fileModel)
        {

            if (upload?.Accept?.Length > 0 && upload.Accept != fileModel.ContentType)
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                fileModel.CreateUser = await _usuarioBL.GetCurrentUserid();
                fileModel.CreateDate = DateTime.Now;
                fileModel = await _filesBL.AddFileAsync(fileModel);
                Model.IdFileDesvinculacion = fileModel.IdFile;
                Model.FilenameDesvinculacion = fileModel.FileName;
            }
        }
        private void HandleError(string message)
        {
            notificationService.Notify(NotificationSeverity.Error, "Error", message, StaticClass.Constants.NotifDuration.Normal);
        }
        #endregion
    }
}
