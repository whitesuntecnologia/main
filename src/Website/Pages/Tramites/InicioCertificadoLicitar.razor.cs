using Business.Interfaces;
using DataTransferObject.BLs;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using StaticClass;
using Website.Models.Formulario;
using Website.Pages.Shared;
using Website.Pages.Shared.Components;

namespace Website.Pages.Tramites
{
    public partial class InicioCertificadoLicitar: ComponentBase
    {
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private DialogService dialogService { get; set; } = null!;
        [Inject] private ICombosBL _CombosBL { get; set; } = null!;
        [Inject] private ITramitesBL _TramiteBL { get; set; } = null!;

        private CertificadoLicitarModel Model { get; set; } = new();
        private IEnumerable<GenericComboDTO> lstObras { get; set; } = null!;
        private bool isBusyAceptar;
        private EditContext? context;

        protected override async Task OnInitializedAsync()
        {
            context = new EditContext(Model);
            lstObras = await _CombosBL.GetObrasPciaLPParaLicitarAsync();
            await base.OnInitializedAsync();

        }
        protected async Task OnClickAceptar(EditContext Ed)
        {
            if (isBusyAceptar)
                return;

            context = Ed;
            isBusyAceptar = true;
            if (Ed != null && Ed.Validate())
            {

                try
                {
                    var tramite = await _TramiteBL.CrearTramiteCertParaLicitarAsync(Model.IdObraPciaLP.GetValueOrDefault());
                    navigationManager.NavigateTo($"/Tramites/Visualizar/{tramite.IdentificadorUnico}", true);
                }
                catch (Exception ex)
                {
                    List<string> mensajes = new List<string>() { Functions.GetErrorMessage(ex) };
                    await dialogService.OpenAsync<ModalValidaciones>("Validaciones",
                                    new Dictionary<string, object>() { { "Mensajes", mensajes }, { "Title", "" } },
                                    new DialogOptions() { Width = "70%", Height = "auto", Resizable = false });

                    isBusyAceptar = false;
                }
            }
            else
                isBusyAceptar = false;
        }
        protected void CancelarClick()
        {
            navigationManager.NavigateTo($"/", true);
        }
    }
}
