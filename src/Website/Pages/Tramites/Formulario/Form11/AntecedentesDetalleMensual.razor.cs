using AutoMapper;
using Business.Extensions;
using DataAccess.Entities;
using DataTransferObject.BLs;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Radzen;
using Website.Models.Formulario;
using Business.Interfaces;
using Business;
using Humanizer;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Components.Forms;
using StaticClass;
using BlazorPro.Spinkit;
using DocumentFormat.OpenXml.Drawing.Charts;
using Radzen.Blazor;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using Microsoft.JSInterop;
using Website.Pages.Shared.Components;

namespace Website.Pages.Tramites.Formulario.Form11
{
    public partial class AntecedentesDetalleMensual: ComponentBase
    {
        [Parameter] public string IdentificadorUnico { get; set; } = null!;
        [Parameter] public int IdTramiteEspecialidad { get; set; } 
        [Parameter] public int IdTramiteEspecialidadSeccion { get; set; } 
        [Parameter] public string AccionFormAnterior { get; set; } = null!; //Ingresar o Editar
        [Parameter] public string Accion { get; set; } = null!; //Ingresar o Editar

        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] private DialogService dialogService { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] protected RouteHistoryService routeHistory { get; set; } = null!;
        [Inject] private ITramitesBL _TramiteBL { get; set; } = null!;
        [Inject] private IAntecedentesBl _AntecedentesBL { get; set; } = null!;
        
        [Inject] IJSRuntime JS { get; set; } = null!;
        
        private TramitesDTO tramite { get; set; } = new();
        private List<AntecedentesDeclaracionMensualModel> Model { get; set; } = new();
        private AntecedentesModel ModelAntecetente { get; set; } = new();
        private List<TramitesAntecedentesDdjjFilaDto> datos2 { get; set; } = new();
        private List<TramitesAntecedentesTotales12mesesDto> lstDetalleCalculo { get; set; } = new();
        private bool EvaluandoTramite { get; set; } = false;
        private IMapper _mapper { get; set; } = null!;
        private bool isBusyAceptar { get; set; }
        
