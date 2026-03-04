using AutoMapper;
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

namespace Website.Pages.Tramites.Formulario.Form11
{
    public partial class AntecedentesObrasAddEdit : ComponentBase
    {
        [Parameter] public string IdentificadorUnico { get; set; } = null!;
        [Parameter] public int? IdTramiteEspecialidad { get; set; } = null!;
        [Parameter] public int? IdTramiteEspecialidadSeccion { get; set; } = null!;
        [Parameter] public int? IdTramiteAntecedente { get; set; } = null!; 
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
        [Inject] private IAntecedentesBl _AntecedentesBL { get; set; } = null!;

        private AntecedentesModel Model { get; set; } = new();
        private TramitesDTO tramite { get; set; } = new();
        private IMapper _mapper { get; set; } = null!;
        private bool EvaluandoTramite { get; set; } = false;
        private IEnumerable<GenericComboDTO> lstObrasPciaLP = new List<GenericComboDTO>();
        private List<GenericComboDTO> lstMeses = new List<GenericComboDTO>();

        private bool isBusyAceptar { get; set; }
        private CustomFileUpload? upload;
        private List<FileDTO> lstFiles = new();
        private bool isBusyFileUpload { get; set; }
        private EditContext? context;

        protected override async Task OnInitializedAsync()
        {
            EvaluandoTramite = navigationManager.Uri.Contains("Evaluar");
            // Comprueba la seguridad del trámite
            if (!await _TramiteBL.ComprobarSeguridad(IdentificadorUnico))
            {
                navigationManager.NavigateTo("/ValidationError/401", true);
                return;
            }
            //--

            //Crear el mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AntecedentesModel, TramitesAntecedentesDto>().ReverseMap();
            });
            _mapper = config.CreateMapper();
            //--


            if (IdTramiteAntecedente.HasValue)
            {
                //Editar
                var dto  = await _AntecedentesBL.GetAntecedenteAsync(IdTramiteAntecedente.Value);
                Model = _mapper.Map<TramitesAntecedentesDto, AntecedentesModel>(dto);
                lstFiles.Add(await _filesBL.GetFileAsync(Model.IdFile.GetValueOrDefault()));
            }
            else
            {
                //Agregar
                var Especialidad = await _TramiteBL.GetEspecialidadAsync(IdTramiteEspecialidad.GetValueOrDefault());
                var Seccion = Especialidad.Secciones.First(x => x.IdTramiteEspecialidadSeccion == IdTramiteEspecialidadSeccion.GetValueOrDefault());
                Model.IdTramiteEspecialidad = IdTramiteEspecialidad.GetValueOrDefault();
                Model.DescripcionEspecialidad = Especialidad.DescripcionEspecialidad;
                Model.IdTramiteEspecialidadSeccion = IdTramiteEspecialidadSeccion.GetValueOrDefault();
                Model.DescripcionSeccion = Seccion.DescripcionSeccion;
            }

            tramite = await _TramiteBL.GetTramiteByGuidAsync(IdentificadorUnico);
            tramite.PermiteEditarEmpresa = tramite.PermiteEditarFormulario(Constants.Formularios.AntecedentesDeProduccion);
            Model.FechaInicioSolicitud = tramite.CreateDate;

            //Llena los Doce meses del Año
            for (int i = 1; i <= 12; i++)
            {
                lstMeses.Add(new GenericComboDTO
                {
                    Id = i,
                    Descripcion = i.ToString() // acá va el número como texto
                });
            }

            //Si es Alta trae solo las activas y si es visualizacion trae todas por si se dio de baja
            lstObrasPciaLP = await _CombosBL.GetObrasPciaLP(tramite.PermiteEditarEmpresa);


            await base.OnInitializedAsync();
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
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido (Archivos Excel).", StaticClass.Constants.NotifDuration.Normal);
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
            navigationManager.NavigateTo($"/Tramites/Antecedentes/{AccionFormAnterior}/{IdentificadorUnico}", true);
        }


        protected async Task OnClickGuardar(EditContext Ed)
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
                        var dto = _mapper.Map<AntecedentesModel, TramitesAntecedentesDto>(Model);
                        dto.IdTramite = tramite.IdTramite;

                        if (Model.IdTramiteAntecedente.HasValue)
                            await _AntecedentesBL.ActualizarAntecedenteAsync(dto);
                        else
                            await _AntecedentesBL.AgregarAntecedenteAsync(dto);

                        navigationManager.NavigateTo($"/Tramites/Antecedentes/{AccionFormAnterior}/{IdentificadorUnico}", true);
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
            {
                notificationService.Notify(NotificationSeverity.Error, "Aviso", "Revise los campos inválidos.", StaticClass.Constants.NotifDuration.Normal);
                isBusyAceptar = false;
            }
            
        }

        private async Task<bool> ValidacionesAlGuardarAsync()
        {
            List<string> mensajes = new List<string>();
            //Se pidio que se saque en mail enviado por Javier el  30/08
            //if((Model.MontoEjecutado ?? 0 ) > (Model.AntecedentesDDJJMensual.Sum(x=> x.Monto) ?? 0 ))
            //{
            //    mensajes.Add("El monto ejecutado debe ser menor o igual a la sumatoria de montos de la certificación mensual");
            //}

            if(mensajes.Count > 0) { 
                await dialogService.OpenAsync<ModalValidaciones>("Validaciones",
                                       new Dictionary<string, object>() { { "Mensajes", mensajes }, { "Title", "Se han encontrado validaciones a revisar:" } },
                                       new DialogOptions() { Width = "auto", Height = "auto", Resizable = false });

            }
            return mensajes.Count == 0;
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
                Model.IdObraPciaLp = respuesta;
                lstObrasPciaLP = await _CombosBL.GetObrasPciaLP(tramite.PermiteEditarEmpresa);

            }
        }
        protected async Task ddlObraChanged(object IdObraLp)
        {
            var obraDTO = await _obrasLpBL.GetObraAsync(Convert.ToInt32(IdObraLp));
            if (obraDTO != null)
            {
                Model.Plazo = obraDTO.PlazoObra;
                Model.MontoContrato = obraDTO.MontoObra;
                
            }
        }
    }
}


