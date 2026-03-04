using AutoMapper;
using Business.Interfaces;
using DataTransferObject.BLs;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen.Blazor;
using Radzen;
using Website.Models.Formulario;
using Website.Pages.Tramites.Formulario.Form10;
using Business;
using StaticClass;
using Business.Extensions;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Website.Pages.Tramites.Formulario.Form11
{
    public partial class AntecedentesIndex : ComponentBase
    {
        [Parameter] public string Accion { get; set; } = null!;
        [Parameter] public string IdentificadorUnico { get; set; } = null!;
        [Inject] private DialogService DS { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] protected RouteHistoryService routeHistory { get; set; } = null!;

        [Inject] private ITramitesBL _TramiteBL { get; set; } = null!;
        [Inject] private IAntecedentesBl _AntecedentesBL { get; set; } = null!;
        [Inject] private ITramitesEvaluacionBL _TramiteEvaluacionBL { get; set; } = null!;
        [Inject] private IFilesBL _FilesBL { get; set; } = null!;
        private IMapper _mapper { get; set; } = null!;
        private bool EvaluandoTramite { get; set; } = false;
        private TramitesDTO tramite { get; set; } = new();
        private List<TramitesAntecedentesDto> lstAntecedentesObras { get; set; } = new();
        private List<ItemGrillaEspecialidadesDTO> lstEspecialidades { get; set; } = new();
        
        private RadzenDataGrid<TramitesAntecedentesDto>? grdAntecedentes = null;
        
        private bool PermiteEditar = false;

        private bool _SinDatosForm = false;
        private bool SinDatosForm
        {
            get { return _SinDatosForm; }
            set
            {
                _SinDatosForm = value;
                tramite.SinDatosForm11 = value;
                _TramiteBL.ActualizarTramiteSinDatosFormAsync(tramite.IdTramite, 11, value);
            }
        }
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
            PermiteEditar = tramite.PermiteEditarEmpresa && tramite.PermiteEditarFormulario(Constants.Formularios.AntecedentesDeProduccion);

            //Crear el mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AntecedentesModel, TramitesAntecedentesDto>().ReverseMap();
            });
            _mapper = config.CreateMapper();
            //--

            SinDatosForm = tramite.SinDatosForm11;
            await ReloadGrillaAsync();
            await base.OnInitializedAsync();
        }

        private async Task ReloadGrillaAsync()
        {
            lstEspecialidades = await _TramiteBL.GetEspecialidadesAsync(tramite.IdTramite);
            lstAntecedentesObras = await _AntecedentesBL.GetAntecedentesAsync(tramite.IdTramite);
            grdAntecedentes?.Reload();
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

        private Task AgregarObraClick(int IdtramiteEspecialidad, int IdTramiteEspecialidadSeccion)
        {
            navigationManager.NavigateTo($"/Tramites/AntecedentesObras/{Accion}/{IdentificadorUnico}/Agregar/{IdtramiteEspecialidad}/{IdTramiteEspecialidadSeccion}", true);
            return Task.CompletedTask;

        }
        private Task EditarObraClick(TramitesAntecedentesDto dto)
        {
            navigationManager.NavigateTo($"/Tramites/AntecedentesObras/{Accion}/{IdentificadorUnico}/Editar/{dto.IdTramiteAntecedente}");
            return Task.CompletedTask;
        }
        private async Task EliminarObraClick(TramitesAntecedentesDto dto)
        {
            var respuesta = await DS.Confirm("¿Está seguro de eliminar el registro?", "Confirmación",
               new ConfirmOptions() { CancelButtonText = "No", OkButtonText = "Sí" });

            if (respuesta ?? false)
            {
                await _AntecedentesBL.EliminarAntecedenteAsync(dto.IdTramiteAntecedente);
                await ReloadGrillaAsync();
            }
        }
        private Task DetalleMensualClick(int IdTramiteEspecialidad, int IdTramiteEspecialidadSeccion)
        {
            string AccionActual = tramite.PermiteEditarEmpresa ? "Editar" : "Visualizar";
            navigationManager.NavigateTo($"/Tramites/AntecedentesDetalleMensual/{Accion}/{IdentificadorUnico}/{AccionActual}/{IdTramiteEspecialidad}/{IdTramiteEspecialidadSeccion}", true);
            return Task.CompletedTask;

        }
    }
}
