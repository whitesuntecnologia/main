using AutoMapper;
using Business.Interfaces;
using DataAccess.Entities;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using StaticClass;
using Website.Models.Parametros;

namespace Website.Pages.Parametros.Especialidades.Equipos
{
    public partial class AddOrEdit: ComponentBase
    {
        [Parameter] public int? IdEquipo { get; set; } = null!;
        [Inject] private DialogService DS { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private DialogService dialogService { get; set; } = null!;

        [Inject] private IEspecilidadesBL _especialidadesBL { get; set; } = null!;
        private EspecialidadEquipoModel Model { get; set; } = new();
        private IMapper _mapper { get; set; } = null!;
        private bool isBusyAceptar { get; set; }
        private bool IsBaja { get; set; }
        private EditContext? context;

        protected override async Task OnInitializedAsync()
        {
            //Crear el mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EspecialidadEquipoModel, EspecialidadEquipoDTO>().ReverseMap();
            });
            _mapper = config.CreateMapper();
            //--

            if (IdEquipo.HasValue)
            {
                var dto = await _especialidadesBL.GetEspecialidadEquipoAsync(IdEquipo.Value);
                Model = _mapper.Map<EspecialidadEquipoModel>(dto);
                IsBaja = Model.Baja;
            }
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
                    var dto = _mapper.Map<EspecialidadEquipoModel, EspecialidadEquipoDTO>(Model);

                    if (Model.IdEquipo.HasValue)
                        await _especialidadesBL.ActualizarEquipoAsync(dto);
                    else
                        await _especialidadesBL.AgregarEquipoAsync(dto);

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
