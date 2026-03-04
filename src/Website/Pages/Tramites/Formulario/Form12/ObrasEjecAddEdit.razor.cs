using AutoMapper;
using Business;
using Business.Extensions;
using Business.Interfaces;
using DataTransferObject;
using DataTransferObject.BLs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using Radzen.Blazor;
using StaticClass;
using System.Linq.Dynamic.Core;
using Website.Models.Formulario;
using Website.Pages.Shared.Components;

namespace Website.Pages.Tramites.Formulario.Form12
{
    public partial class ObrasEjecAddEdit : ComponentBase
    {
        [Parameter] public string IdentificadorUnico { get; set; } = null!;
        [Parameter] public int? IdTramiteObraEjec { get; set; } = null!;
        [Parameter] public string AccionFormAnterior { get; set; } = null!; //Ingresar o Editar
        [Parameter] public string Accion { get; set; } = null!; //Ingresar o Editar

        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private DialogService dialogService { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] protected RouteHistoryService routeHistory { get; set; } = null!;

        [Inject] private ICombosBL _CombosBL { get; set; } = null!;
        [Inject] private ITramitesBL _TramiteBL { get; set; } = null!;
        [Inject] private IUsuariosBL _usuarioBL { get; set; } = null!;
        [Inject] private IFilesBL _filesBL { get; set; } = null!;
        [Inject] private IObrasProvinciaLaPampaBL _obrasLpBL { get; set; } = null!;

        private ObraEjecucionModel Model { get; set; } = new();
        private TramitesDTO tramite { get; set; } = new();
        private IMapper _mapper { get; set; } = null!;
        private bool EvaluandoTramite { get; set; } = false;
        private IEnumerable<GenericComboDTO> lstEspecialidades = new List<GenericComboDTO>();
        private IEnumerable<GenericComboDTO> lstTiposDeObra = new List<GenericComboDTO>();
        private IEnumerable<GenericComboDTO> lstObrasPciaLP = new List<GenericComboDTO>();
        private List<GenericComboDTO> lstMeses { get; set; } = new();
        private bool isBusyAceptar { get; set; }

        private CustomFileUpload? upload;
        private List<FileDTO> lstFiles = new();
        private bool isBusyFileUpload { get; set; }

        private EditContext? context;

        protected override async Task OnInitializedAsync()
        {
            EvaluandoTramite = navigationManager.Uri.Contains("Evaluar");
            //Crear el mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ObraEjecucionModel, TramitesObraEjecucionDTO>().ReverseMap();
            });
            _mapper = config.CreateMapper();
            //--

            tramite = await _TramiteBL.GetTramiteByGuidAsync(IdentificadorUnico);
            tramite.PermiteEditarEmpresa = tramite.PermiteEditarFormulario(Constants.Formularios.ObrasEnEjecucion);

            if (IdTramiteObraEjec.HasValue)
            {
                var dto = await _TramiteBL.GetObraEjecucionAsync(IdTramiteObraEjec.Value);
                Model = _mapper.Map<TramitesObraEjecucionDTO, ObraEjecucionModel>(dto);
                lstFiles.Add(await _filesBL.GetFileAsync(Model.IdFile.GetValueOrDefault()));
            }

            Model.FechaCertificacion =  await _TramiteBL.GetFechaCertificacionAsync(tramite.IdTramite) ?? DateTime.Today;

            lstEspecialidades = await _CombosBL.GetEspecialidadesFromTramiteAsync(tramite.IdTramite);
            lstTiposDeObra = await _CombosBL.GetTiposDeObraAsync();
            //Si es Alta trae solo las activas y si es visualizacion trae todas por si se dio de baja
            lstObrasPciaLP = await _CombosBL.GetObrasPciaLP(tramite.PermiteEditarEmpresa,tramite.CuitEmpresa.ToString());

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

        private Task ReValidate(string fieldName)
        {
            if (context != null)
            {
                FieldIdentifier field = context.Field(fieldName);
                context?.NotifyFieldChanged(field);
            }
            return Task.CompletedTask;
        }

        #region Metodos File Contrato
        private void HandleFileDeletionRequested(FileDTO fileModel)
        {
            Model.IdFile = null;
            Model.Filename = null;
            lstFiles = lstFiles.Where(x => x != fileModel).ToList();
        }

