using AutoMapper;
using Business;
using Business.Interfaces;
using DataTransferObject;
using DataTransferObject.BLs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.VisualBasic;
using Website.Models.Formulario;
using Website.Models.Shared;

namespace Website.Pages.Shared
{
    public partial class FormEvaluacion : ComponentBase
    {
        [Parameter] public string IdentificadorUnico { get; set; } = null!;
        [Parameter] public int? NroFormulario { get; set; } = null!;
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public EventCallback<TramiteFormularioEvaluadoDTO> OnGuardarClick { get; set; }
        [Parameter] public EventCallback OnCancelarClick { get; set; }

        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private ICombosBL _CombosBL { get; set; } = null!;
        [Inject] private ITramitesBL _TramiteBL { get; set; } = null!;
        [Inject] private ITramitesEvaluacionBL _TramiteEvaluacionBL { get; set; } = null!;
        
        private IMapper _mapper { get; set; } = null!;
        private FormEvaluacionModel Model { get; set; } = new();
        private TramitesDTO tramite { get; set; } = new();
        private IEnumerable<GenericComboDTO> lstEstados = null!;
        private EditContext context { get; set; } = null!;  
        private bool isBusyGuardando { get; set; }

        protected override async Task OnInitializedAsync()
        {
            //Crear el mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FormEvaluacionModel, TramiteFormularioEvaluadoDTO>().ReverseMap();
            });
            _mapper = config.CreateMapper();
            //--
            
            tramite = await _TramiteBL.GetTramiteByGuidAsync(IdentificadorUnico);
            lstEstados = await _CombosBL.GetEstadosEvaluacionAsync();

            // Recuperar los datos de evaluacion y/o inicializar el modelo
            int NroNotificacion = await _TramiteEvaluacionBL.GetNumeroNotificacionActualAsync(tramite.IdTramite);
            var dto = await _TramiteEvaluacionBL.GetEvaluacionAsync(tramite.IdTramite, NroNotificacion, NroFormulario.GetValueOrDefault());
            Model = _mapper.Map<FormEvaluacionModel>(dto);

            if (Model == null)
            {
                
                Model = new FormEvaluacionModel()
                {
                    IdTramite = tramite.IdTramite,
                    NroFormulario = NroFormulario,
                    NroNotificacion = NroNotificacion
                };
            }
            //--
            context = new EditContext(Model);
            await base.OnInitializedAsync();
        }
        private Task ReValidate(string fieldName)
        {
            if (context != null)
            {
                FieldIdentifier field = context.Field(fieldName);
                context?.NotifyFieldChanged(field);
            }
            return Task.CompletedTask;
        }
        protected async Task OnClickGuardar( EditContext Ed)
        {
            if (isBusyGuardando)
                return;
            
            isBusyGuardando = true;
            context = Ed;
            if (Ed != null && Ed.Validate())
            {
                if (OnGuardarClick.HasDelegate)
                {
                    
                    var args = _mapper.Map<FormEvaluacionModel, TramiteFormularioEvaluadoDTO>(Model);
                    await OnGuardarClick.InvokeAsync(args);
                }

            }
            isBusyGuardando = false;
            
        }
        protected async Task CancelarClick()
        {
            if (OnCancelarClick.HasDelegate)
            {
                if(context.Validate())
                    await OnCancelarClick.InvokeAsync();

            }
            
        }
    }
}
