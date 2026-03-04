using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Radzen;
using Business.Interfaces;
using DataTransferObject;
using System.Security.Claims;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Runtime.CompilerServices;
using DataAccess.Entities;
using DataTransferObject.BLs;
using Business;
using StaticClass;

namespace Website.Pages.Tramites
{

    public partial class IniciarTramite: ComponentBase
    {
        [Inject] private DialogService DS { get; set; } = null!;
        [Inject] private NavigationManager NM { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private ITramitesBL _tramitesBL { get; set; }= null!;
        [Inject] private IUsuariosBL _usuarioBL { get; set; } = null!;
        [Inject] private IEmpresasBL _empresaBL { get; set; } = null!;
        private TramitesDTO TramitePendiente { get; set; } = null!;
        private EmpresaDTO EmpresaDto { get; set; } = null!;
        private bool EmpresaVencida { get; set; } = false;
        private bool isPageLoaded { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            int IdEmpresa = 0;
            int.TryParse(await _usuarioBL.GetCurrentIdEmpresa(), out IdEmpresa);
            
            //Obtiene la empresa a partir del usuario logueado
            EmpresaDto = await _empresaBL.GetEmpresaAsync(IdEmpresa);
            if(EmpresaDto == null)
            {
                notificationService.Notify(NotificationSeverity.Error, "Aviso", "No se pudo obtener la información de la empresa.", StaticClass.Constants.NotifDuration.Normal);
                return;
            }
            
            //Verifica si la empresa está vencida hace más de dos años
            if (EmpresaDto.Vencimiento.HasValue && DateTime.Today > EmpresaDto.Vencimiento.Value.AddYears(2))
                EmpresaVencida = true;

            isPageLoaded = true;

            //Si no tiene un trmaite pendiente y la empresa está vencida o NO tiene fecha de vencimiento, genera un nuevo trámite
            if (!(await TieneTramitePendiente()) && (EmpresaVencida || !EmpresaDto.Vencimiento.HasValue))
                await GenerarTramite();


        }
        public async Task GenerarTramite()
        {
            var respuesta = await DS.Confirm("¿Está seguro de iniciar un nuevo trámite?" ,
                                 "Quiero Inscribirme",
                                 new ConfirmOptions() { OkButtonText= "Sí", CancelButtonText ="No"  }
                                 );

            
            if(respuesta.HasValue && respuesta.Value) //Si
            {
                try
                {
                    var tramite = await _tramitesBL.CrearTramiteAsync(Constants.TiposDeTramite.Reli_Inscripcion);
                    NM.NavigateTo($"/Tramites/Visualizar/{tramite.IdentificadorUnico}");
                }
                catch (Exception ex)
                {
                    notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
                }
                
            }
            else
            {
                NM.NavigateTo($"/1");
            }
        }
        private async Task<bool> TieneTramitePendiente()
        {
            
            int userIdEmpresa =0;
            int.TryParse(await _usuarioBL.GetCurrentIdEmpresa(), out userIdEmpresa);
            TramitePendiente = await _tramitesBL.GetTramitePendienteAsync(Constants.GruposDeTramite.RegistroLicitadores, userIdEmpresa);

            return TramitePendiente != null;
        }

        private async Task<bool> EsEmpresaVencida(int IdEmpresa)
        {
            bool returnValue = false;
            
            //Si la fecha actual aun no supera la fecha de vencimiento mas 2 años, no se puede iniciar una nueva inscripción.
            return returnValue;
        }
    }

}