        private async Task HandleFileUploaded(FileDTO fileModel)
        {

            if (upload?.Accept?.Length > 0 && !upload.Accept.Split(",").Contains(fileModel.ContentType))
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido.", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                fileModel.CreateUser = await _usuarioBL.GetCurrentUserid();
                fileModel.CreateDate = DateTime.Now;
                fileModel = await _filesBL.AddFileAsync(fileModel);
                Model.IdFile = fileModel.IdFile;
                Model.Filename = fileModel.FileName;
            }
            isBusyFileUpload = false;
            context?.NotifyFieldChanged(context.Field("IdFileDetalleInmueble"));

        }
        private void HandleError(string message)
        {
            isBusyFileUpload = false;
            notificationService.Notify(NotificationSeverity.Error, "Error", message, StaticClass.Constants.NotifDuration.Normal);
        }
        protected Task OnProgress(UploadProgressArgs args)
        {
            if (args.Progress > 0 && args.Progress != 100)
                isBusyFileUpload = true;

            return Task.CompletedTask;
        }
        #endregion

        protected void CancelarClick()
        {
            navigationManager.NavigateTo($"/Tramites/ObrasEjecucion/{AccionFormAnterior}/{IdentificadorUnico}", true);
        }

        protected async Task OnClickGuardar(EditContext Ed)
        {
            if (isBusyAceptar)
                return;

            isBusyAceptar = true;
            context = Ed;
            if (Ed != null && Ed.Validate() )
            {
                try
                {
                    if (await ValidacionesAlGuardarAsync())
                    {
                        var dto = _mapper.Map<ObraEjecucionModel, TramitesObraEjecucionDTO>(Model);
                        dto.IdTramite = tramite.IdTramite;

                        if (Model.IdTramiteObraEjec.HasValue)
                            await _TramiteBL.ActualizarObraEjecucionAsync(dto);
                        else
                            await _TramiteBL.AgregarObraEjecucionAsync(dto);

                        navigationManager.NavigateTo($"/Tramites/ObrasEjecucion/{AccionFormAnterior}/{IdentificadorUnico}", true);
                    }
                }
                catch (Exception ex)
                {
                    notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
                }
            }
            else
            {
                notificationService.Notify(NotificationSeverity.Error, "Aviso", "Revise los campos inválidos.", StaticClass.Constants.NotifDuration.Normal);
            }
            isBusyAceptar = false;
        }

        private async Task<bool> ValidacionesAlGuardarAsync()
        {
            List<string> mensajes = new List<string>();
            var lstObras = await _TramiteBL.GetObrasEjecucionAsync(tramite.IdTramite);

            if(lstObras.Any(a=> a.IdObraPciaLp == Model.IdObraPciaLp && a.IdTramiteObraEjec != Model.IdTramiteObraEjec))
            {
                var obra = lstObras.First(x => x.IdObraPciaLp == Model.IdObraPciaLp);
                mensajes.Add($"La obra {obra.ObraNombre}, ya existe en la grilla. No es posible agregarla nuevamente.");
            }

            if (mensajes.Count > 0)
            {
                await dialogService.OpenAsync<ModalValidaciones>("Validaciones",
                                       new Dictionary<string, object>() { { "Mensajes", mensajes }, { "Title", "Se han encontrado validaciones a revisar:" } },
                                       new DialogOptions() { Width = "70%", Height = "auto", Resizable = false });

            }
            return mensajes.Count == 0;
        }
        protected void VolverAlVisor()
        {
                navigationManager.NavigateTo($"/Tramites/Visualizar/{IdentificadorUnico}", true);
        }
        protected async Task AgregarObraClick()
        {
            var respuesta = await dialogService.OpenAsync<ObrasAddModal>("Agregar Obra",
                                       null,
                                       new DialogOptions() { Width = "60%", Height = "auto", Resizable = true });

            if (respuesta != null && respuesta > 0)
            {
                //Reload Combo
                Model.IdObraPciaLp = respuesta;
                lstObrasPciaLP = await _CombosBL.GetObrasPciaLP(tramite.PermiteEditarEmpresa);

            }
        }

        protected async Task ddlObraChanged(object IdObraLp)
        {
            var obraDTO = await _obrasLpBL.GetObraAsync(Convert.ToInt32(IdObraLp));
            if(obraDTO != null)
            {
                Model.Expediente = obraDTO.Expediente;
                Model.TotalContratado = obraDTO.MontoObra;
                
                decimal valorPlazo = obraDTO.PlazoObra.GetValueOrDefault() / 30;    // El plazo en las obras de La Pampa esta expresado en días
                Model.PlazoObra = Convert.ToInt32(Math.Ceiling(valorPlazo));
            }
        }
    }
}
