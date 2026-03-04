using AutoMapper;
using Business.Extensions;
using Business.Interfaces;
using DataTransferObject;
using DataTransferObject.BLs;
using DocumentFormat.OpenXml;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.IdentityModel.Abstractions;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using StaticClass;
using System.Globalization;
using System.Runtime.CompilerServices;
using Website.Models.Formulario;
using Website.Pages.Shared.Components;

namespace Website.Pages.Tramites.Formulario.Form9
{
    public partial class RepresentantestTecnicosAdd : ComponentBase
    {
        [Parameter] public string IdentificadorUnico { get; set; } = null!;
        [Parameter] public int? IdRepresentanteTecnico { get; set; } = null!;
        [Parameter] public string AccionFormAnterior { get; set; } = null!; //Ingresar o Editar
        [Parameter] public string Accion { get; set; } = null!; //Ingresar o Editar
        
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private DialogService dialogService { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] protected RouteHistoryService routeHistory { get; set; } = null!;

        [Inject] private ICombosBL _CombosBL { get; set; } = null!;
        [Inject] private ITramitesBL _TramiteBL { get; set; } = null!;
        [Inject] private IUsuariosBL _usuarioBL { get; set; } = null!;
        [Inject] private ITablasBL _tablasBL { get; set; } = null!;
        [Inject] private IFilesBL _filesBL { get; set; } = null!;

        private TramitesDTO tramite { get; set; } = new();
        private bool EvaluandoTramite { get; set; } = false;
        private IEnumerable<GenericComboDTO> lstEspecialidades = new List<GenericComboDTO>();
        private IEnumerable<GenericComboDTO> lstProvincias = new List<GenericComboDTO>();
        private IMapper _mapper { get; set; } = null!;
        private RepresentanteTecnicoModel Model { get; set; } = new();
        private CustomFileUpload? upload;
        private CustomFileUpload? uploadBP;
        private CustomFileUpload? uploadMatricula;
        private CustomFileUpload? uploadCV;

        private List<FileDTO> lstFilesContratos = new();
        private List<FileDTO> lstFilesBoletas = new();
        private List<FileDTO> lstFilesMatricula = new();
        private List<FileDTO> lstFilesCV = new(); 
        private bool isBusyAceptar { get; set; }

        protected override async Task OnInitializedAsync()
        {
            EvaluandoTramite = navigationManager.Uri.Contains("Evaluar");

            //Crear el mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TramitesRepresentanteTecnicoDTO, RepresentanteTecnicoModel>()
                    .ForMember(dest => dest.Jurisdicciones, opt => opt.MapFrom(src => src.TramitesRepresentantesTecnicosJurisdicciones.Select(s => s.IdProvincia)))
                    .ForMember(dest => dest.Especialidades, opt => opt.MapFrom(src => src.TramitesRepresentantesTecnicosEspecialidades.Select(s => s.IdTramiteEspecialidad)))
                ;
            });
            _mapper = config.CreateMapper();
            //--

            tramite = await _TramiteBL.GetTramiteByGuidAsync(IdentificadorUnico);
            tramite.PermiteEditarEmpresa = tramite.PermiteEditarEmpresa && tramite.PermiteEditarFormulario(Constants.Formularios.RepresentantesTecnicos);

            lstEspecialidades = await _CombosBL.GetEspecialidadesFromTramiteAsync(tramite.IdTramite);
            lstProvincias = await _CombosBL.GetProvinciasAsync();
            // Se pone por default LA Pampa
            Model.Jurisdicciones.Add(Constants.ProvLaPampa);

            if (IdRepresentanteTecnico.HasValue)
            {
                var dto = await _TramiteBL.GetRepresentanteAsync(IdRepresentanteTecnico.Value);
                Model = _mapper.Map<RepresentanteTecnicoModel>(dto);
                lstFilesContratos.Add(await _filesBL.GetFileAsync(dto.IdFileContrato));
                lstFilesBoletas.Add(await _filesBL.GetFileAsync(dto.IdFileBoleta));
                lstFilesMatricula.Add(await _filesBL.GetFileAsync(dto.IdFileMatricula));
                if(dto.IdFileCurriculum.HasValue)
                    lstFilesCV.Add(await _filesBL.GetFileAsync(dto.IdFileCurriculum.GetValueOrDefault()));
            }
            
