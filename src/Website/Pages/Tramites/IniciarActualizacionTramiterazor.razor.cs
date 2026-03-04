using Business.Interfaces;
using CsvHelper.Configuration.Attributes;
using DataTransferObject.BLs;
using Microsoft.AspNetCore.Components;
using Radzen;
using StaticClass;
using Website.Pages.Tramites.Formulario.Form2y3;

namespace Website.Pages.Tramites
{
    public partial class IniciarActualizacionTramiterazor : ComponentBase
    {
        // Parámetro recibido desde la vista para identificar el tipo de trámite.
        [Parameter] public int IdTipoTramite { get; set; }

        // Servicios inyectados para navegación, diálogos, notificaciones y lógica de negocio.
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private DialogService dialogService { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private ITramitesBL _tramitesBL { get; set; } = null!;
        [Inject] private IUsuariosBL _usuarioBL { get; set; } = null!;
        [Inject] private ICombosBL _combosBL { get; set; } = null!;

        // Propiedades privadas para gestionar el estado del componente.
        private bool InscripcionNoAprobada = false;
        private string mensajeValidacionActualizacion = string.Empty;
        private string TipoDeTramite = null!;

        // Propiedades para almacenar información de trámites.
        private TramitesDTO? tramitePendiente { get; set; } = null!;

        /// <summary>
        /// Método que se ejecuta al inicializar el componente.
        /// Realiza validaciones, obtiene datos iniciales y genera trámites si es necesario.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            // Verifica si la inscripción está aprobada.
            InscripcionNoAprobada = !await TieneInscripcionAprobada();

            // Obtiene el trámite pendiente, si existe.
            tramitePendiente = await GetTramitePendiente();

            // Obtiene la descripción del tipo de trámite.
            var lstTiposDeTramite = await _combosBL.GetTiposDeTramiteAsync();
            TipoDeTramite = lstTiposDeTramite.FirstOrDefault(x => x.Id == IdTipoTramite)?.Descripcion ?? string.Empty;

            // Si la inscripción está aprobada y no hay trámites pendientes, realiza validaciones adicionales.
            if (!InscripcionNoAprobada && tramitePendiente == null)
            {
                int IdEmpresa = 0;
                int.TryParse(await _usuarioBL.GetCurrentIdEmpresa(), out IdEmpresa);

                try
                {
                    if (IdTipoTramite == Constants.TiposDeTramite.Reli_ActualizacionCompleta)
                    {
                        // Valida las condiciones para la actualización anual.
                        await _tramitesBL.ThrowIfValidacionesActualizacionAnual(IdTipoTramite, IdEmpresa);
                    }
                    else if (IdTipoTramite == Constants.TiposDeTramite.Reli_ActualizacionCapacidadTecnica)
                    {
                        // Valida las condiciones para la actualización Capacidad tecnica, responsables tecnicos y Equipos.
                        await _tramitesBL.ThrowIfValidacionesActualizacionCapacidadRespEquipos(IdTipoTramite, IdEmpresa);
                    }
                }
                catch (Exception ex)
                {
                    mensajeValidacionActualizacion = ex.Message;
                }

                // Si no hay errores de validación, genera el trámite.
                if (string.IsNullOrWhiteSpace(mensajeValidacionActualizacion))
                    await GenerarTramite(IdTipoTramite);
            }

            await base.OnInitializedAsync();
        }

        /// <summary>
        /// Genera un nuevo trámite basado en el tipo de trámite especificado.
        /// </summary>
        /// <param name="IdTipoTramite">Identificador del tipo de trámite.</param>
        public async Task GenerarTramite(int IdTipoTramite)
        {
            string mensajeConfirmacion = string.Empty;

            // Define el mensaje de confirmación según el tipo de trámite.
            if (IdTipoTramite == Constants.TiposDeTramite.Reli_ActualizacionSoloTecnicos)
                mensajeConfirmacion = "¿Está seguro de iniciar un nuevo trámite de actualización de los datos (SOLO RESPONSABLES TECNICOS) del último trámite?";
            else if (IdTipoTramite == Constants.TiposDeTramite.Reli_ActualizacionCapacidadTecnica)
                mensajeConfirmacion = "¿Está seguro de iniciar un nuevo trámite de actualización de los datos (CAPACIDAD TECNICA, EQUIPOS y/o RESPONSABLES TECNICOS) del último trámite?";
            else
                mensajeConfirmacion = "¿Está seguro de iniciar un nuevo trámite de actualización de los datos del último trámite (ACTUALIZACION COMPLETA)?";

            // Muestra un cuadro de diálogo de confirmación.
            var respuesta = await dialogService.Confirm(mensajeConfirmacion,
                                 "Quiero Actualizar Información",
                                 new ConfirmOptions() { OkButtonText = "Sí", CancelButtonText = "No" });

            if (respuesta.HasValue && respuesta.Value) // Si el usuario confirma.
            {
                int IdEmpresa = 0;
                int.TryParse(await _usuarioBL.GetCurrentIdEmpresa(), out IdEmpresa);

                // Crea el trámite y redirige a la página de visualización.
                var tramite = await _tramitesBL.CrearTramiteActualizacionAsync(IdTipoTramite, IdEmpresa);
                navigationManager.NavigateTo($"/Tramites/Visualizar/{tramite.IdentificadorUnico}");
            }
            else
            {
                // Redirige a la página principal si el usuario cancela.
                navigationManager.NavigateTo($"/1");
            }
        }

        /// <summary>
        /// Cancela la operación y redirige a la página principal.
        /// </summary>
        protected void CancelarClick()
        {
            navigationManager.NavigateTo($"/", true);
        }

        /// <summary>
        /// Verifica si la inscripción de la empresa está aprobada.
        /// </summary>
        /// <returns>True si la inscripción está aprobada, de lo contrario False.</returns>
        private async Task<bool> TieneInscripcionAprobada()
        {
            int IdEmpresa = 0;
            int.TryParse(await _usuarioBL.GetCurrentIdEmpresa(), out IdEmpresa);
            var ultimoTramite = await _tramitesBL.GetUltimoTramiteAprobadoAsync(Constants.TiposDeTramite.Reli_Inscripcion, IdEmpresa);

            return (ultimoTramite != null);
        }

        /// <summary>
        /// Obtiene el trámite pendiente de la empresa, si existe.
        /// </summary>
        /// <returns>El trámite pendiente o null si no hay trámites pendientes.</returns>
        private async Task<TramitesDTO?> GetTramitePendiente()
        {
            TramitesDTO? result = null;
            int IdEmpresa = 0;
            int.TryParse(await _usuarioBL.GetCurrentIdEmpresa(), out IdEmpresa);

            var tramites = await _tramitesBL.GetTramitesByEmpresaAsync(IdEmpresa);

            result = tramites.FirstOrDefault(x =>
                                    (x.IdTipoTramite == Constants.TiposDeTramite.Reli_ActualizacionSoloTecnicos
                                    || x.IdTipoTramite == Constants.TiposDeTramite.Reli_ActualizacionCapacidadTecnica
                                    || x.IdTipoTramite == Constants.TiposDeTramite.Reli_ActualizacionCompleta)
                                    && x.IdEstado != Constants.TramitesEstados.Aprobado
                                    && x.IdEstado != Constants.TramitesEstados.Rechazado
                                    && x.IdEstado != Constants.TramitesEstados.Anulado);

            return result;
        }
    }
    
}
