using AutoMapper;
using Business;
using Business.Interfaces;
using DataAccess.Entities;
using DataTransferObject;
using DataTransferObject.BLs;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using StaticClass;
using Website.Models.Formulario;
using Website.Models.Parametros;


namespace Website.Pages.Parametros.UVI
{
    public partial class AddEditIndice: ComponentBase
    {
        [Parameter] public int? IdUvi { get; set; } = null!;
        [Inject] private DialogService DS { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private DialogService dialogService { get; set; } = null!;

        [Inject] private IIndiceUnidadViviendaBL _indicesBL { get; set; } = null!;
        private IndiceUnidadViviendaModel Model { get; set; } = new();
        private List<GenericComboDTO> lstMeses { get; set; } = new();
        private IMapper _mapper { get; set; } = null!;
        private bool isBusyAceptar { get; set; }
        private EditContext? context;

        protected override async Task OnInitializedAsync()
        {
            //Crear el mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IndiceUnidadViviendaModel, IndiceUnidadViviendaDTO>().ReverseMap();
            });
            _mapper = config.CreateMapper();
            //--

            if (IdUvi.HasValue)
            {
                var dto = await _indicesBL.GetIndiceAsync(IdUvi.Value);
                Model = _mapper.Map<IndiceUnidadViviendaModel>(dto);
            }

            //Carga de los meses
            for (int i = 1; i <= 12; i++)
            {
                lstMeses.Add(new GenericComboDTO()
                {
                    Id = i,
                    Descripcion = i.ToString(),
                });
            }
            //--
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
                    var dto = _mapper.Map<IndiceUnidadViviendaModel, IndiceUnidadViviendaDTO>(Model);

                    if (Model.IdUvi.HasValue)
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
