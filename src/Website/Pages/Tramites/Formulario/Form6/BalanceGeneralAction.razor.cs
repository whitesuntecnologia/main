using AutoMapper;
using Business;
using Business.Interfaces;
using DataTransferObject;
using DataTransferObject.BLs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Radzen;
using StaticClass;
using Website.Models.Formulario;

namespace Website.Pages.Tramites.Formulario.Form6
{
    public partial class BalanceGeneralAction : ComponentBase
    {
        [Parameter] public string IdentificadorUnico { get; set; } = null!;
        [Parameter] public string AccionFormAnterior { get; set; } = null!; //Ingresar o Editar
        [Parameter] public string Accion { get; set; } = null!; //Ingresar o Editar
        [Parameter] public int? IdTramiteBalanceGeneral { get; set; } = null!; //Ingresar o Editar
        [Inject] private DialogService DS { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private ITramitesBL _TramiteBL { get; set; } = null!;
        private IMapper _mapper { get; set; } = null!;
        private bool EvaluandoTramite { get; set; } = false;
        private TramitesDTO tramite { get; set; } = new();
        private BalanceGeneralModel Model { get; set; } = new();
        private bool IsBusyGuardar { get;set; }

        protected override async Task OnInitializedAsync()
        {
            EvaluandoTramite = navigationManager.Uri.Contains("Evaluar");

            //Crear el mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TramitesBalanceGeneralDTO,BalanceGeneralModel>();
                cfg.CreateMap<BalanceGeneralModel, TramitesBalanceGeneralDTO>()
                    .ForMember(dest => dest.OoficDepositosCortoPlazo, opt => opt.MapFrom(src => src.OoficDepositosCortoPlazoEmp))
                    .ForMember(dest => dest.OoficDepositosLargoPlazo, opt => opt.MapFrom(src => src.OoficDepositosLargoPlazoEmp))
                    .ForMember(dest => dest.OoficInversionesCortoPlazo, opt => opt.MapFrom(src => src.OoficInversionesCortoPlazoEmp))
                    .ForMember(dest => dest.OoficInversionesLargoPlazo, opt => opt.MapFrom(src => src.OoficInversionesLargoPlazoEmp))
                    .ForMember(dest => dest.OoficOtrosConceptos, opt => opt.MapFrom(src => src.OoficOtrosConceptosEmp))
                    .ForMember(dest => dest.OpartDepositosCortoPlazo, opt => opt.MapFrom(src => src.OpartDepositosCortoPlazoEmp))
                    .ForMember(dest => dest.OpartDepositosLargoPlazo, opt => opt.MapFrom(src => src.OpartDepositosLargoPlazoEmp))
                    .ForMember(dest => dest.OpartInversionesCortoPlazo, opt => opt.MapFrom(src => src.OpartInversionesCortoPlazoEmp))
                    .ForMember(dest => dest.OpartInversionesLargoPlazo, opt => opt.MapFrom(src => src.OpartInversionesLargoPlazoEmp))
                    .ForMember(dest => dest.OpartInversiones, opt => opt.MapFrom(src => src.OpartInversionesEmp))
                    .ForMember(dest => dest.OpartOtrosConceptos, opt => opt.MapFrom(src => src.OpartOtrosConceptosEmp))
                    .ForMember(dest => dest.DispCajayBancos, opt => opt.MapFrom(src => src.DispCajayBancosEmp))
                    .ForMember(dest => dest.BusoInversiones, opt => opt.MapFrom(src => src.BusoInversionesEmp))
                    .ForMember(dest => dest.BusoMaqUtilAfec, opt => opt.MapFrom(src => src.BusoMaqUtilAfecEmp))
                    .ForMember(dest => dest.BusoMaqUtilNoAfec, opt => opt.MapFrom(src => src.BusoMaqUtilNoAfecEmp))
                    .ForMember(dest => dest.BusoBienesRaicesAfec, opt => opt.MapFrom(src => src.BusoBienesRaicesAfecEmp))
                    .ForMember(dest => dest.BusoBienesRaicesNoAfec, opt => opt.MapFrom(src => src.BusoBienesRaicesNoAfecEmp))
                    .ForMember(dest => dest.BusoOtrosConceptos, opt => opt.MapFrom(src => src.BusoOtrosConceptosEmp))
                    .ForMember(dest => dest.BcamMateriales, opt => opt.MapFrom(src => src.BcamMaterialesEmp))
                    .ForMember(dest => dest.BcamOtrosConceptos, opt => opt.MapFrom(src => src.BcamOtrosConceptosEmp))
                    .ForMember(dest => dest.DeuCortoPlazo, opt => opt.MapFrom(src => src.DeuCortoPlazoEmp))
                    .ForMember(dest => dest.DeuLargoPlazo, opt => opt.MapFrom(src => src.DeuLargoPlazoEmp))
                ;
            });
            _mapper = config.CreateMapper();
            //--

            tramite = await _TramiteBL.GetTramiteByGuidAsync(IdentificadorUnico);
            tramite.PermiteEditarEmpresa = tramite.PermiteEditarFormulario(Constants.Formularios.BalanceGeneral);


            if (IdTramiteBalanceGeneral.HasValue)
            {
                var dto = await _TramiteBL.GetBalanceGeneralAsync(IdTramiteBalanceGeneral.Value);
                Model = _mapper.Map<TramitesBalanceGeneralDTO, BalanceGeneralModel>(dto);
            }

            Model.IdTramite = tramite.IdTramite;

            await base.OnInitializedAsync();
        }

        protected async Task OnClickGuardar(EditContext ed)
        {
            if (IsBusyGuardar)
                return;

            IsBusyGuardar = true;
            try
            {
                if (ed.Validate())
                {
                    var dto = _mapper.Map<BalanceGeneralModel, TramitesBalanceGeneralDTO>(Model);
                    dto.IdTramiteBalanceGeneral = IdTramiteBalanceGeneral;
                    await _TramiteBL.ActualizarBalanceGeneralAsync(dto);

                    notificationService.Notify(NotificationSeverity.Success, "Aviso", "El registro se ha guardado exitosamente", Constants.NotifDuration.Normal);
                    navigationManager.NavigateTo($"/Tramites/BalanceGeneral/{AccionFormAnterior}/{IdentificadorUnico}", true);
                }
                else
                {
                    notificationService.Notify(NotificationSeverity.Warning, "Aviso", "Verifique los campos requeridos...", Constants.NotifDuration.Normal);
                }
            }
            catch (Exception ex)
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", Functions.GetErrorMessage(ex), Constants.NotifDuration.VerySlow);
            }
            
            IsBusyGuardar = false;
        }
        protected void CancelarClick()
        {
            navigationManager.NavigateTo($"/Tramites/BalanceGeneral/{AccionFormAnterior}/{IdentificadorUnico}", true);
        }
        protected void VolverAlVisor()
        {
                navigationManager.NavigateTo($"/Tramites/Visualizar/{IdentificadorUnico}", true);
        }
    }
}
