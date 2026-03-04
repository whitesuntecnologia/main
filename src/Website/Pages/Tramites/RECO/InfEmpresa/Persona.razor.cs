using Business.Interfaces;
using DataTransferObject;
using DataTransferObject.BLs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using StaticClass;
using Website.Models.Formulario;

namespace Website.Pages.Tramites.RECO.InfEmpresa
{
    public partial class Persona: ComponentBase
    {
        [Parameter] public string IdentificadorUnico { get; set; } = null!;
        [Parameter] public Func<InfEmpresaConsultoraPersonaModel, Task>? OnUpdate { get; set; }
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private DialogService dialogService { get; set; } = null!;
        [Inject] private ICombosBL _CombosBL { get; set; } = null!;
        [Inject] private ITramitesBL _TramiteBL { get; set; } = null!;
        [Inject] private IUsuariosBL _usuarioBL { get; set; } = null!;

        private TramitesDTO tramite { get; set; } = null!;
        private InfEmpresaConsultoraPersonaModel Model { get; set; } = new();
        private IEnumerable<GenericComboDTO> lstTiposDeCaracterLegal { get; set; } = null!;
        private bool isBusyAceptar { get; set; }
        protected override async Task OnInitializedAsync()
        {
            tramite = await _TramiteBL.GetTramiteByGuidAsync(IdentificadorUnico);
            lstTiposDeCaracterLegal = await _CombosBL.GetTiposDeCaracterLegalAsync();

            await base.OnInitializedAsync();
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
                        Model.DescripcionTipoCaracterLegal = lstTiposDeCaracterLegal.FirstOrDefault(x => x.Id == Model.IdTipoCaracterLegal)?.Descripcion;
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
