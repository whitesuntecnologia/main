using AutoMapper;
using Business;
using Business.Extensions;
using Business.Interfaces;
using DataTransferObject;
using DataTransferObject.BLs;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2019.Drawing.Model3D;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using Radzen.Blazor;
using StaticClass;
using System.Linq;
using Website.Models.Formulario;
using Website.Pages.Tramites.Formulario.Form1;

namespace Website.Pages.Tramites.RECO.Especialidades
{
    public partial class Index: ComponentBase
    {
        [Parameter] public string Accion { get; set; } = null!;
        [Parameter] public string IdentificadorUnico { get; set; } = null!;
        [Inject] private DialogService DS { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private ITramitesBL _TramiteBL { get; set; } = null!;
        [Inject] private ITramitesEvaluacionBL _TramiteEvaluacionBL { get; set; } = null!;
        [Inject] private IEspecilidadesBL _especialidadesBL { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] protected RouteHistoryService routeHistory { get; set; } = null!;
        private bool EvaluandoTramite { get; set; } = false;
        private TramitesDTO tramite { get; set; } = new();
        private List<EspecialidadSeccionDTO> lstEspecialidadesSecciones = new List<EspecialidadSeccionDTO>();
        private IList<EspecialidadSeccionDTO> lstEspecialidadesSeccionesSelecteds { get; set; } = new List<EspecialidadSeccionDTO>();
        private RadzenDataGrid<EspecialidadSeccionDTO>? grdEspecialidadesSecciones = null;
        private bool PermiteEditar = false;
        private bool IsBusyGuardar = false;
        private IMapper _mapper { get; set; } = null!;
        

        protected override async Task OnInitializedAsync()
        {
         
            EvaluandoTramite = await _TramiteBL.isEvaluandoTramite(IdentificadorUnico);

            // Comprueba la seguridad del trámite
            if (!await _TramiteBL.ComprobarSeguridad(IdentificadorUnico))
            {
                navigationManager.NavigateTo("/ValidationError/401", true);
                return;
            }
            //--

            tramite = await _TramiteBL.GetTramiteByGuidAsync(IdentificadorUnico);
            PermiteEditar = tramite.PermiteEditarEmpresa && tramite.PermiteEditarFormulario(Constants.Formularios.Especialidades);

            //Obtiene las Especialidades desde la base de datos
            lstEspecialidadesSecciones = await _especialidadesBL.GetSeccionesObrasComplementariasAsync();
            var lstEspecialidadesDb = await _TramiteBL.GetEspecialidadesAsync(tramite.IdTramite);
            var lstSeccionesDb = lstEspecialidadesDb.SelectMany(s => s.Secciones).Select(s => s.IdSeccion).ToList();
            lstEspecialidadesSeccionesSelecteds = lstEspecialidadesSecciones.Where(x => lstSeccionesDb.Contains(x.IdSeccion)).ToList();
            //

            grdEspecialidadesSecciones?.Reload();
        
            await base.OnInitializedAsync();

        }
                
        protected void VolverAlVisor()
        {
            navigationManager.NavigateTo($"/Tramites/Visualizar/{IdentificadorUnico}", true);
        }

        protected async Task GuardarEvaluacionClick(TramiteFormularioEvaluadoDTO dto)
        {
            try
            {
                await _TramiteEvaluacionBL.GuardarEvaluacionAsync(dto);
                VolverAlVisor();
            }
            catch (Exception ex)
            {
                notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.VerySlow);
            }
        }
        protected Task CancelarEvaluacionClick()
        {
            VolverAlVisor();
            return Task.CompletedTask;

        }
        protected Task CheckboxClick(EspecialidadSeccionDTO value)
        {
            lstEspecialidadesSeccionesSelecteds?.Add(value);
            return Task.CompletedTask;
        }

        protected async Task OnClickAceptar()
        {
            if(!lstEspecialidadesSeccionesSelecteds.Any())
            {
                notificationService.Notify(NotificationSeverity.Info, "Aviso", "Debe seleccionar las especialidades.", StaticClass.Constants.NotifDuration.Normal);
                return;
            }
            if (IsBusyGuardar)
                return;

            IsBusyGuardar = true;

            try
            {
                await _TramiteBL.ActualizarEspecialidadesRecoAsync(tramite.IdTramite, lstEspecialidadesSeccionesSelecteds.ToList());
                navigationManager.NavigateTo($"/Tramites/Visualizar/{IdentificadorUnico}", true);
            }
            catch (Exception ex)
            {
                notificationService.Notify(NotificationSeverity.Error, "Error",Functions.GetErrorMessage(ex) , StaticClass.Constants.NotifDuration.VerySlow);
            }
            
        }
    }
}

