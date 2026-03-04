using AutoMapper;
using Business.Extensions;
using Business.Interfaces;
using DataTransferObject;
using DataTransferObject.BLs;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using StaticClass;
using Website.Models.Formulario;
using Website.Pages.Shared.Components;

namespace Website.Pages.Tramites.Formulario.Form10
{
    public partial class ObrasAddEdit : ComponentBase
    {
        [Parameter] public string IdentificadorUnico { get; set; } = null!;
        [Parameter] public int? IdTramiteObra { get; set; } = null!;
        [Parameter] public string AccionFormAnterior { get; set; } = null!; //Ingresar o Editar
        [Parameter] public string Accion { get; set; } = null!; //Ingresar o Editar

        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private DialogService dialogService { get; set; } = null!;
        [Inject] protected RouteHistoryService routeHistory { get; set; } = null!;

        [Inject] private ICombosBL _CombosBL { get; set; } = null!;
        [Inject] private ITramitesBL _TramiteBL { get; set; } = null!;
        [Inject] private IUsuariosBL _usuarioBL { get; set; } = null!;
        [Inject] private IFilesBL _filesBL { get; set; } = null!;
        [Inject] private IObrasProvinciaLaPampaBL _obrasLpBL { get; set; } = null!;


        private IMapper _mapper { get; set; } = null!;

        private ObrasModel Model {get;set;} = new();
        private TramitesDTO tramite { get; set; } = new();
        private bool EvaluandoTramite { get; set; } = false;
        private IEnumerable<GenericComboDTO> lstEspecialidades = new List<GenericComboDTO>();
        private IEnumerable<GenericComboDTO> lstSecciones = new List<GenericComboDTO>();
        private IEnumerable<GenericComboDTO> lstTiposDeObra = new List<GenericComboDTO>();
        private IEnumerable<GenericComboDTO> lstObrasPciaLP = new List<GenericComboDTO>();
        
        private List<GenericComboDTO> lstMeses { get; set; } = new();
        private bool isBusyAceptar { get; set; }

        private CustomFileUpload? upload;
        private List<FileDTO> lstFiles = new();
        private bool isBusyFileUploadDE { get; set; }

        private EditContext? context;

        protected override async Task OnInitializedAsync()
        {
            EvaluandoTramite = navigationManager.Uri.Contains("Evaluar");

            //Crear el mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ObrasModel, TramitesObrasDTO>().ReverseMap();
            });
            _mapper = config.CreateMapper();
            //--

            if (IdTramiteObra.HasValue)
            {
                var dto = await _TramiteBL.GetObraAsync(IdTramiteObra.Value);
                Model = _mapper.Map<ObrasModel>(dto);
                lstFiles.Add(await _filesBL.GetFileAsync(Model.IdFile.GetValueOrDefault()));
            }
            
            tramite = await _TramiteBL.GetTramiteByGuidAsync(IdentificadorUnico);
            tramite.PermiteEditarEmpresa = tramite.PermiteEditarFormulario(Constants.Formularios.Obras);

            lstEspecialidades = await _CombosBL.GetEspecialidadesFromTramiteAsync(tramite.IdTramite);
            if(Model != null && Model.IdTramiteEspecialidad.HasValue)
                lstSecciones = await _CombosBL.GetSeccionesFromEspecialidadTramiteAsync(Model.IdTramiteEspecialidad.Value);
            
            lstTiposDeObra = await _CombosBL.GetTiposDeObraAsync();
            //Si es Alta trae solo las activas y si es visualizacion trae todas por si se dio de baja
            lstObrasPciaLP = await _CombosBL.GetObrasPciaLP(tramite.PermiteEditarEmpresa);

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
        protected async Task OnChangeEspecialidades(object value)
        {
            if (value != null)
            {
                int IdTramiteEspecialidad = Convert.ToInt32(value);
                
                lstSecciones = await _CombosBL.GetSeccionesFromEspecialidadTramiteAsync(IdTramiteEspecialidad);
                if (lstSecciones.Count() == 1)
                    Model.IdTramiteEspecialidadSeccion = lstSecciones.First().Id;
                else
                    Model.IdTramiteEspecialidadSeccion = null;
                            }
            else
                lstSecciones = new List<GenericComboDTO>();
        }
        
        //protected async Task OnChangeObras(object value)
        //{
        //    Model.Expediente = string.Empty;
        //    Model.Designacion = string.Empty;

        //    if (value != null)
        //    {
        //        int IdObraPciLp = Convert.ToInt32(value);
        //        var dto = await _obrasPciaLpBL.GetObraAsync(IdObraPciLp);
        //        if(dto != null)
        //        {
        //            Model.Expediente = dto.Expediente;
        //            Model.Designacion = dto.ObraNombre;
        //        }
        //    }
        //}

