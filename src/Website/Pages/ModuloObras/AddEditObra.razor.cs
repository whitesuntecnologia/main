using AutoMapper;
using Business;
using Business.Interfaces;
using DataTransferObject;
using DataTransferObject.BLs;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using StaticClass;
using Website.Models.Formulario;
using Website.Models.ModuloObras;
using Website.Models.Parametros;
using Website.Pages.Shared.Components;

namespace Website.Pages.ModuloObras
{
    
    public partial class AddEditObra: ComponentBase
    {
        [Parameter] public string Accion { get; set; } = null!;
        [Parameter] public int? IdObra { get; set; }

        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] NotificationService notificationService { get; set; } = null!;
        [Inject] private ICombosBL _combosBL { get; set; } = null!;
        [Inject] IObrasProvinciaLaPampaBL obrasBL { get; set; } = null!;
        [Inject] private IUsuariosBL _usuarioBL { get; set; } = null!;
        [Inject] private IFilesBL _filesBL { get; set; } = null!;


        private ObrasPciaLPAddModel Model { get; set; } = new();
        private IMapper _mapper { get; set; } = null!;
        private bool isBusyAceptar { get; set; }
        private bool isBaja { get; set; }
        private IEnumerable<GenericComboStrDTO> lstEstadosObra = new List<GenericComboStrDTO>();
        private List<GenericComboDTO> lstMeses { get; set; } = new();

        private CustomFileUpload? uploadIC;
        private List<FileDTO> lstFilesIC = new();
        private bool isBusyFileUploadIC { get; set; }

        private EditContext context = new EditContext(new EquiposAddModel());
        protected override async Task OnInitializedAsync()
        {
            //Crear el mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ObrasPciaLPAddModel, ObrasProvinciaLaPampaDTO>().ReverseMap();
            });
            _mapper = config.CreateMapper();
            //--

            if (IdObra.HasValue)
            {
                var dto = await obrasBL.GetObraAsync(IdObra.Value);
                Model = _mapper.Map<ObrasPciaLPAddModel>(dto);
                isBaja = Model.BajaLogica;
            }
            lstEstadosObra = await _combosBL.GetEstadosObraAsync();

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
            context = new EditContext(Model);
            
            if(Model.IdFileInformeCoeficiente.HasValue)
                lstFilesIC.Add(await _filesBL.GetFileAsync(Model.IdFileInformeCoeficiente.GetValueOrDefault()));

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
                    var dto = _mapper.Map<ObrasProvinciaLaPampaDTO>(Model);
                    dto.EsAltaPorProceso = false;
                    dto.EsAltaPorUsuario = true;
                    

                    if (Model.IdObraPciaLp.HasValue)
                        await obrasBL.ActualizarObraAsync(dto);
                    else
                        await obrasBL.AgregarObraAsync(dto);

                    navigationManager.NavigateTo("/ModuloObras");
                }
                catch (Exception ex)
                {
                    notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
                }
            }

            isBusyAceptar = false;
        }
        protected Task CancelarClick()
        {
            navigationManager.NavigateTo("/ModuloObras");
            return Task.CompletedTask;
        }
        protected Task chkEsUTEValueChanged(bool value)
        {
            if (!value)
            {
                Model.PorcentajeParticipacion = null;
            }
            return Task.CompletedTask;
        }

        #region Metodos File Informe Coeficiente Conceptual
        private void HandleFileDeletionRequestedIC(FileDTO fileModel)
        {
            Model.IdFileInformeCoeficiente = null;
            Model.IdFileInformeCoeficiente = null;
            lstFilesIC = lstFilesIC.Where(x => x != fileModel).ToList();
        }

        private async Task HandleFileUploadedIC(FileDTO fileModel)
        {


            if (uploadIC?.Accept?.Length > 0 && !uploadIC.Accept.Split(",").Contains(fileModel.ContentType))
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido (Archivos Excel).", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                fileModel.CreateUser = await _usuarioBL.GetCurrentUserid();
                fileModel.CreateDate = DateTime.Now;
                fileModel = await _filesBL.AddFileAsync(fileModel);
                Model.IdFileInformeCoeficiente = fileModel.IdFile;
                Model.FilenameInformeCoeficiente = fileModel.FileName;
            }
            isBusyFileUploadIC = false;
            context?.Validate();

        }
        private void HandleErrorIC(string message)
        {
            isBusyFileUploadIC = false;
            notificationService.Notify(NotificationSeverity.Error, "Error", message, StaticClass.Constants.NotifDuration.Normal);
        }
        protected Task OnProgressIC(UploadProgressArgs args)
        {
            if (args.Progress > 0 && args.Progress != 100)
                isBusyFileUploadIC = true;

            return Task.CompletedTask;
        }
        #endregion
    }
}
