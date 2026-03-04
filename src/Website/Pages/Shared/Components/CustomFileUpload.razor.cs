using Business.Interfaces;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Website.Pages.Shared.Components
{
    public partial class CustomFileUpload: ComponentBase
    {
        [Parameter] public string? Accept { get; set; }
        [Parameter] public List<FileDTO> Files { get; set; } = new();
        [Parameter] public EventCallback<FileDTO> OnFileUploaded { get; set; }
        [Parameter] public EventCallback<FileDTO> OnFileDeleted { get; set; }
        [Parameter] public EventCallback<UploadProgressArgs> OnProgress { get; set; }
        [Parameter] public EventCallback<string> OnError { get; set; }
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool DisableAddFile { get; set; } = false;

        [Inject] IParametrosBL ParametrosBL { get; set; } = null!;
        [Inject] IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] IFilesBL filesBL { get; set; } = null!;

        private double progressUploadingFile;
        private bool showGrid = true;
        private RadzenDataGrid<FileDTO>? grid;
        private bool isBusyFileDownload { get; set; }

        private FileDTO _fileModel = new();
        private string pathParam = "";

        protected override async Task OnInitializedAsync()
        {
            pathParam = await ParametrosBL.GetParametroCharAsync("Path.Upload.Files");
            await base.OnInitializedAsync();
        }
        protected override async Task OnParametersSetAsync()
        {
            //if(Files.Count > 0) { 
                await ReloadGridAsync();
            //}
            await base.OnParametersSetAsync();
        }
        protected async Task OnProgressAsync(UploadProgressArgs args)
        {
            progressUploadingFile = args.Progress;

            if (args.Progress == 100)
            {

                var file = args.Files.FirstOrDefault();
                if (file != null)
                {

                    string filePath = Path.Combine(pathParam, file.Name);
                    string contentType = GetMimemMapping(file.Name);

                    string[] accepts = Accept?.Split(",") ?? new string[0];
                    if (accepts != null && accepts.Length > 0 && !accepts.Contains(contentType))
                    {
                        await OnError.InvokeAsync("El archivo seleccionado no posee el formato permitido");
                    }
                    else
                    {

                        _fileModel.FilePath = filePath;
                        _fileModel.FileName = file.Name;
                        _fileModel.Size = file.Size;
                        _fileModel.Extension = Path.GetExtension(file.Name);
                        _fileModel.ContentType = contentType;
                        _fileModel.Rowid = Guid.NewGuid();
                    }
                }
            }
            else
            {
                showGrid = false;
            }
            await OnProgress.InvokeAsync(args);
        }
        private byte[] ReadAllBytes2(string filePath)
        {

            using (var fs = new System.IO.FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }

        private string GetMimemMapping(string FileName)
        {
            string? contentType;
            new FileExtensionContentTypeProvider().TryGetContentType(FileName, out contentType);
            return contentType ?? "application/octet-stream";
        }
        public async Task RemoveFileAsync(FileDTO fileModel)
        {
            await OnFileDeleted.InvokeAsync(fileModel);
        }

        private async Task OnFileDeletionRequestedInternalAsync(FileDTO fileModel)
        {
            await OnFileDeleted.InvokeAsync(fileModel);
            await ReloadGridAsync();
        }

        private async Task ReloadGridAsync()
        {
            if (grid != null)
                await grid.Reload();
        }

        protected async Task OnComplete(UploadCompleteEventArgs args)
        {

            if (File.Exists(_fileModel.FilePath))
            {
                byte[] arrFile = ReadAllBytes2(_fileModel.FilePath);
                _fileModel.ContentFile = arrFile;
                _fileModel.Md5 = filesBL.CreateMD5(arrFile);
                Files.Clear();
                Files.Add(_fileModel);
                await OnFileUploaded.InvokeAsync(_fileModel);
            }

            await ReloadGridAsync();
            ResetProgressBar();
            showGrid = true;
            StateHasChanged();

        }

        protected async Task OnErrorUpload(UploadErrorEventArgs args)
        {
            await OnError.InvokeAsync(args.Message);
        }

        private void ResetProgressBar()
        {
            progressUploadingFile = 0;
        }

        private async Task OnDownloadLocalFileAsync(string filePath)
        {
            isBusyFileDownload = true;
            byte[] file = await File.ReadAllBytesAsync(filePath);
            string[] ArrSergments = filePath.Split(@"\");
            string fileName = ArrSergments[ArrSergments.Length - 1];
            string contentType = "application/octet-stream";

            await JSRuntime.InvokeVoidAsync("BlazorDownloadFile", fileName, contentType, file);
            isBusyFileDownload = false;
        }

        private async Task OnDownloadFileAsync(int fileId)
        {
            isBusyFileDownload = true;
            var archivoDTO = await filesBL.GetFileAsync(fileId);
            byte[] file = archivoDTO.ContentFile;
            string fileName = archivoDTO.FileName;
            string contentType = archivoDTO.ContentType;

            await JSRuntime.InvokeVoidAsync("BlazorDownloadFile", fileName, contentType, file);
            isBusyFileDownload = false;
        }
        public void Refresh()
        {
            StateHasChanged();
        }
    }
}
