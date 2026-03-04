using AutoMapper;
using Business;
using Business.Interfaces;
using DataAccess.Entities;
using DataTransferObject;
using DataTransferObject.BLs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using StaticClass;
using Website.Models.Formulario;

namespace Website.Pages.Tramites.Formulario
{
    public partial class ObrasAddModal: ComponentBase
    {
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private DialogService dialogService { get; set; } = null!;
        [Inject] private ICombosBL _combosBL { get; set; } = null!;
        [Inject] private IUsuariosBL _usuarioBL { get; set; } = null!;
        [Inject] private IObrasProvinciaLaPampaBL _obrasLP { get; set; } = null!;

        
        private ObrasAddModel Model { get; set; } = new();
        private IMapper _mapper { get; set; } = null!;
        private IEnumerable<GenericComboStrDTO> lstEstadosObra = new List<GenericComboStrDTO>();
        private bool CuitEmpresaDisabled { get; set; } = true;

        private bool isBusyAceptar { get; set; }

        protected override async Task OnInitializedAsync()
        {
            //Crear el mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ObrasAddModel, ObrasProvinciaLaPampaDTO>().ReverseMap();
            });
            _mapper = config.CreateMapper();
            //--

            lstEstadosObra = await _combosBL.GetEstadosObraAsync();
            Model.CuitEmpresa = Convert.ToDecimal(await _usuarioBL.GetCurrentUserNameEmpresa());
            Model.Empresa = await _usuarioBL.GetCurrentNombreEmpresa();
            
            await base.OnInitializedAsync();
        }

        protected async Task OnClickAceptar(EditContext ed)
        {
            if (isBusyAceptar)
                return;

            if (ed.Validate())
            {
                isBusyAceptar = true;
                try
                {
                    var dto = _mapper.Map<ObrasProvinciaLaPampaDTO>(Model);
                    dto.EsAltaPorUsuario = true;
                    dto = await _obrasLP.AgregarObraAsync(dto);
                    dialogService.Close(dto.IdObraPciaLp);

                }
                catch (Exception ex)
                {
                    notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
                }
                finally
                {
                    isBusyAceptar = false;
                }
            }
        }

        protected async Task chkEsUTEValueChanged(bool value)
        {
            
            CuitEmpresaDisabled = !value;
            if (CuitEmpresaDisabled)
            {
                Model.CuitEmpresa = Convert.ToDecimal(await _usuarioBL.GetCurrentUserNameEmpresa());
                Model.Empresa = await _usuarioBL.GetCurrentNombreEmpresa();
                Model.PorcentajeParticipacion = null;
            }
        }

    }
}
