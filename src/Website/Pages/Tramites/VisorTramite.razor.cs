using Business;
using Business.Extensions;
using Business.Interfaces;
using DataAccess.Entities;
using DataTransferObject;
using DataTransferObject.BLs;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using Radzen.Blazor.Rendering;
using StaticClass;
using System.Security.Policy;
using System.Text;
using Website.Models.Formulario;
using Website.Models.Shared;
using Website.Pages.Shared;
using Website.Pages.Shared.Components;
using Website.Pages.Tramites.Formulario;
using static StaticClass.Constants;
using ConsultaDocumentoCnn = Services.Gedo.ConsultaDocumentoCnn;
using GenerarDocumentoCnn = Services.Gedo.GenerarDocumentoCnn;

namespace Website.Pages.Tramites
{
    public partial class VisorTramite : ComponentBase
    {
        [Parameter] public string IdentificadorUnico { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private DialogService DS { get; set; } = null!;
        [Inject] protected RouteHistoryService routeHistory { get; set; } = null!;
        [Inject] private IWebHostEnvironment WebHostEnvironment { get; set; } = null!;
        [Inject] private ITramitesBL _TramiteBL { get; set; } = null!;
        [Inject] private ITramitesEvaluacionBL _TramiteEvaluacionBL { get; set; } = null!;
        [Inject] private INotificacionesBL _notificacionesBL { get; set; } = null!;
        [Inject] private IGedoBL _gedoBL { get; set; } = null!;
        [Inject] private IUsuariosBL _usuarioBL { get; set; } = null!;
        [Inject] private IReportesBL _reportesBL { get; set; } = null!;
        [Inject] private IParametrosBL _paramBL { get; set; } = null!;
        [Inject] private IFilesBL _filesBL { get; set; } = null!;
        [Inject] IJSRuntime JSRuntime { get; set; } = null!;
        private bool EvaluandoTramite { get; set; } = false;
        private TramitesDTO tramite { get; set; } = null!;
        private List<ItemGrillaVisorTramiteDTO> lstFormularios { get; set; } = new();
        private RadzenDataList<ItemGrillaVisorTramiteDTO>? dlFormularios = null;
        private Dictionary<string, string> lstBreadcrumb { get; set; } = new();
        private List<PerfilDTO> lstPerfiles { get; set; } = new();
        private bool btnPresentarTramiteVisible { get; set; } = false;
        private bool btnAnularTramiteVisible { get; set; } = false;
        private bool btnAprobarNotificarTramiteVisible { get; set; } = false;
        private string btnAprobarNotificarTramiteText { get; set; } = string.Empty;
        private string btnAprobarNotificarTramiteClass { get; set; } = string.Empty;
        private bool isBusyAprobado { get; set; } = false;
        private bool isBusyActualizacion { get; set; } = false;
        private bool isBusyInitialLoading { get; set; } = true;
        private bool isDebug { get; set; } = true;
        private PresentaEnUteModel UteModel { get; set; } = new();
        private bool isBusyGuardarUte { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {

            EvaluandoTramite = await _TramiteBL.isEvaluandoTramite(IdentificadorUnico);
            // Comprueba la seguridad del trámite
            if (!await _TramiteBL.ComprobarSeguridad(IdentificadorUnico))
            {
                navigationManager.NavigateTo("/ValidationError/401", true);
                return;
            }
            //--
            string userid = await _usuarioBL.GetCurrentUserid();
            lstPerfiles = await _usuarioBL.GetPerfilesForUserAsync(userid);

            //si el usuario es un Administrador se cargan los usuarios posibles de asignacion

            tramite = await _TramiteBL.GetTramiteByGuidAsync(IdentificadorUnico);
            string Accion = "Visualizar";
            isDebug = await _paramBL.GetSettingAsync<bool>("IsDebug");

            if (tramite != null)
            {
                UteModel.SePresentaEnUte = tramite.SePresentaEnUte;
                UteModel.PorcParticipUte = tramite.PorcParticipUte;

                var lstFormulariosGuardados = await _TramiteBL.GetFormularioGuardadoAsync(tramite.IdTramite);
                int NroNotificacion = await _TramiteEvaluacionBL.GetNumeroNotificacionActualAsync(tramite.IdTramite);
                var lstFormulariosEvaluados = await _TramiteBL.GetFormulariosEvaluadosAsync(tramite.IdTramite, NroNotificacion);
                int NroFormulario = 0;
                string UrlFormulario = string.Empty;

                lstFormularios.Add(new ItemGrillaVisorTramiteDTO()
                {
                    NroFormulario = NroFormulario,
                    Guardado = lstFormulariosGuardados[NroFormulario],
                    IdentificadorUnico = tramite.IdentificadorUnico,
                    DescripcionFormulario = "Boleta de Pago y Cumplimiento Fiscal",
                    IdTramite = tramite.IdTramite,
                    Url = $"/Tramites/Pago/{Accion}/{tramite.IdentificadorUnico}",
                    IdEstadoEvaluacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.IdEstadoEvaluacion,
                    NombreEstadoEvaluacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.NombreEstadoEvaluacion,
                    MensajeNotificacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.MensajeNotificacion,
                    PermiteEditar =tramite.PermiteEditarEmpresa && tramite.PermiteEditarFormulario(Constants.Formularios.BoletaPago)
                });

                if (tramite.IdGrupoTramite == Constants.GruposDeTramite.RegistroLicitadores)
                    UrlFormulario = $"/Tramites/FormEspecialidades/{Accion}/{tramite.IdentificadorUnico}";
                else
                    UrlFormulario = $"/Tramites/RECO/Especialidades/{Accion}/{tramite.IdentificadorUnico}";

                NroFormulario = 1;
                lstFormularios.Add(new ItemGrillaVisorTramiteDTO()
                {
                    NroFormulario = NroFormulario,
                    Guardado = lstFormulariosGuardados[NroFormulario],
                    IdentificadorUnico = tramite.IdentificadorUnico,
                    DescripcionFormulario = "Formulario 1: Especialidades",
                    IdTramite = tramite.IdTramite,
                    Url = UrlFormulario,
                    IdEstadoEvaluacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.IdEstadoEvaluacion,
                    NombreEstadoEvaluacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.NombreEstadoEvaluacion,
                    MensajeNotificacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.MensajeNotificacion,
                    PermiteEditar = tramite.PermiteEditarEmpresa && tramite.PermiteEditarFormulario(Constants.Formularios.Especialidades)
                });


                NroFormulario = 2;
                if (tramite.IdGrupoTramite == Constants.GruposDeTramite.RegistroLicitadores)
                    UrlFormulario = $"/Tramites/DocumentacionObligatoria/{Accion}/{tramite.IdentificadorUnico}";
                else
                    UrlFormulario = $"/Tramites/DocumentacionObligatoriaConsultores/{Accion}/{tramite.IdentificadorUnico}";

                lstFormularios.Add(new ItemGrillaVisorTramiteDTO()
                {
                    NroFormulario = NroFormulario,
                    Guardado = lstFormulariosGuardados[NroFormulario],
                    IdentificadorUnico = tramite.IdentificadorUnico,
                    DescripcionFormulario = "Formulario 2-3-4 y 5: Documentación Complementaria y Obligatoria y Datos fiscales y sociales de la Empresa",
                    IdTramite = tramite.IdTramite,
                    Url = UrlFormulario,
                    IdEstadoEvaluacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.IdEstadoEvaluacion,
                    NombreEstadoEvaluacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.NombreEstadoEvaluacion,
                    MensajeNotificacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.MensajeNotificacion,
                    PermiteEditar = tramite.PermiteEditarEmpresa && tramite.PermiteEditarFormulario(Constants.Formularios.InformacionEmpresa)
                });

                NroFormulario = 6;
                lstFormularios.Add(new ItemGrillaVisorTramiteDTO()
                {
                    NroFormulario = NroFormulario,
                    Guardado = lstFormulariosGuardados[NroFormulario],
                    IdentificadorUnico = tramite.IdentificadorUnico,
                    DescripcionFormulario = "Formulario 6: Balance y Estado de Situación Patrimonial",
                    IdTramite = tramite.IdTramite,
                    Url = $"/Tramites/BalanceGeneral/{Accion}/{tramite.IdentificadorUnico}",
                    IdEstadoEvaluacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.IdEstadoEvaluacion,
                    NombreEstadoEvaluacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.NombreEstadoEvaluacion,
                    MensajeNotificacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.MensajeNotificacion,
                    PermiteEditar = tramite.PermiteEditarEmpresa && tramite.PermiteEditarFormulario(Constants.Formularios.BalanceGeneral)
                });

                if (tramite.IdGrupoTramite == Constants.GruposDeTramite.RegistroLicitadores)
                {
                    NroFormulario = 7;
                    lstFormularios.Add(new ItemGrillaVisorTramiteDTO()
                    {
                        NroFormulario = NroFormulario,
                        Guardado = lstFormulariosGuardados[NroFormulario],
                        IdentificadorUnico = tramite.IdentificadorUnico,
                        DescripcionFormulario = "Formulario 7: Equipos, Propiedad de la Empresa",
                        IdTramite = tramite.IdTramite,
                        Url = $"/Tramites/Equipos/{Accion}/{tramite.IdentificadorUnico}",
                        IdEstadoEvaluacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.IdEstadoEvaluacion,
                        NombreEstadoEvaluacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.NombreEstadoEvaluacion,
                        MensajeNotificacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.MensajeNotificacion,
                        PermiteEditar = tramite.PermiteEditarEmpresa && tramite.PermiteEditarFormulario(Constants.Formularios.Equipos)
                    });

                    NroFormulario = 8;
                    lstFormularios.Add(new ItemGrillaVisorTramiteDTO()
                    {
                        NroFormulario = NroFormulario,
                        Guardado = lstFormulariosGuardados[NroFormulario],
                        IdentificadorUnico = tramite.IdentificadorUnico,
                        DescripcionFormulario = "Formulario 8: Inmuebles, Propiedad de la Empresa",
                        IdTramite = tramite.IdTramite,
                        Url = $"/Tramites/BienesRaices/{Accion}/{tramite.IdentificadorUnico}",
                        IdEstadoEvaluacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.IdEstadoEvaluacion,
                        NombreEstadoEvaluacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.NombreEstadoEvaluacion,
                        MensajeNotificacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.MensajeNotificacion,
                        PermiteEditar = tramite.PermiteEditarEmpresa && tramite.PermiteEditarFormulario(Constants.Formularios.BienesRaices)
                    });
                }

                NroFormulario = 9;
                lstFormularios.Add(new ItemGrillaVisorTramiteDTO()
                {
                    NroFormulario = NroFormulario,
                    Guardado = lstFormulariosGuardados[NroFormulario],
                    IdentificadorUnico = tramite.IdentificadorUnico,
                    DescripcionFormulario = "Formulario 9: Representantes Técnicos",
                    IdTramite = tramite.IdTramite,
                    Url = $"/Tramites/RepresentantesTecnicos/{Accion}/{tramite.IdentificadorUnico}",
                    IdEstadoEvaluacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.IdEstadoEvaluacion,
                    NombreEstadoEvaluacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.NombreEstadoEvaluacion,
                    MensajeNotificacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.MensajeNotificacion,
                    PermiteEditar = tramite.PermiteEditarEmpresa && tramite.PermiteEditarFormulario(Constants.Formularios.RepresentantesTecnicos)
                });

                if (tramite.IdGrupoTramite == Constants.GruposDeTramite.RegistroLicitadores)
                {
                    NroFormulario = 10;
                    lstFormularios.Add(new ItemGrillaVisorTramiteDTO()
                    {
                        NroFormulario = NroFormulario,
                        Guardado = lstFormulariosGuardados[NroFormulario],
                        IdentificadorUnico = tramite.IdentificadorUnico,
                        DescripcionFormulario = "Formulario 10: Planilla para determinar Capacidad Técnica",
                        IdTramite = tramite.IdTramite,
                        Url = $"/Tramites/Obras/{Accion}/{tramite.IdentificadorUnico}",
                        IdEstadoEvaluacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.IdEstadoEvaluacion,
                        NombreEstadoEvaluacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.NombreEstadoEvaluacion,
                        MensajeNotificacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.MensajeNotificacion,
                        PermiteEditar = tramite.PermiteEditarEmpresa && tramite.PermiteEditarFormulario(Constants.Formularios.Obras)
                    });
                }

                NroFormulario = 11;
                lstFormularios.Add(new ItemGrillaVisorTramiteDTO()
                {
                    NroFormulario = NroFormulario,
                    Guardado = lstFormulariosGuardados[NroFormulario],
                    IdentificadorUnico = tramite.IdentificadorUnico,
                    DescripcionFormulario = "Formulario 11: Planilla de Antecedentes de Producción",
                    IdTramite = tramite.IdTramite,
                    Url = $"/Tramites/Antecedentes/{Accion}/{tramite.IdentificadorUnico}",
                    IdEstadoEvaluacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.IdEstadoEvaluacion,
                    NombreEstadoEvaluacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.NombreEstadoEvaluacion,
                    MensajeNotificacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.MensajeNotificacion,
                    PermiteEditar = tramite.PermiteEditarEmpresa && tramite.PermiteEditarFormulario(Constants.Formularios.AntecedentesDeProduccion)
                });

                if (tramite.IdGrupoTramite == Constants.GruposDeTramite.RegistroLicitadores)
                {
                    NroFormulario = 12;
                    lstFormularios.Add(new ItemGrillaVisorTramiteDTO()
                    {
                        NroFormulario = NroFormulario,
                        Guardado = lstFormulariosGuardados[NroFormulario],
                        IdentificadorUnico = tramite.IdentificadorUnico,
                        DescripcionFormulario = "Formulario 12: Declaración Jurada Obras en Ejecución contratadas y adjudicadas",
                        IdTramite = tramite.IdTramite,
                        Url = $"/Tramites/ObrasEjecucion/{Accion}/{tramite.IdentificadorUnico}",
                        IdEstadoEvaluacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.IdEstadoEvaluacion,
                        NombreEstadoEvaluacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.NombreEstadoEvaluacion,
                        MensajeNotificacion = lstFormulariosEvaluados.FirstOrDefault(x => x.NroFormulario == NroFormulario)?.MensajeNotificacion,
                        PermiteEditar = tramite.PermiteEditarEmpresa && tramite.PermiteEditarFormulario(Constants.Formularios.ObrasEnEjecucion)
                    });

                }

                //si se está editando el trámite por la empresa
                if ((tramite.IdEstado == Constants.TramitesEstados.EditarInformacion
                    || tramite.IdEstado == Constants.TramitesEstados.Observado))
                {
                    
                    btnPresentarTramiteVisible = tramite.PermiteEditarEmpresa && !lstFormularios.Exists(x => !x.Guardado); // se busca algun formulario no guardado, si todos estan guardados se habilita la presentación del trámite
                    btnAnularTramiteVisible = tramite.PermiteEditarEmpresa;
                    lstFormularios.ForEach(f => f.PorcentajeAvance = (f.Guardado ? 50 : 0));
                }
                else
                {
                    btnAnularTramiteVisible = false;
                    btnPresentarTramiteVisible = false;
                    lstFormularios.ForEach(f => f.PorcentajeAvance = 100);
                }

                // Si se está evaluando el tramite por el evaluador o administrador
                if (tramite.IdEstado == Constants.TramitesEstados.EnEvaluacion)
                {
                    // Es visible solo si se calificaron todos los formularios o si alguno tiene el estado de Notificar.
                    btnAprobarNotificarTramiteVisible = !lstFormularios.Exists(x => !x.IdEstadoEvaluacion.HasValue) ||
                        lstFormularios.Exists(x => x.IdEstadoEvaluacion == (int)Constants.EstadosEvaluacion.Notificar);

                    if (btnAprobarNotificarTramiteVisible)
                    {
                        btnAprobarNotificarTramiteText = (lstFormularios.Exists(x => x.IdEstadoEvaluacion == (int)Constants.EstadosEvaluacion.Notificar) ? "Notificar trámite" : "Aprobar Tramite");
                        btnAprobarNotificarTramiteClass = (lstFormularios.Exists(x => x.IdEstadoEvaluacion == (int)Constants.EstadosEvaluacion.Notificar) ? "btn btn-warning" : "btn btn-success");
                    }
                }
                else
                {
                    btnAprobarNotificarTramiteVisible = false;
                    btnAprobarNotificarTramiteText = "";
                    btnAprobarNotificarTramiteClass = "";
                }


                dlFormularios?.Reload();
            }
            isBusyInitialLoading = false;
        }
        protected void OnEditarClick(ItemGrillaVisorTramiteDTO item)
        {
            navigationManager.NavigateTo(item.Url, true);
        }
        protected async Task PresentarClick()
        {
            try
            {
                var lstMensajes = await _TramiteBL.ValidacionesPresentarTramite(tramite.IdTramite);

                if (lstMensajes.Count > 0)
                {
                    await DS.OpenAsync<ModalValidaciones>("Validaciones",
                                           new Dictionary<string, object>() { { "Mensajes", lstMensajes }, { "Title", "Para poder presentar el trámite debe corregir los siguientes puntos:" } },
                                           new DialogOptions() { Width = "auto", Height = "auto", Resizable = false });
                }
                else
                {

                    var respuesta = await DS.OpenAsync<ModalPresentarTramite>("Presentar la información", null,
                                        new DialogOptions() { Width = "auto", Height = "auto", Resizable = true });

                    if (respuesta ?? false)
                    {
                        await _TramiteBL.ActualizarEstadoTramiteAsync(tramite.IdTramite, Constants.TramitesEstados.EnEvaluacion);
                        notificationService.Notify(NotificationSeverity.Success, "Aviso", "El trámite se ha presentado exitosamente", StaticClass.Constants.NotifDuration.Normal);
                        navigationManager.NavigateTo(navigationManager.Uri, true);
                    }
                }

            }
            catch (Exception ex)
            {
                notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
            }
        }
        protected async Task AnularClick()
        {
            try
            {

                var respuesta = await DS.Confirm("¿Estás seguro de anular el trámite?",
                                                "Confirmar anulación",
                                            new ConfirmOptions() { OkButtonText = "Sí", CancelButtonText = "No",  Height = "auto", Resizable = true });

                if (respuesta ?? false)
                {
                    await _TramiteBL.ActualizarEstadoTramiteAsync(tramite.IdTramite, Constants.TramitesEstados.Anulado);
                    notificationService.Notify(NotificationSeverity.Success, "Aviso", "El trámite se anuló correctamente", StaticClass.Constants.NotifDuration.Normal);
                    navigationManager.NavigateTo(navigationManager.Uri, true);
                }

            }
            catch (Exception ex)
            {
                notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
            }
        }
        private async Task VerNotificacionClick(ItemGrillaVisorTramiteDTO formulario)
        {
            await DS.OpenAsync<ModalNotificaciones>("Notificación",
                        new Dictionary<string, object>() { { "Mensaje", formulario.MensajeNotificacion }, { "Title", $"Notificación:" } },
                                           new DialogOptions() { Width = "60%", Height = "auto", Resizable = false });
        }
        protected async Task AprobarNotificarClick()
        {
            if (isBusyAprobado)
                return;

            isBusyAprobado = true;
            try
            {
                bool isNotificar = lstFormularios.Exists(x => x.IdEstadoEvaluacion == (int)Constants.EstadosEvaluacion.Notificar);
                if (isNotificar)
                {
                    await _TramiteBL.ActualizarEstadoTramiteAsync(tramite.IdTramite, Constants.TramitesEstados.Observado);
                    await _notificacionesBL.Notificar(tramite.IdTramite, "Observación de trámite", $"Revise las notificaciones realizadas en el trámite {tramite.IdTramite}");
                }
                else
                {
                    await _TramiteBL.ActualizarEstadoTramiteAsync(tramite.IdTramite, Constants.TramitesEstados.ElevadoParaLaFirma);
                }

                navigationManager.NavigateTo("/BandejaTramites");

            }
            catch (Exception ex)
            {
                notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
            }
            finally
            {
                isBusyAprobado = false;
            }

        }
        protected async Task RechazarClick()
        {
            if (isBusyAprobado)
                return;

            isBusyAprobado = true;
            try
            {
                var NotificacionModel = new NotificacionModel()
                {
                    IdTramite = tramite.IdTramite,
                    Titulo = "Trámite Rechazado"
                };
                NotificacionModel = await DS.OpenAsync<ModalNotificacion>("Notificación al usuario",
                                      new Dictionary<string, object>() { { "Model", NotificacionModel } },
                                      new DialogOptions() { Width = "60%", Height = "auto", Resizable = true });

                if (NotificacionModel != null)
                {
                    await _TramiteBL.ActualizarEstadoTramiteAsync(tramite.IdTramite, Constants.TramitesEstados.Rechazado);
                    await _notificacionesBL.Notificar(tramite.IdTramite, NotificacionModel.Titulo, NotificacionModel.Mensaje);
                    navigationManager.NavigateTo(navigationManager.Uri,true);
                }

            }
            catch (Exception ex)
            {
                notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
            }
            finally
            {
                isBusyAprobado = false;
            }

        }

        #region Generar Certificado
        protected async Task ProbarCertificadoClick(int TipoCertificado)
        {
            string wwwrootPath = WebHostEnvironment.WebRootPath;
            byte[] pdf = new byte[0];
            tramite = await _TramiteBL.GetTramiteByGuidAsync(IdentificadorUnico);
            string fileName = "";
            switch (TipoCertificado)
            {
                case Constants.TiposDeTramite.Reli_Inscripcion:
                    pdf = await _reportesBL.GetPdfReliInscripcion(tramite.IdTramite, wwwrootPath);
                    fileName = $"Certificado-Inscripcion-RELI-{tramite.IdTramite}.pdf";
                    break;
                case Constants.TiposDeTramite.Reli_Licitar:
                    pdf = await _reportesBL.GetPdfReliLicitar(tramite.IdTramite, wwwrootPath);
                    fileName = $"Certificado-Licitacion-RELI-{tramite.IdTramite}.pdf";
                    break;
                case Constants.TiposDeTramite.Reli_ActualizacionCapacidadTecnica:
                case Constants.TiposDeTramite.Reli_ActualizacionSoloTecnicos:
                case Constants.TiposDeTramite.Reli_ActualizacionCompleta:
                    pdf = await _reportesBL.GetPdfReliActualizacion(tramite.IdTramite, wwwrootPath);
                    fileName = $"Certificado-Actualizarcion-RELI-{tramite.IdTramite}.pdf";
                    break;

                case Constants.TiposDeTramite.Reco_Inscripcion:
                    pdf = await _reportesBL.GetPdfRecoInscripcion(tramite.IdTramite, wwwrootPath);
                    fileName = $"Certificado-Inscripcion-RECO-{tramite.IdTramite}.pdf";
                    break;
            }
            
            string contentType = "application/octet-stream";
            await JSRuntime.InvokeVoidAsync("BlazorDownloadFile", fileName, contentType, pdf);

        }
        protected async Task PrevisualizarDocumentoClick()
        {
            string userid = await _usuarioBL.GetCurrentUserid();
            string wwwrootPath = WebHostEnvironment.WebRootPath;
            byte[] pdf = new byte[0];

            tramite = await _TramiteBL.GetTramiteByGuidAsync(IdentificadorUnico);
            string fileName = "";

            if (tramite.IdTipoTramite == Constants.TiposDeTramite.Reli_Inscripcion)
            {
                pdf = await _reportesBL.GetPdfReliInscripcion(tramite.IdTramite, wwwrootPath);
                fileName = $"Certificado-Inscripcion-RELI-{tramite.IdTramite}.pdf";
            }
            else if (tramite.IdTipoTramite == Constants.TiposDeTramite.Reli_Licitar)
            {
                pdf = await _reportesBL.GetPdfReliLicitar(tramite.IdTramite, wwwrootPath);
                fileName = $"Certificado-Licitacion-RELI-{tramite.IdTramite}.pdf";
            }
            else if (tramite.IdTipoTramite == Constants.TiposDeTramite.Reli_ActualizacionSoloTecnicos ||
                tramite.IdTipoTramite == Constants.TiposDeTramite.Reli_ActualizacionCompleta ||
                tramite.IdTipoTramite == Constants.TiposDeTramite.Reli_ActualizacionCapacidadTecnica)
            {
                pdf = await _reportesBL.GetPdfReliInscripcion(tramite.IdTramite, wwwrootPath);
                fileName = $"Certificado-Renovacion-RELI-{tramite.IdTramite}.pdf";
            }
            if (tramite.IdTipoTramite == Constants.TiposDeTramite.Reco_Inscripcion)
            {
                pdf = await _reportesBL.GetPdfRecoInscripcion(tramite.IdTramite, wwwrootPath);
                fileName = $"Certificado-Inscripcion-RECO-{tramite.IdTramite}.pdf";
            }

            string contentType = "application/pdf";
            await JSRuntime.InvokeVoidAsync("BlazorDownloadFile", fileName, contentType, pdf);

        }

        protected async Task FirmarDocumentoClick()
        {
            _ = BusyDialog("Firmando el documento...");
            
            try
            {
                string userid = await _usuarioBL.GetCurrentUserid();
                string wwwrootPath = WebHostEnvironment.WebRootPath;
                byte[] pdf = new byte[0];

                tramite = await _TramiteBL.GetTramiteByGuidAsync(IdentificadorUnico);
                string fileName = "";

                if (tramite.IdEstado == Constants.TramitesEstados.ElevadoParaLaFirma)
                {


                    if (tramite.IdTipoTramite == Constants.TiposDeTramite.Reli_Inscripcion)
                    {
                        pdf = await _reportesBL.GetPdfReliInscripcion(tramite.IdTramite, wwwrootPath, false);
                        fileName = $"Certificado-Inscripcion-RELI-{tramite.IdTramite}.pdf";
                    }
                    else if (tramite.IdTipoTramite == Constants.TiposDeTramite.Reli_Licitar)
                    {
                        pdf = await _reportesBL.GetPdfReliLicitar(tramite.IdTramite, wwwrootPath, false);
                        fileName = $"Certificado-Licitacion-RELI-{tramite.IdTramite}.pdf";
                    }
                    else if (tramite.IdTipoTramite == Constants.TiposDeTramite.Reli_ActualizacionSoloTecnicos ||
                        tramite.IdTipoTramite == Constants.TiposDeTramite.Reli_ActualizacionCompleta ||
                        tramite.IdTipoTramite == Constants.TiposDeTramite.Reli_ActualizacionCapacidadTecnica)
                    {
                        pdf = await _reportesBL.GetPdfReliInscripcion(tramite.IdTramite, wwwrootPath, false);
                        fileName = $"Certificado-Renovacion-RELI-{tramite.IdTramite}.pdf";
                    }
                    if (tramite.IdTipoTramite == Constants.TiposDeTramite.Reco_Inscripcion)
                    {
                        pdf = await _reportesBL.GetPdfRecoInscripcion(tramite.IdTramite, wwwrootPath, false);
                        fileName = $"Certificado-Inscripcion-RECO-{tramite.IdTramite}.pdf";
                    }

                    if (string.IsNullOrWhiteSpace(tramite.numeroGEDO))
                    {
                        tramite.numeroGEDO = await GenerarGEDO(pdf, tramite.IdTramite);
                        await _TramiteBL.ActualizarTramiteAsync(tramite);
                    }

                    var gedo = await _gedoBL.ObtenerPdfDocumento(tramite.numeroGEDO);
                    //antiguo SOAP
                    //var gedo = await DescargarGEDO(tramite.numeroGEDO);
                    
                    FileDTO fileGEDO = new FileDTO()
                    {
                        ContentFile = gedo,
                        ContentType = "application/pdf",
                        Extension = System.IO.Path.GetExtension(fileName),
                        FileName = fileName,
                        Size = gedo.LongLength,
                        Rowid = Guid.NewGuid(),
                        CreateDate = DateTime.Now,
                        CreateUser = userid

                    };
                    fileGEDO = await _filesBL.AddFileAsync(fileGEDO);

                    if (fileGEDO.IdFile > 0)
                    {
                        tramite.IdFileGEDO = fileGEDO.IdFile;
                        await _TramiteBL.AprobarTramiteAsync(tramite);
                        tramite = await _TramiteBL.GetTramiteByGuidAsync(IdentificadorUnico);
                    }

                    string contentType = "application/octet-stream";
                    await JSRuntime.InvokeVoidAsync("BlazorDownloadFile", fileName, contentType, gedo);
                }
                else
                    throw new ArgumentException("El trámite no se envió a firmar porque el estado no es elevado a la firma.");
            }
            catch (Exception ex)
            {
                var error = Functions.GetErrorMessage(ex);
                await DS.OpenAsync<ModalNotificaciones>("Error",
                        new Dictionary<string, object>() { { "Mensaje", error }, { "Title", string.Empty } },
                                new DialogOptions() { Width = "60%", Height = "auto", Resizable = false });
            }
            finally
            {
                DS.Close();
            }

        }
        protected async Task ObtenerActualizacionClick()
        {
            isBusyActualizacion = true;
            try
            {
                string wwwrootPath = WebHostEnvironment.WebRootPath;
                byte[] pdf = new byte[0];
                pdf = await _reportesBL.GetPdfReliActualizacion(tramite.IdTramite, wwwrootPath,false);

                string fileName = $"Certificado-Actualizacion-{tramite.IdTramite}.pdf";
                string contentType = "application/octet-stream";
                await JSRuntime.InvokeVoidAsync("BlazorDownloadFile", fileName, contentType, pdf);
            }
            catch (Exception ex) 
            {
                notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
            }
            finally
            {
                isBusyActualizacion = false;
            }
        }

        private async Task<string> GenerarGEDO(byte[] pdf, int IdTramite)
        {
            string numeroGEDO;
            string urlSADEHost = await _paramBL.GetParametroCharAsync("SADE.Host");
            string sistemaOrigen = await _paramBL.GetParametroCharAsync("SADE.SistemaOrigen");
            string usernameSADE = await _paramBL.GetParametroCharAsync("SADE.UsuarioDirector");
            string acronimoSADE = await _paramBL.GetParametroCharAsync("SADE.Acronimo.CertInscripcion");

            GenerarDocumentoCnn.ExternalGenerarDocumentoServiceClient client = new GenerarDocumentoCnn.ExternalGenerarDocumentoServiceClient();
            client.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri(new Uri(urlSADEHost), "GEDOServices/generarDocumentoGEDO"));
            GenerarDocumentoCnn.requestExternalGenerarDocumento req = new GenerarDocumentoCnn.requestExternalGenerarDocumento()
            {
                acronimoTipoDocumento = acronimoSADE,
                data = pdf,
                sistemaOrigen = sistemaOrigen,
                referencia = $"Trámite de Inscripcion Nro. {IdTramite}",
                tipoArchivo = "pdf",
                usuario = usernameSADE
            };
            var response = await client.generarDocumentoGEDOAsync(req);
            numeroGEDO = response.@return.numero;
            client.Close();

            return numeroGEDO;
        }

        private async Task<byte[]> DescargarGEDO(string numeroGEDO)
        {
            byte[] result;
            string urlSADEHost = await _paramBL.GetParametroCharAsync("SADE.Host");
            string sistemaOrigen = await _paramBL.GetParametroCharAsync("SADE.SistemaOrigen");
            string usernameSADE = await _paramBL.GetParametroCharAsync("SADE.UsuarioDirector");

            ConsultaDocumentoCnn.ExternalConsultaDocumentoMuleServiceClient client = new ConsultaDocumentoCnn.ExternalConsultaDocumentoMuleServiceClient();
            client.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri(new Uri(urlSADEHost), "GEDOServices/consultaDocumento"));

            ConsultaDocumentoCnn.requestExternalConsultaDocumento req = new ConsultaDocumentoCnn.requestExternalConsultaDocumento()
            {
                numeroDocumento = numeroGEDO,
                sistemaOrigen = sistemaOrigen,
                usuarioConsulta = usernameSADE
            };
            var response = await client.consultarDocumentoPdfAsync(req);
            result = response.@return;
            await client.CloseAsync();

            return result;
        }



      
        // Busy dialog from string
        private async Task BusyDialog(string message)
        {
            await DS.OpenAsync("", ds =>
            {
                RenderFragment content = b =>
                {
                    b.OpenElement(0, "div");
                    b.AddAttribute(1, "class", "d-flex flex-row justify-content-end align-items-center");

                    b.OpenElement(2, "div");
                    b.AddAttribute(3, "class", "spinner-border text-primary");
                    b.AddAttribute(4, "role", "status");
                    b.CloseElement();

                    b.OpenElement(5, "label");
                    b.AddAttribute(6, "class", "ms-1");
                    b.AddContent(7, message);

                    b.CloseElement();
                    b.CloseElement();
                };
                return content;
            }, new DialogOptions() { ShowTitle = false, Style = "min-height:auto;min-width:auto;width:auto", CloseDialogOnEsc = false });
        }
        #endregion