        private RadzenDataGrid<TramitesAntecedentesDdjjFilaDto> grid2 = null!;
        private List<HeaderGrillaDdjjmensualModel> HeadersGrilla { get; set; } = new();
        private List<string> antecedentesHeaders = new();
        private string DescripcionEspecialidad { get; set; } = null!;
        private string DescripcionEspecialidadSeccion { get; set; } = null!;
        private TramitesAntecedentesResumen12MesesDto ResumenMejores12 {get;set;}= null!;
        private bool isRenderized = false;
        private bool isEditRow = false;
        private bool isRowRendered = false;
        private bool isBusyInitialLoading = true;
        
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
                cfg.CreateMap<AntecedentesDeclaracionMensualModel, TramitesAntecedentesDdjjMensualDto>().ReverseMap();
                cfg.CreateMap<AntecedentesModel, TramitesAntecedentesDto>().ReverseMap();
                
            });
            _mapper = config.CreateMapper();
            //--

            tramite = await _TramiteBL.GetTramiteByGuidAsync(IdentificadorUnico);
            tramite.PermiteEditarEmpresa = tramite.PermiteEditarFormulario(Constants.Formularios.AntecedentesDeProduccion);

            //Obtiene las Antecedentes de Obras de la especialidad
            var lstAntecedentes = await _AntecedentesBL.GetAntecedentesxEspecialidad(IdTramiteEspecialidad, IdTramiteEspecialidadSeccion);
            
            //Obtiene las especialidades y secciones del tramite para poder sacar la descripcion
            var lstEspecialidades = await _TramiteBL.GetEspecialidadesAsync(tramite.IdTramite);

            var Especialidad = lstEspecialidades.FirstOrDefault(x => x.IdTramiteEspecialidad == IdTramiteEspecialidad);
            DescripcionEspecialidad = Especialidad!.DescripcionEspecialidad;
            var EspecialidadSeccion = Especialidad.Secciones.FirstOrDefault(x => x.IdTramiteEspecialidadSeccion == IdTramiteEspecialidadSeccion);
            DescripcionEspecialidadSeccion = EspecialidadSeccion!.DescripcionSeccion;

            if (lstAntecedentes.Any())
            {

                //Arma una lista con los nombres de las obras que van a ir en las columnas de la grilla.
                HeadersGrilla = lstAntecedentes.OrderBy(o=> o.IdTramiteAntecedente).Select(s => new HeaderGrillaDdjjmensualModel
                {
                     IdTramiteAntecedente = s.IdTramiteAntecedente,
                     ColumnName = s.NombreObra
                }).ToList();
            
                //Genera el contenido de la grilla e intercala los valores ya guardados en la base de datos.
                var FechaPresentacion = (await _TramiteBL.GetFechaPresentacionAsync(tramite.IdTramite)) ?? DateTime.Now.Date;
                datos2  = await _AntecedentesBL.GetDdjjMensual5AniosAsync(FechaPresentacion, IdTramiteEspecialidad, IdTramiteEspecialidadSeccion);
                // Armo headers dinámicos a partir de la primera fila
                if(datos2.Any())
                    antecedentesHeaders = datos2[0].Antecedentes.Select(a => a.NombreObra).ToList();

                ResumenMejores12 = await _AntecedentesBL.GetResumenMejores12Meses(datos2);
                if(ResumenMejores12 != null)
                    lstDetalleCalculo = await _AntecedentesBL.GetTotalesMejores12MesesAsync(datos2, ResumenMejores12.MesInicio, ResumenMejores12.AnioInicio);
                

            }
            isBusyInitialLoading = false;
            await base.OnInitializedAsync();
        }
        private void VolverAlVisor()
        {
            navigationManager.NavigateTo($"/Tramites/{AccionFormAnterior}/{IdentificadorUnico}", true);
        }

        protected void CancelarClick()
        {
            navigationManager.NavigateTo($"/Tramites/Antecedentes/{AccionFormAnterior}/{IdentificadorUnico}", true);
        }
    
        protected async Task OnClickGuardar()
        {
            
            if (isBusyAceptar)
                return;

            isBusyAceptar = true;
            try
            {


                if (await ValidacionesAlGuardarAsync())
                {

                    // Convierte los valores de la grilla al formato del dto para luego ser guardado
                    List<TramitesAntecedentesDdjjMensualDto> lstDdjjMensual = new();
                    foreach (var dato in datos2)
                    {
                        foreach (var antecedente in dato.Antecedentes.Where(x => x.MontoMensual > 0))
                        {
                            lstDdjjMensual.Add(new TramitesAntecedentesDdjjMensualDto
                            {
                                IdTramiteAntecedente = antecedente.IdAntecedente,
                                Mes = dato.Mes,
                                Anio = dato.Anio,
                                Monto = antecedente.MontoMensual,
                            });
                        }
                    }
                    //--

                    await _AntecedentesBL.ActualizarAntecedenteDdjjMensualAsync(lstDdjjMensual);
                    isBusyAceptar = false;

                    navigationManager.NavigateTo($"/Tramites/Antecedentes/{AccionFormAnterior}/{IdentificadorUnico}", true);
                }
                isBusyAceptar = false;
            }
            catch (Exception ex)
            {
                notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
                isBusyAceptar = false;
            }

        }
        private async Task<bool> ValidacionesAlGuardarAsync()
        {
            List<string> mensajes = new List<string>();


            //Obtiene los id de antecedentes (obras) de las columnas
            var lstIdsAntecedentes = datos2.SelectMany(s => s.Antecedentes).Select(s => s.IdAntecedente).Distinct().ToList();

            var FechaPresentacion = (await _TramiteBL.GetFechaPresentacionAsync(tramite.IdTramite)) ?? DateTime.Now.Date;
            var FechaInicio5AniosAnteriores = FechaPresentacion.AddMonths(-60);
            var PeriodoInicio5AniosAnteriores = $"{FechaInicio5AniosAnteriores.Month.ToString("00")}/{FechaInicio5AniosAnteriores.Year}";

            //Valida que lo guardadon en el antecedente coincida con el monto total informado en la obra
            foreach (int IdAntecedente in lstIdsAntecedentes)
            {
                var AntecedenteInDb = await _AntecedentesBL.GetAntecedenteAsync(IdAntecedente);
                var TotalDetalleMontoEjecutado = datos2.SelectMany(s => s.Antecedentes).Where(x => x.IdAntecedente == IdAntecedente).Sum(s => s.MontoMensual);

                //Si el período de inicio de la obra es posterior a 5 años atras, se exige que el monto del detalle sea = al monto que tiene la obra como ejecutado.
                //Si l período de inicio es anterior a 5 años, solo se valida que el detalle no sea superior al monto ejecutado de la obra, pero podria ser inferior ya que desconocemos el ejecutado anteriore a los 5 años
                if (Functions.ConvertPeriodoToNumber(AntecedenteInDb.PeriodoInicio) >= Functions.ConvertPeriodoToNumber(PeriodoInicio5AniosAnteriores)
                    && AntecedenteInDb.MontoEjecutado != TotalDetalleMontoEjecutado)
                        mensajes.Add ($"El Antecedente {AntecedenteInDb.NombreObra} tiene informado como monto ejecutado {String.Format("{0:C2}", AntecedenteInDb.MontoEjecutado)}, y el monto toal informado en este detalle es de {String.Format("{0:C2}", TotalDetalleMontoEjecutado)}. Los importes deben conincidir para poder guardar.");
                else if(TotalDetalleMontoEjecutado > AntecedenteInDb.MontoEjecutado)
                    mensajes.Add($"El Antecedente {AntecedenteInDb.NombreObra} tiene informado como monto ejecutado {String.Format("{0:C2}", AntecedenteInDb.MontoEjecutado)}, y el monto toal informado en este detalle es de {String.Format("{0:C2}", TotalDetalleMontoEjecutado)}. El detalle no puede ser superior al monto ejecutado.");
            }
                

            if (mensajes.Count > 0)
            {
                await dialogService.OpenAsync<ModalValidaciones>("Validaciones",
                                       new Dictionary<string, object>() { { "Mensajes", mensajes }, { "Title", "Se han encontrado validaciones a revisar:" } },
                                       new DialogOptions() { Width = "70%", Height = "auto", Resizable = false });

            }
            return mensajes.Count == 0;
        }
        async Task EditRow2(TramitesAntecedentesDdjjFilaDto row)
        {
            if (!grid2.IsValid) return;
            isEditRow = true;
            await grid2.EditRow(row);

        }
        async Task SaveRow2(TramitesAntecedentesDdjjFilaDto row)
        {
            //Actualiza los totales de Fila de los datos aun no almacenados
            row.MontoFila = row.Antecedentes.Sum(s => s.MontoMensual);

            //--
            //Actualiza el Resumen
            ResumenMejores12 = await _AntecedentesBL.GetResumenMejores12Meses(datos2);
            if(ResumenMejores12 != null)
                lstDetalleCalculo = await _AntecedentesBL.GetTotalesMejores12MesesAsync(datos2, ResumenMejores12.MesInicio, ResumenMejores12.AnioInicio);

            await grid2.UpdateRow(row);
            isRowRendered = false;
            isEditRow = false;
        }
        void CancelEdit2(TramitesAntecedentesDdjjFilaDto row)
        {
            grid2.CancelEditRow(row);
            isRowRendered = false;
            isEditRow = false;
        }
        void OnRowRender(RowRenderEventArgs<TramitesAntecedentesDdjjFilaDto> args)
        {
            // Chequeo si es Marzo 2022
            var PeriodoData = Convert.ToInt32($"{args.Data.Anio}{args.Data.Mes:00}");
            if (ResumenMejores12 != null && ResumenMejores12.AnioInicio > 0 && ResumenMejores12.AnioFin > 0)
            {
                var PeriodoInicioResumen = Convert.ToInt32($"{ResumenMejores12.AnioInicio}{ResumenMejores12.MesInicio:00}");
                var PeriodoFinResumen = Convert.ToInt32($"{ResumenMejores12.AnioFin}{ResumenMejores12.MesFin:00}");

                if (PeriodoData >= PeriodoInicioResumen && PeriodoData <= PeriodoFinResumen)
                {
                    args.Attributes["style"] = "background-color: #cae9f3;";
                }
            }

            if (isEditRow && !isRowRendered)
            {
                // ponemos un pequeño delay para que el input ya exista en DOM
                _ = Task.Run(async () =>
                {
                    await Task.Delay(50);
                    await JS.InvokeVoidAsync("eval", $"document.querySelector('input')?.focus();document.querySelector('input')?.select();");
                    isRowRendered = true;
                });
                
            }
        }
    }
}
