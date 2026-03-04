using AutoMapper;
using Business.Interfaces;
using DataTransferObject;
using DataTransferObject.BLs;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using StaticClass;
using Website.Models.Formulario;

namespace Website.Pages.Tramites.RECO.InfEmpresa
{
    public partial class DeudaAdd: ComponentBase
    {
        [Parameter] public string IdentificadorUnico { get; set; } = null!;
        [Parameter] public InfEmpresaConsultoraDeudaModel Model { get; set; } = new();
        [Parameter] public Func<InfEmpresaConsultoraDeudaModel, Task>? OnUpdate { get; set; }
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private DialogService dialogService { get; set; } = null!;

        private List<GenericComboDTO> lstSituaciones { get; set; } = new();
        private List<GenericComboDTO> lstMeses { get; set; } = new();
        private bool isBusyAceptar { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            
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

            await base.OnInitializedAsync();
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
                    // Invocar el evento OnUpdate pasando el Model
                    if (OnUpdate != null)
                    {
                        await OnUpdate.Invoke(Model);
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