        #region Metodos File Contrato
        private void HandleFileDeletionRequestedDE(FileDTO fileModel)
        {
            Model.IdFile = null;
            Model.Filename = null;
            lstFiles = lstFiles.Where(x => x != fileModel).ToList();
        }

        private async Task HandleFileUploadedDE(FileDTO fileModel)
        {


            if (upload?.Accept?.Length > 0 && !upload.Accept.Split(",").Contains(fileModel.ContentType))
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido (Archivos Excel).", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                fileModel.CreateUser = await _usuarioBL.GetCurrentUserid();
                fileModel.CreateDate = DateTime.Now;
                fileModel = await _filesBL.AddFileAsync(fileModel);
                Model.IdFile= fileModel.IdFile;
                Model.Filename= fileModel.FileName;
            }
            isBusyFileUploadDE = false;
            context?.NotifyFieldChanged(context.Field("IdFileDetalleInmueble"));

        }
        private void HandleErrorDE(string message)
        {
            isBusyFileUploadDE = false;
            notificationService.Notify(NotificationSeverity.Error, "Error", message, StaticClass.Constants.NotifDuration.Normal);
        }
        protected Task OnProgressDE(UploadProgressArgs args)
        {
            if (args.Progress > 0 && args.Progress != 100)
                isBusyFileUploadDE = true;

            return Task.CompletedTask;
        }
        #endregion

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
                    if (await ValidacionesAlGuardarAsync())
                    {
                        var dto = _mapper.Map<ObrasModel, TramitesObrasDTO>(Model);
                        dto.IdTramite = tramite.IdTramite;

                        if (Model.IdTramiteObra.HasValue)
                            await _TramiteBL.ActualizarObraAsync(dto);
                        else
                            await _TramiteBL.AgregarObraAsync(dto);

                        navigationManager.NavigateTo($"/Tramites/Obras/{AccionFormAnterior}/{IdentificadorUnico}", true);
                    }
                    else
                        isBusyAceptar = false;
                }
                catch (Exception ex)
                {
                    notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
                    isBusyAceptar = false;
                }
            }
            else
                isBusyAceptar = false;
        }

        private async Task<bool> ValidacionesAlGuardarAsync()
        {
            List<string> mensajes = new List<string>();

            var lstObras = await _TramiteBL.GetObrasAsync(tramite.IdTramite);
            if(lstObras.Any(x=> x.IdObraPciaLp == Model.IdObraPciaLP && x.IdTramiteObra != Model.IdTramiteObra))
            {
                mensajes.Add("Ya existe un registro con la misma Obra.");
            }

            if (mensajes.Count > 0)
            {
                await dialogService.OpenAsync<ModalValidaciones>("Validaciones",
                                       new Dictionary<string, object>() { { "Mensajes", mensajes }, { "Title", "Se han encontrado validaciones a revisar:" } },
                                       new DialogOptions() { Width = "auto", Height = "auto", Resizable = false });

            }
            return mensajes.Count == 0;
        }

        protected void CancelarClick()
        {
            navigationManager.NavigateTo($"/Tramites/Obras/{AccionFormAnterior}/{IdentificadorUnico}", true);
        }
        protected void VolverAlVisor()
        {
            navigationManager.NavigateTo($"/Tramites/{AccionFormAnterior}/{IdentificadorUnico}", true);
        }
        protected async Task AgregarObraClick()
        {
            var respuesta = await dialogService.OpenAsync<ObrasAddModal>("Agregar Obra",
                                       null,
                                       new DialogOptions() { Width = "60%", Height = "auto", Resizable = true });

            if (respuesta != null && respuesta > 0)
            {
                //Reload Combo
                Model.IdObraPciaLP = respuesta;
                lstObrasPciaLP = await _CombosBL.GetObrasPciaLP(tramite.PermiteEditarEmpresa);

            }
        }
        protected async Task ddlObraChanged(object IdObraLp)
        {
            var obraDTO = await _obrasLpBL.GetObraAsync(Convert.ToInt32(IdObraLp));
            if (obraDTO != null)
            {
                
                Model.Monto = obraDTO.MontoObra;

                if(obraDTO.FechaFinObra.HasValue) 
                {
                    Model.AnioFin = obraDTO.FechaFinObra.GetValueOrDefault().Year;
                    Model.MesFin = obraDTO.FechaFinObra.GetValueOrDefault().Month;
                }

            }
        }
    }

}
