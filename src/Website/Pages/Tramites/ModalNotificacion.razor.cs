using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Build.Framework;
using Radzen;
using StaticClass;
using Website.Models.Shared;

namespace Website.Pages.Tramites
{
    public partial class ModalNotificacion: ComponentBase
    {
        [Parameter] public NotificacionModel Model { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private DialogService dialogService { get; set; } = null!;
        
        private bool isBusyAceptar { get; set; } = false;

        protected async Task OnClickAceptar(EditContext ed)
        {
            if (isBusyAceptar)
                return;

            if (ed.Validate())
            {
                isBusyAceptar = true;
                try
                {
                    dialogService.Close(Model);
                }
                catch (Exception ex)
                {
                    notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
                }
                finally
                {
                    isBusyAceptar = false;
                }
            }
        }

    }
}
