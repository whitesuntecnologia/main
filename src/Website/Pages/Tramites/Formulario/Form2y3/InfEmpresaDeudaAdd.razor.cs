using AutoMapper;
using Business.Interfaces;
using DataAccess.Entities;
using DataTransferObject;
using DataTransferObject.BLs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using StaticClass;
using Website.Models.Formulario;

namespace Website.Pages.Tramites.Formulario.Form2y3
{
    public partial class InfEmpresaDeudaAdd : ComponentBase
    {
        [Parameter] public string IdentificadorUnico { get; set; } = null!;
        [Parameter] public int? IdTramiteInfEmpDeuda { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private DialogService dialogService { get; set; } = null!;
        [Inject] private ICombosBL _CombosBL { get; set; } = null!;
        [Inject] private ITramitesBL _TramiteBL { get; set; } = null!;
        [Inject] private IUsuariosBL _usuarioBL { get; set; } = null!;

        private IMapper _mapper { get; set; } = null!;
        private TramitesDTO tramite { get; set; } = null!;
        private InfEmpresaDeudaAddModel Model { get; set; } = new();
        private List<GenericComboDTO> lstSituaciones { get; set; } = new();
        private List<GenericComboDTO> lstMeses { get; set; } = new();
        private bool isBusyAceptar { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await CreateMapper();
            tramite = await _TramiteBL.GetTramiteByGuidAsync(IdentificadorUnico);
            //Carga el combo con 6 situaciones
            for (int i = 1; i <= 6; i++)
            {
                lstSituaciones.Add(new GenericComboDTO()
                {
                    Id = i,
                    Descripcion = i.ToString(),
                });
            }
            //--
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

            if(IdTramiteInfEmpDeuda.HasValue)
            {
                var Deuda = await _TramiteBL.GetInfEmpDeudaAsync(IdTramiteInfEmpDeuda.Value);
                Model = _mapper.Map<InfEmpresaDeudaAddModel>(Deuda);
                if (!string.IsNullOrWhiteSpace(Deuda.Periodo))
                {
                    Model.Mes = Convert.ToInt32(Deuda.Periodo.Split("/")[0]);
                    Model.Anio = Convert.ToInt32(Deuda.Periodo.Split("/")[1]);
                }
            }
            await base.OnInitializedAsync();
        }
        private Task CreateMapper()
        {
            //Crear el mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TramitesInfEmpDeudaDTO, InfEmpresaDeudaAddModel>();
            });
            _mapper = config.CreateMapper();
            //--

            return Task.CompletedTask;
        }
        protected async Task OnClickAceptar(EditContext ed)
        {
            if (isBusyAceptar)
                return;

            isBusyAceptar = true;
            if (ed.Validate())
            {

                //Alta o Modificacion de la Deuda
                try
                {
                    if (IdTramiteInfEmpDeuda.HasValue)
                    {
                        //Modificacion
                        var dtoMod = await _TramiteBL.GetInfEmpDeudaAsync(IdTramiteInfEmpDeuda.Value);
                        dtoMod.Entidad = Model.Entidad;
                        dtoMod.DiasDeAtraso = Model.DiasDeAtraso ?? 0;
                        dtoMod.Monto = Model.Monto ?? 0;
                        dtoMod.Periodo = Model.Periodo;
                        dtoMod.Situacion = Model.Situacion ?? 0;
                        dtoMod.LastUpdateUser = await _usuarioBL.GetCurrentUserid();
                        dtoMod.LastUpdateDate = DateTime.Now;
                        await _TramiteBL.ActualizarInfEmpDeudaAsync(dtoMod);
                        
                    }
                    else
                    {
                        var tramite = await _TramiteBL.GetTramiteByGuidAsync(IdentificadorUnico);
                        var tramiteInfEmp = await _TramiteBL.GetInfEmpAsync(tramite.IdTramite);
                        var dto = new TramitesInfEmpDeudaDTO()
                        {
                            IdTramite = tramite.IdTramite,
                            IdTramiteInfEmp = (tramiteInfEmp != null ? tramiteInfEmp.IdTramiteInfEmp : null),
                            Entidad = Model.Entidad,
                            DiasDeAtraso = Model.DiasDeAtraso ?? 0,
                            Monto = Model.Monto ?? 0,
                            Periodo = Model.Periodo,
                            Situacion = Model.Situacion ?? 0,
                            CreateUser = await _usuarioBL.GetCurrentUserid(),
                            CreateDate = DateTime.Now,
                        };
                        await _TramiteBL.AgregarInfEmpDeudaAsync(dto);
                    }
                    
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
