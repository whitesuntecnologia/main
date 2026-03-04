using AutoMapper;
using Business.Interfaces;
using DataTransferObject;
using DataTransferObject.BLs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using StaticClass;
using Website.Models.Parametros;

namespace Website.Pages.Parametros.Especialidades.Tareas
{
    public partial class AddOrEdit: ComponentBase
    {
        [Parameter] public int? IdTarea { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private DialogService dialogService { get; set; } = null!;

        [Inject] private IEspecilidadesBL _especialidadesBL { get; set; } = null!;
        [Inject] private ICombosBL _combosBL { get; set; } = null!;
        private EspecialidadTareaModel Model { get; set; } = new();
        
        private IEnumerable<GenericComboDTO> lstSecciones { get; set; } = null!;
        private IMapper _mapper { get; set; } = null!;
        private bool isBusyAceptar { get; set; }
        private bool IsBaja { get; set; }
        private EditContext? context;

        protected override async Task OnInitializedAsync()
        {
            //Crear el mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EspecialidadTareaModel, EspecialidadTareaDTO>().ReverseMap();
            });
            _mapper = config.CreateMapper();
            //--

            if (IdTarea.HasValue)
            {
                var dto = await _especialidadesBL.GetEspecialidadTareaAsync(IdTarea.Value);
                Model = _mapper.Map<EspecialidadTareaModel>(dto);
                IsBaja = Model.Baja;
            }

            
            lstSecciones   = await _combosBL.GetEspecialidadesSeccionesAsync();

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
                    var dto = _mapper.Map<EspecialidadTareaModel, EspecialidadTareaDTO>(Model);

                    if (Model.IdTarea.HasValue)
                        await _especialidadesBL.ActualizarEspecialidadTareaAsync(dto);
                    else
                        await _especialidadesBL.AgregarEspecialidadTareaAsync(dto);

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

