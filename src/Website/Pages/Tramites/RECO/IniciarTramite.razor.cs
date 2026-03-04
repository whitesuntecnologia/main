using Business.Interfaces;
using DataTransferObject.BLs;
using Microsoft.AspNetCore.Components;
using Radzen;
using StaticClass;

namespace Website.Pages.Tramites.RECO
{
    public partial class IniciarTramite : ComponentBase
    {
        [Inject] private DialogService DS { get; set; } = null!;
        [Inject] private NavigationManager NM { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private ITramitesBL _tramitesBL { get; set; } = null!;
        [Inject] private IUsuariosBL _usuarioBL { get; set; } = null!;
        private TramitesDTO TramitePendiente { get; set; } = null!;
        private TramitesDTO TramiteAprobadoNoVencido { get; set; } = null!;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (!(await TieneTramitePendiente()))
                await GenerarTramite();
        }
        public async Task GenerarTramite()
        {
            var respuesta = await DS.Confirm("¿Está seguro de iniciar un nuevo trámite?",
                                 "Quiero Inscribirme",
                                 new ConfirmOptions() { OkButtonText = "Sí", CancelButtonText = "No" }
                                 );


            if (respuesta.HasValue && respuesta.Value) //Si
            {
                try
                {
                    var tramite = await _tramitesBL.CrearTramiteAsync(Constants.TiposDeTramite.Reco_Inscripcion);
                    NM.NavigateTo($"/Tramites/Visualizar/{tramite.IdentificadorUnico}");
                }
                catch (Exception ex)
                {
                    notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
                }

            }
            else
            {
                NM.NavigateTo($"/1");
            }
        }
        private async Task<bool> TieneTramitePendiente()
        {

            int userIdEmpresa = 0;
            int.TryParse(await _usuarioBL.GetCurrentIdEmpresa(), out userIdEmpresa);
            TramitePendiente = await _tramitesBL.GetTramitePendienteAsync(Constants.GruposDeTramite.RegistroConsultores, userIdEmpresa);

            return TramitePendiente != null;
        }

    }
}