            //Establece el grupo de trámite al modelo
            Model.IdGrupoTramite = tramite.IdGrupoTramite;
            await base.OnInitializedAsync();
        }

        #region File Contrato
        private void HandleFileDeletionRequested(FileDTO fileModel)
        {
            Model.IdFileContrato = null;
            Model.FilenameContrato = null;
            lstFilesContratos = lstFilesContratos.Where(x => x != fileModel).ToList();
        }

        private async Task HandleFileUploaded(FileDTO fileModel)
        {

            if (upload?.Accept?.Length > 0 && upload.Accept != fileModel.ContentType)
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                fileModel.CreateUser = await _usuarioBL.GetCurrentUserid();
                fileModel.CreateDate = DateTime.Now;
                fileModel = await _filesBL.AddFileAsync(fileModel);
                Model.IdFileContrato = fileModel.IdFile;
                Model.FilenameContrato = fileModel.FileName;
            }
        }
        #endregion
        #region File Boleta de Pago
        private void HandleFileDeletionRequestedBP(FileDTO fileModel)
        {
            Model.IdFileBoleta = null;
            Model.FilenameBoleta = null;
            lstFilesBoletas = lstFilesBoletas.Where(x => x != fileModel).ToList();
        }

        private async Task HandleFileUploadedBP(FileDTO fileModel)
        {

            if (uploadBP?.Accept?.Length > 0 && uploadBP.Accept != fileModel.ContentType)
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                fileModel.CreateUser = await _usuarioBL.GetCurrentUserid();
                fileModel.CreateDate = DateTime.Now;
                fileModel = await _filesBL.AddFileAsync(fileModel);
                Model.IdFileBoleta = fileModel.IdFile;
                Model.FilenameBoleta = fileModel.FileName;
            }
        }
        #endregion
        #region File Matricula
        private void HandleFileDeletionRequestedMatricula(FileDTO fileModel)
        {
            Model.IdFileMatricula = null;
            Model.FilenameMatricula = null;
            lstFilesMatricula = lstFilesMatricula.Where(x => x != fileModel).ToList();
        }

        private async Task HandleFileUploadedMatricula(FileDTO fileModel)
        {

            if (uploadMatricula?.Accept?.Length > 0 && uploadMatricula.Accept != fileModel.ContentType)
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                fileModel.CreateUser = await _usuarioBL.GetCurrentUserid();
                fileModel.CreateDate = DateTime.Now;
                fileModel = await _filesBL.AddFileAsync(fileModel);
                Model.IdFileMatricula = fileModel.IdFile;
                Model.FilenameMatricula = fileModel.FileName;
            }
        }
        #endregion

        #region File Curriculum
        private void HandleFileDeletionRequestedCV(FileDTO fileModel)
        {
            Model.IdFileCurriculum = null;
            Model.FilenameCurriculum = null;
            lstFilesCV = lstFilesCV.Where(x => x != fileModel).ToList();
        }

        private async Task HandleFileUploadedCV(FileDTO fileModel)
        {

            if (uploadCV?.Accept?.Length > 0 && uploadCV.Accept != fileModel.ContentType)
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                fileModel.CreateUser = await _usuarioBL.GetCurrentUserid();
                fileModel.CreateDate = DateTime.Now;
                fileModel = await _filesBL.AddFileAsync(fileModel);
                Model.IdFileCurriculum = fileModel.IdFile;
                Model.FilenameCurriculum= fileModel.FileName;
            }
        }
        #endregion
        private void HandleError(string message)
        {
            notificationService.Notify(NotificationSeverity.Error, "Error", message, StaticClass.Constants.NotifDuration.Normal);
        }
        protected async Task OnClickAceptar(EditContext ed)
        {
            if (isBusyAceptar)
                return;

            isBusyAceptar = true;
            if (ed.Validate())
            {

                //Alta del Representante Técnico
                try
                {


                    var provincias = await _tablasBL.GetProvinciasAsync();
                    
                    var representante = new TramitesRepresentanteTecnicoDTO()
                    {
                        IdRepresentanteTecnico = (Model.IdRepresentanteTecnico.HasValue ? Model.IdRepresentanteTecnico.Value : 0),
                        IdTramite = tramite.IdTramite,
                        Apellido = Model.Apellido,
                        Nombres = Model.Nombres,
                        CUIT = Model.CUIT.GetValueOrDefault(),
                        Cargo = Model.Cargo,
                        Matricula = Model.Matricula,
                        FechaVencimientoMatricula = Model.FechaVencimientoMatricula.GetValueOrDefault(),
                        FechaVencimientoContrato = Model.FechaVencimientoContrato.GetValueOrDefault(),
                        TramitesRepresentantesTecnicosJurisdicciones =
                                provincias.Where(x => Model.Jurisdicciones.Contains(x.IdProvincia))
                                          .Select(s => new TramitesRepresentanteTecnicoJurisdiccionDTO()
                                          {
                                              IdProvincia = s.IdProvincia
                                          }).ToList(),
                        TramitesRepresentantesTecnicosEspecialidades = Model.Especialidades.Select(s=> new TramitesRepresentanteTecnicoEspecilidadDTO()
                        {
                             IdTramiteEspecialidad = s
                        }).ToList(),
                        IdFileContrato = Model.IdFileContrato.GetValueOrDefault(),
                        FilenameContrato = Model.FilenameContrato,
                        IdFileBoleta = Model.IdFileBoleta.GetValueOrDefault(),
                        FilenameBoleta = Model.FilenameBoleta,
                        IdFileMatricula = Model.IdFileMatricula.GetValueOrDefault(),
                        FilenameMatricula = Model.FilenameMatricula
                        
                    };
                    
                    //Solo es para el registro de Consultores
                    if(Model.IdFileCurriculum.HasValue)
                    {
                        representante.IdFileCurriculum = Model.IdFileCurriculum.Value;
                        representante.FilenameCurriculum = Model.FilenameCurriculum;
                    }


                    if (IdRepresentanteTecnico.HasValue)
                        await _TramiteBL.ActualizarRepresentanteTecnicoAsync(representante);
                    else
                        await _TramiteBL.AgregarRepresentanteTecnicoAsync(representante);

                    navigationManager.NavigateTo($"/Tramites/RepresentantesTecnicos/{AccionFormAnterior}/{IdentificadorUnico}", true); 
                }
                catch (Exception ex)
                {
                    notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
                }
            }
            isBusyAceptar = false;
        }

        //private Task OnChangeMatricula(string value)
        //{
        //    string numValue = GetNumbers(value ?? "0");
        //    if (!string.IsNullOrWhiteSpace(numValue))
        //    {
        //        if(numValue.Length < 6)
        //            Model.Matricula = numValue.PadLeft(6,Convert.ToChar("0"));
        //        else
        //            Model.Matricula = numValue;
        //    }
        //    else 
        //    { 
        //        Model.Matricula = ""; 
        //    }
            
        //    return Task.CompletedTask;
        //}

        // retornar solo los números 
        private static string GetNumbers(string input)
            => new string(input.Where(c => (char.IsDigit(c) )).ToArray());


        protected async Task OnChangeCuit(decimal? value)
        {
            var representante = await _TramiteBL.GetRepresentanteTecnicoByCuitAsync(value.GetValueOrDefault());

            if (representante != null)
            {
                var ModelR = _mapper.Map<RepresentanteTecnicoModel>(representante);
                Model.Apellido= ModelR.Apellido;
                Model.Nombres = ModelR.Nombres;
                Model.Cargo= ModelR.Cargo;
                Model.Jurisdicciones= ModelR.Jurisdicciones;
                Model.FechaVencimientoContrato = ModelR.FechaVencimientoContrato;
                Model.Matricula = ModelR.Matricula;
                Model.FechaVencimientoMatricula = ModelR.FechaVencimientoMatricula;
            }
        }
        protected void CancelarClick()
        {
            navigationManager.NavigateTo($"/Tramites/RepresentantesTecnicos/{AccionFormAnterior}/{IdentificadorUnico}", true);
        }
        protected void VolverAlVisor()
        {
                navigationManager.NavigateTo($"/Tramites/Visualizar/{IdentificadorUnico}", true);
        }
    }
}

    