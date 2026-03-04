using Business.Interfaces;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using StaticClass;
using System;
using Website.Models.Formulario;
using Website.Pages.Shared.Components;

namespace Website.Pages.Tramites.Formulario.Form6
{
    public partial class BalanceGeneralDatos: ComponentBase
    {
        [Parameter] public bool ReadOnly { get; set; } = false;
        [Parameter] public BalanceGeneralModel Model { get; set; } = new();
        [Parameter] public EventCallback OnEditClick { get; set; }
        [Parameter] public bool ButtonEditVisible { get; set; } = true;

        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private IUsuariosBL _usuarioBL { get; set; } = null!;
        [Inject] private IFilesBL _filesBL { get; set; } = null!;
        
        private bool isBusyAceptar { get; set; }
        private CustomFileUpload? upload;
        private List<FileDTO> lstFiles = new();
        private bool isBusyFileUpload { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if(Model.IdFile.HasValue)
                lstFiles.Add(await _filesBL.GetFileAsync(Model.IdFile.Value));
            
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
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                fileModel.CreateUser = await _usuarioBL.GetCurrentUserid();
                fileModel.CreateDate = DateTime.Now;
                fileModel = await _filesBL.AddFileAsync(fileModel);
                Model.IdFile = fileModel.IdFile;
                Model.Filename = fileModel.FileName;
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
        protected void EditarBalanceClick()
        {
            if(OnEditClick.HasDelegate)
                OnEditClick.InvokeAsync();
        }
    }
}
