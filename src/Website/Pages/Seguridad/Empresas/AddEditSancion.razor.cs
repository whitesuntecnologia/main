using Business;
using Business.Interfaces;
using DataTransferObject;
using Humanizer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using StaticClass;
using Website.Models.Seguridad;
using Website.Pages.Shared.Components;

namespace Website.Pages.Seguridad.Empresas
{
    public partial class AddEditSancion : ComponentBase
    {
        [Parameter] public EmpresaSancionModel Model { get; set; } = new();

        [Inject] private DialogService dialogService { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private IFilesBL _filesBL { get; set; } = null!;
        [Inject] private IUsuariosBL _usuarioBL { get; set; } = null!;

        private bool isBusyAceptar { get; set; }
        
        private CustomFileUpload? upload;
        private List<FileDTO> lstFilesSanciones = new();

        protected override async Task OnInitializedAsync()
        {
            if (Model == null)
            {
                Model = new EmpresaSancionModel
                {
                    FechaDesdeSancion = DateTime.Today
                };
            }

            if (Model.IdFileSancion.HasValue)
                lstFilesSanciones.Add(await _filesBL.GetFileAsync(Model.IdFileSancion.GetValueOrDefault()));

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
                    if (Model.IdFileSancion.GetValueOrDefault() == 0)
                    {
                        notificationService.Notify(NotificationSeverity.Warning, "Aviso", "Debe cargar un archivo", Constants.NotifDuration.Normal);
                        return;
                    }

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
            Model.IdFileSancion = null;
            Model.FilenameSancion = null;
            lstFilesSanciones = lstFilesSanciones.Where(x => x != fileModel).ToList();
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
                Model.IdFileSancion = fileModel.IdFile;
                Model.FilenameSancion= fileModel.FileName;
            }
        }
        private void HandleError(string message)
        {
            notificationService.Notify(NotificationSeverity.Error, "Error", message, StaticClass.Constants.NotifDuration.Normal);
        }
        #endregion
    }
}