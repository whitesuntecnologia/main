using Business;
using Business.Interfaces;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen.Blazor;

namespace Website.Pages.MisCertificados
{
    public partial class Index: ComponentBase
    {
        [Inject] private ITramitesBL _TramiteBL { get; set; } = null!;
        [Inject] private IUsuariosBL _usuariosBL { get; set; } = null!;
        [Inject] IFilesBL filesBL { get; set; } = null!;
        [Inject] IJSRuntime JSRuntime { get; set; } = null!;


        private List<ItemGrillaMisCertificadosDTO> lstCertificados = new List<ItemGrillaMisCertificadosDTO>();
        private RadzenDataGrid<ItemGrillaMisCertificadosDTO>? grdCertificados = null;
        private bool isBusyFileDownload;

        protected override async Task OnInitializedAsync()
        {

            int IdEmpresa = 0;
            int.TryParse(await _usuariosBL.GetCurrentIdEmpresa(), out IdEmpresa);
            lstCertificados = await _TramiteBL.GetCertificadosGrillaByEmpresaAsync(IdEmpresa);
            lstCertificados = lstCertificados.OrderByDescending(o => o.IdTramite).ToList();
            grdCertificados?.Reload();
            await base.OnInitializedAsync();
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
    }
}
