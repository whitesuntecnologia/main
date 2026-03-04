using Business.Interfaces;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Website.Pages
{
    public partial class Utils: ComponentBase
    {
        [Inject] private ITramitesBL _tramitesBL { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        private int? IdTramite { get; set; }
        private bool isBusyCopiar { get; set; } = false;

        protected async Task CopiarClick()
        {
            if (!IdTramite.HasValue)
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "Debe ingresar un Id de trámite para copiar.", StaticClass.Constants.NotifDuration.Normal);
                return;
            }
            isBusyCopiar = true;
            try
            {
                var tramiteDto = await _tramitesBL.GetTramiteByIdAsync(IdTramite.Value);
                await _tramitesBL.AprobarTramiteAsync(tramiteDto);

                notificationService.Notify(NotificationSeverity.Success, "Éxito", $"Trámite con ID {IdTramite.Value} copiado a la estructura de Empresas.", StaticClass.Constants.NotifDuration.Normal);

            }
            catch (Exception ex)
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", ex.Message, StaticClass.Constants.NotifDuration.Normal);
            }
            finally
            {
                isBusyCopiar = false;
            }
            

        }
    }
}
