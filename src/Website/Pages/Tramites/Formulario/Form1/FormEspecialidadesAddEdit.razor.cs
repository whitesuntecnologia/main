using Business;
using Business.Interfaces;
using DataTransferObject;
using DataTransferObject.BLs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using Radzen.Blazor;
using StaticClass;
using Website.Models.Formulario;

namespace Website.Pages.Tramites.Formulario.Form1
{
    public partial class FormEspecialidadesAddEdit: ComponentBase
    {
        [Parameter] public string IdentificadorUnico { get; set; } = null!;
        [Parameter] public int? IdTramiteEspecialidad { get; set; } = null!;
        [Parameter] public int? IdSeccion { get; set; } = null!;

        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private DialogService dialogService { get; set; } = null!;
        [Inject] private ICombosBL _CombosBL { get; set; } = null!;
        [Inject] private ITramitesBL _TramiteBL { get; set; } = null!;
        [Inject] private IEspecilidadesBL _especialidadesBL { get; set; } = null!;


        private TramitesDTO tramite { get; set; } = new();
        private IEnumerable<GenericComboDTO> lstEspecialidades = new List<GenericComboDTO>();
        private IEnumerable<GenericComboDTO> lstSecciones = new List<GenericComboDTO>();
        private IEnumerable<GenericComboDTO> lstTareas = new List<GenericComboDTO>();

        private FormEspecialidadesAddModel Model {get;set;  } = new();

        protected override async Task OnInitializedAsync()
        {
            tramite = await _TramiteBL.GetTramiteByGuidAsync(IdentificadorUnico);
            lstEspecialidades = await _CombosBL.GetEspecialidadesAsync();
            Model.IdGrupoTramite = tramite.IdGrupoTramite;

            if (IdTramiteEspecialidad.HasValue)
            {
                var EspecilidadGuardad = await _TramiteBL.GetEspecialidadAsync(IdTramiteEspecialidad.Value);
                Model.IdEspecialidadSelected = EspecilidadGuardad.IdEspecialidad;
                await OnChangeEspecialidades(EspecilidadGuardad.IdEspecialidad);
                Model.IdSeccionSelected = EspecilidadGuardad.Secciones.FirstOrDefault( x=> x.IdSeccion == IdSeccion)?.IdSeccion ?? 0;
                await OnChangeSecciones(Model.IdSeccionSelected);
                
                List<int> tareasSelected = new();
                
                if(tramite.IdGrupoTramite == Constants.GruposDeTramite.RegistroLicitadores)
                {
                    foreach (var seccion in EspecilidadGuardad.Secciones.Where(x => x.IdSeccion == IdSeccion))
                    {
                        tareasSelected.AddRange(seccion.Tareas.Select(s => s.IdTarea).ToList());
                    }
                    Model.IdTareasSeleccted = tareasSelected.Distinct();
                }


            }
            await base.OnInitializedAsync();
        }

        protected async Task OnChangeEspecialidades(object value)
        {
            if (value != null)
            {
                int IdEspecialidad = Convert.ToInt32(value);
                Model.IdSeccionSelected = 0;
                //Limpiar Tareas
                Model.IdTareasSeleccted = new List<int>();
                lstTareas = new List<GenericComboDTO>();
                //
                var especialidadDTO = await _especialidadesBL.GetEspecialidadAsync(IdEspecialidad);
                if(especialidadDTO.Rama == "C")
                    lstSecciones = await _CombosBL.GetEspecialidadesSeccionesFromRamaAyBAsync(tramite.IdTramite, IdEspecialidad);
                else
                    lstSecciones = await _CombosBL.GetEspecialidadesSeccionesAsync(IdEspecialidad);
            }
            else
                lstSecciones = new List<GenericComboDTO>();
        }
        protected async Task OnChangeSecciones(object value)
        {
            if (value != null && tramite.IdGrupoTramite == Constants.GruposDeTramite.RegistroLicitadores)
            {
                int IdSeccionChanged = Convert.ToInt32(value);
                Model.IdTareasSeleccted = new List<int>();

                var especialidadDTO = await _especialidadesBL.GetEspecialidadAsync(Model.IdEspecialidadSelected);
                if (especialidadDTO.Rama == "C")
                    lstTareas = await _CombosBL.GetTareasBySeccionFromRamaAyBAsync(tramite.IdTramite, IdSeccionChanged);
                else
                    lstTareas = await _CombosBL.GetTareasBySeccionAsync(IdSeccionChanged);
            }
            else
                lstTareas = new List<GenericComboDTO>();
        }
        protected async Task OnClickAceptar( EditContext ed)
        {
            if (ed.Validate())
            {
                if (Model.IdGrupoTramite != Constants.GruposDeTramite.RegistroConsultores && !Model.IdTareasSeleccted.Any())
                {
                    notificationService.Notify(NotificationSeverity.Error, "Aviso", "Debe ingresar al menos una tarea.", StaticClass.Constants.NotifDuration.Normal);
                    return;
                }
                //Alta de la Especialidad
                try
                {
                    if(IdTramiteEspecialidad.HasValue)
                        await _TramiteBL.ActualizarEspecialidadAsync(tramite.IdTramite, 
                                                                     Model.IdEspecialidadSelected, 
                                                                     Model.IdSeccionSelected,
                                                                     Model.IdTareasSeleccted.ToList());
                    else
                        await _TramiteBL.AgregarEspecialidadAsync(tramite.IdTramite, 
                                                                  Model.IdEspecialidadSelected, 
                                                                  Model.IdSeccionSelected,
                                                                  Model.IdTareasSeleccted.ToList());

                    dialogService.Close(true);
                }
                catch (Exception ex)
                {
                    notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
                }
            }
        }
    }
}
