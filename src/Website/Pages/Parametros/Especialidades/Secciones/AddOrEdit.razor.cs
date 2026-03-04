using AutoMapper;
using Business;
using Business.Interfaces;
using DataTransferObject;
using DataTransferObject.BLs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using StaticClass;
using Website.Models.Parametros;

namespace Website.Pages.Parametros.Especialidades.Secciones
{
    public partial class AddOrEdit: ComponentBase
    {
        [Parameter] public int? IdSeccion { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private DialogService dialogService { get; set; } = null!;

        [Inject] private IEspecilidadesBL _especialidadesBL { get; set; } = null!;
        [Inject] private ICombosBL _combosBL { get; set; } = null!;
        private EspecialidadSeccionModel Model { get; set; } = new();
        private IEnumerable<GenericComboDTO> lstEspecialidades { get; set; } = null!;
        private IMapper _mapper { get; set; } = null!;
        private bool isBusyAceptar { get; set; }
        private bool IsBaja { get; set; }
        private EditContext? context;

        protected override async Task OnInitializedAsync()
        {
            //Crear el mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EspecialidadSeccionModel, EspecialidadSeccionDTO>().ReverseMap();
            });
            _mapper = config.CreateMapper();
            //--

            if (IdSeccion.HasValue)
            {
                var dto = await _especialidadesBL.GetEspecialidadSeccionAsync(IdSeccion.Value);
                Model = _mapper.Map<EspecialidadSeccionModel>(dto);
                IsBaja = Model.Baja;
            }
            lstEspecialidades = await _combosBL.GetEspecialidadesAsync();
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
                    var dto = _mapper.Map<EspecialidadSeccionModel, EspecialidadSeccionDTO>(Model);

                    if (Model.IdSeccion.HasValue)
                        await _especialidadesBL.ActualizarSeccionAsync(dto);
                    else
                        await _especialidadesBL.AgregarSeccionAsync(dto);

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
