using AutoMapper;
using Business.Interfaces;
using DataTransferObject;
using DataTransferObject.BLs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using StaticClass;
using Website.Models.Parametros;

namespace Website.Pages.Parametros.Especialidades.Especialidades
{
    public partial class AddOrEdit: ComponentBase
    {
        [Parameter] public int? IdEspecialidad { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private DialogService dialogService { get; set; } = null!;

        [Inject] private IEspecilidadesBL _especialidadesBL { get; set; } = null!;
        private EspecialidadModel Model { get; set; } = new();
        private List<GenericComboStrDTO> lstRamas { get; set; } = new List<GenericComboStrDTO>();
        private IMapper _mapper { get; set; } = null!;
        private bool isBusyAceptar { get; set; }
        private bool IsBaja { get; set; }
        private EditContext? context;

        protected override async Task OnInitializedAsync()
        {
            //Crear el mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EspecialidadModel, EspecialidadDTO>().ReverseMap();
            });
            _mapper = config.CreateMapper();
            //--

            if (IdEspecialidad.HasValue)
            {
                var dto = await _especialidadesBL.GetEspecialidadAsync(IdEspecialidad.Value);
                Model = _mapper.Map<EspecialidadModel>(dto);
                IsBaja = Model.Baja;
            }

            var alfabeto = new List<string>()
            {
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
                "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
            };
            
            lstRamas.AddRange(
                alfabeto.Select(x => new GenericComboStrDTO()
                {
                    Id = x,
                    Descripcion = x
                }).ToList());

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
                    var dto = _mapper.Map<EspecialidadModel, EspecialidadDTO>(Model);

                    if (Model.IdEspecialidad.HasValue)
                        await _especialidadesBL.ActualizarEspecialidadAsync(dto);
                    else
                        await _especialidadesBL.AgregarEspecialidadAsync(dto);

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
