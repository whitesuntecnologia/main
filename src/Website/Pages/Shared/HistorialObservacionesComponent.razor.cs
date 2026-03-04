using Business.Interfaces;
using DataAccess.Entities;
using DataTransferObject;
using DataTransferObject.BLs;
using Microsoft.AspNetCore.Components;
using StaticClass;

namespace Website.Pages.Shared
{
    public partial class HistorialObservacionesComponent: ComponentBase
    {
        [Parameter] public string IdentificadorUnico { get; set; } = null!;

        [Inject] private ITramitesBL _TramiteBL { get; set; } = null!;

        private TramitesDTO tramite { get; set; } = new();
        private List<TramiteFormularioEvaluadoDTO> lstObservaciones { get; set; } = new();


        protected override async Task OnInitializedAsync()
        {
            tramite = await _TramiteBL.GetTramiteByGuidAsync(IdentificadorUnico);
            lstObservaciones = (await _TramiteBL.GetFormulariosEvaluadosAsync(tramite.IdTramite))
                                .Where(x => x.IdEstadoEvaluacion == (int)Constants.EstadosEvaluacion.Notificar)
                                .OrderByDescending(o => o.NroNotificacion)
                                .ThenBy(o => o.NroFormulario)
                                .ToList();
                                

        }
    }
}