        private async Task OnDownloadFileAsync(int fileId)
        {
            
            var archivoDTO = await _filesBL.GetFileAsync(fileId);
            byte[] file = archivoDTO.ContentFile;
            string fileName = archivoDTO.FileName;
            string contentType = archivoDTO.ContentType;

            await JSRuntime.InvokeVoidAsync("BlazorDownloadFile", fileName, contentType, file);
            
        }
        private async Task ReAsignarClick()
        {

            var respuesta = await DS.OpenAsync<ModalAsignacionEvaluador>("Reasignar Evaluador",
                                new Dictionary<string, object>() { { "IdTramite", tramite.IdTramite } },
                                new DialogOptions() { Width = "600px", Height = "auto", Resizable = false });

            if (respuesta ?? false)
            {
                navigationManager.NavigateTo(navigationManager.Uri, true);
            }
        }

        //OnClickGuardarUte
        protected async Task OnClickGuardarUte(EditContext Ed)
        {
            if (isBusyGuardarUte)
                return;

            
            isBusyGuardarUte = true;
            if (Ed != null && Ed.Validate())
            {

                try
                {
                    await _TramiteBL.ActualizarEnUte(tramite.IdTramite,UteModel.SePresentaEnUte,UteModel.PorcParticipUte);
                    isBusyGuardarUte = false;
                }
                catch (Exception ex)
                {
                    notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
                    isBusyGuardarUte = false;
                }
            }
            else
                isBusyGuardarUte = false;
        }
    }
}
