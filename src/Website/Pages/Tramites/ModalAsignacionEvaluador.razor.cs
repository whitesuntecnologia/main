using Business.Interfaces;
using DataTransferObject;
using DataTransferObject.BLs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using StaticClass;
using Website.Models.Shared;

namespace Website.Pages.Tramites
{
    public partial class ModalAsignacionEvaluador: ComponentBase
    {
        [Parameter] public int IdTramite { get; set; }
        [Inject] private IUsuariosBL _usuarioBL { get; set; } = null!;
        [Inject] private DialogService dialogService { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private ITramitesEvaluacionBL _TramiteEvaluacionBL { get; set; } = null!;

        private List<GenericComboStrDTO> lstUsuariosAsignacion { get; set; } = new();
        private ReAsignarEvaluadorModel Model { get; set; } = new();
        private bool isBusyAceptar;

        protected override async Task OnInitializedAsync()
        {
            
            List<UsuarioDTO> lstUsuarios = new();
            lstUsuarios.AddRange(await _usuarioBL.GetUsersAsync(null, Constants.Perfiles.Evaluador, 0));
            lstUsuarios.AddRange(await _usuarioBL.GetUsersAsync(null, Constants.Perfiles.Administrador, 0));
            lstUsuariosAsignacion = lstUsuarios.Select( s=> 
                                        new GenericComboStrDTO
                                        {
                                            Id= s.Id,
                                            Descripcion = s.UserName + " - " + s.NombreyApellido
                                        })
                                   .DistinctBy(x=> x.Id)
                                   .ToList();
            
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
                    await _TramiteEvaluacionBL.ActualizarEvaluadorAsignadoAsync(IdTramite, Model.userid);
                    dialogService.Close(true);
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
    }
}
