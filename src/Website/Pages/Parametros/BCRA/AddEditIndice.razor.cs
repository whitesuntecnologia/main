using AutoMapper;
using Business.Interfaces;
using DataTransferObject;
using DataTransferObject.BLs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using StaticClass;
using Website.Models.Parametros;

namespace Website.Pages.Parametros.BCRA
{
    public partial class AddEditIndice: ComponentBase
    {
        [Parameter] public int? IdIndiceBcra { get; set; } = null!;
        [Inject] private DialogService DS { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private DialogService dialogService { get; set; } = null!;

        [Inject] private IIndiceBancoCentralBL _indicesBL { get; set; } = null!;
        [Inject] private ICombosBL _combosBL { get; set; } = null!;
        private IndiceBancoCentralModel Model { get; set; } = new();
        private IEnumerable<GenericComboDTO> lstSituaciones { get; set; } = null!;
        private IMapper _mapper { get; set; } = null!;
        private bool isBusyAceptar { get; set; }

        protected override async Task OnInitializedAsync()
        {
            //Crear el mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IndiceBancoCentralModel, IndiceBancoCentralDTO>().ReverseMap();
            });
            _mapper = config.CreateMapper();
            //--

            if (IdIndiceBcra.HasValue)
            {
                var dto = await _indicesBL.GetIndiceAsync(IdIndiceBcra.Value);
                Model = _mapper.Map<IndiceBancoCentralModel>(dto);
            }
            lstSituaciones = await _combosBL.GetSituacionesBcraAsync();
        
            await base.OnInitializedAsync();
        }

        protected async Task OnClickAceptar(EditContext Ed)
        {
            if (isBusyAceptar)
                return;

            //context = Ed;
            isBusyAceptar = true;

            if (Ed != null && Ed.Validate())
            {

                try
                {
                    var dto = _mapper.Map<IndiceBancoCentralModel, IndiceBancoCentralDTO>(Model);

                    if (Model.IdIndiceBcra.HasValue)
                        await _indicesBL.ActualizarIndiceAsync(dto);
                    else
                        await _indicesBL.AgregarIndiceAsync(dto);

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
