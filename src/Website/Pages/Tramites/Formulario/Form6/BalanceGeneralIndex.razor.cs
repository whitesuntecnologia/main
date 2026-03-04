using AutoMapper;
using Business;
using Business.Extensions;
using Business.Interfaces;
using DataAccess.Entities;
using DataTransferObject;
using DataTransferObject.BLs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using StaticClass;
using System;
using System.Linq;
using Website.Models.Formulario;

namespace Website.Pages.Tramites.Formulario.Form6
{
    public partial class BalanceGeneralIndex : ComponentBase
    {
        [Parameter] public string Accion { get; set; } = null!;
        [Parameter] public string IdentificadorUnico { get; set; } = null!;
        [Inject] private DialogService DS { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] protected RouteHistoryService routeHistory { get; set; } = null!;

        [Inject] private ITramitesBL _TramiteBL { get; set; } = null!;
        [Inject] private ITramitesEvaluacionBL _TramiteEvaluacionBL { get; set; } = null!;
        [Inject] private IFilesBL _FilesBL { get; set; } = null!;
        [Inject] private IUsuariosBL _usuarioBL { get; set; } = null!;
        [Inject] private ITablasBL _tablasBL { get; set; } = null!;
        [Inject] private IIndiceUnidadViviendaBL _indiceUnidadViviendaBL { get; set; } = null!;
        [Inject] private IEspecilidadesBL especialidadesBL { get; set; } = null!;
        [Inject] private IAntecedentesBl _AntecedentesBL { get; set; } = null!;
        
        private IMapper _mapper { get; set; } = null!;
        private bool EvaluandoTramite { get; set; } = false;
        private TramitesDTO tramite { get; set; } = new();
        private List<BalanceGeneralModel> BalancesModel { get; set; } = new();
        private BalanceGeneral_Evaluar_Model EvaluarModel { get; set; } = new();
        private List<PerfilDTO> lstPerfiles { get; set; } = new();

        private bool isBusyInitialLoading { get; set; } = true;
        private bool isLoaded { get; set; } = false;
        private bool PermiteEditar = false;

        private void CrearMapper()
        {
           
            //Crear el mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BalanceGeneralModel, TramitesBalanceGeneralDTO>().ReverseMap();
                cfg.CreateMap<BalanceGeneral_Evaluar_Model, TramitesBalanceGeneralDTO>().ReverseMap();
                
                
                cfg.CreateMap<BalanceGeneral_Evaluar_Model, TramitesBalanceGeneralEvaluarDTO>()
                    .ForMember(dest => dest.CoefObrasOficialesDepositosCortoPlazo, opt => opt.MapFrom(src => src.DatosEvaluacion.CoefObrasOficialesDepositosCortoPlazo))
                    .ForMember(dest => dest.CoefObrasOficialesDepositosLargoPlazo, opt => opt.MapFrom(src => src.DatosEvaluacion.CoefObrasOficialesDepositosLargoPlazo))
                    .ForMember(dest => dest.CoefObrasOficialesInversionesCortoPlazo, opt => opt.MapFrom(src => src.DatosEvaluacion.CoefObrasOficialesInversionesCortoPlazo))
                    .ForMember(dest => dest.CoefObrasOficialesInversionesLargoPlazo, opt => opt.MapFrom(src => src.DatosEvaluacion.CoefObrasOficialesInversionesLargoPlazo))
                    .ForMember(dest => dest.CoefObrasOficialesOtrosConceptos, opt => opt.MapFrom(src => src.DatosEvaluacion.CoefObrasOficialesOtrosConceptos))
                    .ForMember(dest => dest.CoefObrasParticularesDepositosCortoPlazo, opt => opt.MapFrom(src => src.DatosEvaluacion.CoefObrasParticularesDepositosCortoPlazo))
                    .ForMember(dest => dest.CoefObrasParticularesDepositosLargoPlazo, opt => opt.MapFrom(src => src.DatosEvaluacion.CoefObrasParticularesDepositosLargoPlazo))
                    .ForMember(dest => dest.CoefObrasParticularesInversionesCortoPlazo, opt => opt.MapFrom(src => src.DatosEvaluacion.CoefObrasParticularesInversionesCortoPlazo))
                    .ForMember(dest => dest.CoefObrasParticularesInversionesLargoPlazo, opt => opt.MapFrom(src => src.DatosEvaluacion.CoefObrasParticularesInversionesLargoPlazo))
                    .ForMember(dest => dest.CoefObrasParticularesInversiones, opt => opt.MapFrom(src => src.DatosEvaluacion.CoefObrasParticularesInversiones))
                    .ForMember(dest => dest.CoefObrasParticularesOtrosConceptos, opt => opt.MapFrom(src => src.DatosEvaluacion.CoefObrasParticularesOtrosConceptos))
                    .ForMember(dest => dest.CoefDisponibilidadesCajaYBancos, opt => opt.MapFrom(src => src.DatosEvaluacion.CoefDisponibilidadesCajaYBancos))
                    .ForMember(dest => dest.CoefBienesDeUsoInversiones, opt => opt.MapFrom(src => src.DatosEvaluacion.CoefBienesDeUsoInversiones))
                    .ForMember(dest => dest.CoefBienesDeUsoEquiposAfectados, opt => opt.MapFrom(src => src.DatosEvaluacion.CoefBienesDeUsoEquiposAfectados))
                    .ForMember(dest => dest.CoefBienesDeUsoEquiposNoAfectados, opt => opt.MapFrom(src => src.DatosEvaluacion.CoefBienesDeUsoEquiposNoAfectados))
                    .ForMember(dest => dest.CoefBienesDeUsoInmueblesAfectados, opt => opt.MapFrom(src => src.DatosEvaluacion.CoefBienesDeUsoInmueblesAfectados))
                    .ForMember(dest => dest.CoefBienesDeUsoInmueblesNoAfectados, opt => opt.MapFrom(src => src.DatosEvaluacion.CoefBienesDeUsoInmueblesNoAfectados))
                    .ForMember(dest => dest.CoefBienesDeUsoOtrosConceptos, opt => opt.MapFrom(src => src.DatosEvaluacion.CoefBienesDeUsoOtrosConceptos))
                    .ForMember(dest => dest.CoefBienesDeCambioMaterialesEnDeposito, opt => opt.MapFrom(src => src.DatosEvaluacion.CoefBienesDeCambioMaterialesEnDeposito))
                    .ForMember(dest => dest.CoefBienesDeCambioOtrosConceptos, opt => opt.MapFrom(src => src.DatosEvaluacion.CoefBienesDeCambioOtrosConceptos))
                    .ForMember(dest => dest.CoefDeudasCortoPlazo, opt => opt.MapFrom(src => src.DatosEvaluacion.CoefDeudasCortoPlazo))
                    .ForMember(dest => dest.CoefDeudasLargoPlazo, opt => opt.MapFrom(src => src.DatosEvaluacion.CoefDeudasLargoPlazo))
                    .ForMember(dest => dest.CoefCapacidadEconomicaArqRep, opt => opt.MapFrom(src => src.DatosEvaluacion.CoefCapacidadEconomicaArqRep))
                    .ForMember(dest => dest.CoefCapacidadEconomicaByC, opt => opt.MapFrom(src => src.DatosEvaluacion.CoefCapacidadEconomicaByC))
                    .ForMember(dest => dest.CoefCapacidadEconomicaAyC, opt => opt.MapFrom(src => src.DatosEvaluacion.CoefCapacidadEconomicaAyC))
                    .ForMember(dest => dest.CapitalRealEspecifico, opt => opt.MapFrom(src => src.ValoresCalculados.CapitalRealEspecifico))
                    .ForMember(dest => dest.CapitalRealEspecificoEvaluar, opt => opt.MapFrom(src => src.ValoresCalculados.CapitalRealEspecificoEvaluar))
                    .ForMember(dest => dest.CapacidadEconomicaArqRep, opt => opt.MapFrom(src => src.ValoresCalculados.CapacidadEconomicaArqRep))
                    .ForMember(dest => dest.CapacidadEconomicaByC, opt => opt.MapFrom(src => src.ValoresCalculados.CapacidadEconomicaByC))
                    .ForMember(dest => dest.CapacidadEconomicaAyC, opt => opt.MapFrom(src => src.ValoresCalculados.CapacidadEconomicaAyC))
                    .ForMember(dest => dest.IndiceSolvencia, opt => opt.MapFrom(src => src.ValoresCalculados.IndiceSolvencia))
                    .ForMember(dest => dest.IndiceLiquidez, opt => opt.MapFrom(src => src.ValoresCalculados.IndiceLiquidez))
                    .ForMember(dest => dest.IndiceSolvenciaResultante, opt => opt.MapFrom(src => src.ValoresCalculados.IndiceSolvenciaResultante))
                    .ForMember(dest => dest.IndiceLiquidezResultante, opt => opt.MapFrom(src => src.ValoresCalculados.IndiceLiquidezResultante))
                    .ForMember(dest => dest.IndiceAntiguedadEmpresa, opt => opt.MapFrom(src => src.ValoresCalculados.IndiceAntiguedadEmpresa))
                    .ForMember(dest => dest.IndiceRelacionEquipos, opt => opt.MapFrom(src => src.ValoresCalculados.IndiceRelacionEquipos))
                    .ForMember(dest => dest.IndiceRelacionEquiposResultante, opt => opt.MapFrom(src => src.ValoresCalculados.IndiceRelacionEquiposResultante))
                    .ForMember(dest => dest.Factor, opt => opt.MapFrom(src => src.ValoresCalculados.Factor))
                    .ForMember(dest => dest.TramitesBalancesGeneralesEvaluarCapEjec, opt => opt.MapFrom(src => src.lstCapacidadEjecucion))
                    .ForMember(dest => dest.TramitesBalancesGeneralesEvaluarCapProd, opt => opt.MapFrom(src => src.lstCapacidadProduccion))
                    .ForMember(dest => dest.TramitesBalancesGeneralesEvaluarCapTecnica, opt => opt.MapFrom(src => src.lstCapacidadTecnica))
                    .ForMember(dest => dest.TramitesBalancesGeneralesEvaluarCapTecxEquipo, opt => opt.MapFrom(src => src.lstCapacidadTecnicaxEquipo))
                    .ForMember(dest => dest.TramitesBalancesGeneralesEvaluarConstanciaCap, opt => opt.MapFrom(src => src.lstConstanciaCapacidad))
                    .ForMember(dest => dest.TramitesBalancesGeneralesEvaluarDetalleObrasEjecucion, opt => opt.MapFrom(src => src.lstDetalleObrasEnEjecucion))

                .ReverseMap();

                cfg.CreateMap<BalanceGeneral_CapTecxEquipoItem_Evaluar_Model, TramitesBalancesGeneralesEvaluarCapTecxEquipoDTO>().ReverseMap();
                cfg.CreateMap<BalanceGeneral_CapTecxEquipoItem_Evaluar_Model, TramitesBalancesGeneralesEvaluarCapProdDTO>().ReverseMap();
                cfg.CreateMap<BalanceGeneral_CapacidadEjecucionItem_Evaluar_Model, TramitesBalancesGeneralesEvaluarCapEjecDTO>().ReverseMap();
                cfg.CreateMap<BalanceGeneral_CapTecItem_Evaluar_Model, TramitesBalancesGeneralesEvaluarCapTecnicaDTO>().ReverseMap();
                cfg.CreateMap<BalanceGeneral_ConstanciaCapacidadItem_Evaluar_Model, TramitesBalancesGeneralesEvaluarConstanciaDTO>().ReverseMap();
                cfg.CreateMap<BalanceGeneral_ObrasEjecucionItem_Evaluar_Model, TramitesBalancesGeneralesEvaluarDetalleObrasEjecucionDTO>().ReverseMap();

                cfg.CreateMap<BalanceGeneral_CapTecItem_Evaluar_Model, ItemGrillaBGECapacidadTecnica2DTO>().ReverseMap();
                cfg.CreateMap<ItemGrillaBGECapacidadTecnicaxEquipoDTO, BalanceGeneral_CapTecxEquipoItem_Evaluar_Model>();
                
                cfg.CreateMap<BalanceGeneralCapacidadProduccionDto, BalanceGeneral_CapTecxEquipoItem_Evaluar_Model>().ReverseMap();
                
            });
            _mapper = config.CreateMapper();
            //--
        }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            EvaluandoTramite = await _TramiteBL.isEvaluandoTramite(IdentificadorUnico);
            // Comprueba la seguridad del trámite
            if (!await _TramiteBL.ComprobarSeguridad(IdentificadorUnico))
            {
                navigationManager.NavigateTo("/ValidationError/401", true);
                return;
            }
            //--

            CrearMapper();
            string userid = await _usuarioBL.GetCurrentUserid();
            lstPerfiles = await _usuarioBL.GetPerfilesForUserAsync(userid);

            tramite = await _TramiteBL.GetTramiteByGuidAsync(IdentificadorUnico);
            if(EvaluandoTramite)
                PermiteEditar = tramite.PermiteEditarFormulario(Constants.Formularios.BalanceGeneral);
            else
                PermiteEditar = tramite.PermiteEditarEmpresa && tramite.PermiteEditarFormulario(Constants.Formularios.BalanceGeneral);

            await CargarBalances();
            isBusyInitialLoading = false;


        }
        private async Task CargarBalances()
        {
            try
            {

            
                var balances = (await _TramiteBL.GetBalancesGeneralesAsync(tramite.IdTramite)).OrderBy(x=> x.Anio).ToList();

                if (balances.Count > 0 && (lstPerfiles.Exists(x => x.IdPerfil == Constants.Perfiles.Administrador) ||
                   lstPerfiles.Exists(x => x.IdPerfil == Constants.Perfiles.Evaluador)))
                {
                    var ultimoBalance = balances.OrderByDescending(o => o.Anio).First();

                    var EvaluarDTO = await _TramiteEvaluacionBL.GetBalanceGeneralEvaluar(ultimoBalance.IdTramiteBalanceGeneral.GetValueOrDefault());

                    EvaluarModel = _mapper.Map<BalanceGeneral_Evaluar_Model>(ultimoBalance) ?? new BalanceGeneral_Evaluar_Model();
                    if (EvaluarDTO != null && !EvaluandoTramite)
                    {
                        _mapper.Map<TramitesBalanceGeneralEvaluarDTO, BalanceGeneral_Evaluar_Model>(EvaluarDTO, EvaluarModel);
                    }
                    BalancesModel = new List<BalanceGeneralModel>() { _mapper.Map<BalanceGeneralModel>(ultimoBalance) };


                    EvaluarModel.FechaVencimientoConstanciaAnterior = await _TramiteBL.GetFechaVencimientoConstanciaInscripcion(tramite.IdTramite);
                    if (tramite.IdTipoTramite == Constants.TiposDeTramite.Reli_Licitar)
                    {
                        var ObraDto = await _TramiteEvaluacionBL.GetDatosObraLicitarAsync(tramite.IdTramite);
                        EvaluarModel.MontoObra = ObraDto.MontoObra.GetValueOrDefault();
                        EvaluarModel.PlazoObra = ObraDto.PlazoObra.GetValueOrDefault();
                    }

                    // Si se está evaluando o si es la primera vez que se entra al formulario se realizan todos los calculos,
                    // caso contrario se utilizan los almacenados en base de datos.
                    if ((EvaluarDTO == null || EvaluandoTramite) && tramite.IdTipoTramite != Constants.TiposDeTramite.Reli_ActualizacionSoloTecnicos)
                    {

                        if (EvaluarDTO != null)
                        {
                            EvaluarModel.IdTramiteBalanceGeneral = EvaluarDTO.IdTramiteBalanceGeneral;
                            EvaluarModel.IdTramiteBalanceGeneralEvaluar = EvaluarDTO.IdTramiteBalanceGeneralEvaluar;
                        }

                        var datosEmpresa = await _TramiteBL.GetInfEmpAsync(tramite.IdTramite);
                        var lstAntecedentes = await _AntecedentesBL.GetAntecedentesAsync(tramite.IdTramite);
                        var lstSecciones = await especialidadesBL.GetEspecialidadesSeccionesAsync();
                        var lstEspecialidadesForm1 = await _TramiteBL.GetEspecialidadesAsync(tramite.IdTramite);
                        var lstSeccionesForm1 = new List<EspecialidadSeccionDTO>();

                        foreach (var f in lstEspecialidadesForm1)
                        {
                            foreach (var sec in f.Secciones)
                            {
                                lstSeccionesForm1.Add(new EspecialidadSeccionDTO
                                {
                                    IdSeccion = sec.IdSeccion,
                                    Rama = f.Rama,
                                    DescripcionSeccion = (f.Rama == "C" ? "Comp. " : "") + sec.DescripcionSeccion,
                                });
                            }
                        }

                        int EstadoDeudaBCRA = await _TramiteEvaluacionBL.GetEstadoDeudaBCRAAsync(tramite.IdTramite);
                        decimal indiceUVI = 1;

                        if (EvaluarModel.FechaBalance.HasValue)
                        {
                            var fechaBalance = EvaluarModel.FechaBalance.Value;
                            //se pidió que el indice de UVI sea del período anterior a la fecha de creacion del trámite.
                            var fechaPedidoMesAnterior = tramite.CreateDate.AddMonths(-1);
                            var dtoIndiceUVIPedido = await _indiceUnidadViviendaBL.GetIndiceAsync(fechaPedidoMesAnterior.Month, fechaPedidoMesAnterior.Year);
                            var dtoIndiceUVIBalance = await _indiceUnidadViviendaBL.GetIndiceAsync(fechaBalance.Month, fechaBalance.Year);
                            if (dtoIndiceUVIPedido != null && dtoIndiceUVIBalance != null) indiceUVI = dtoIndiceUVIPedido.Valor / dtoIndiceUVIBalance.Valor;
                        }


                        //Recupera los datos de la Grilla de Capacidad Técnica de la hoja 2 (Tecnica)
                        //Solo recupera los datos de las obras, los datos del footer se recuperan en la actualización de los cálculos.
                        var lstObrasCapacidadTecnica2 = _mapper.Map<List<BalanceGeneral_CapTecItem_Evaluar_Model>>(
                                                            await _TramiteEvaluacionBL.GetGrillaCapacidadTecnica2(tramite.IdTramite));

                        var lstObrasEjecucion = await _TramiteBL.GetObrasEjecucionAsync(tramite.IdTramite);
                        decimal TotalContratadoObrasEjecucion = lstObrasEjecucion.Sum(s => s.TotalContratado);
                        decimal PromedioCoeficienteConceptual = await _TramiteEvaluacionBL.GetPromedioCoeficienteConceptualAsync(tramite.IdTramite);
                        

                        var EquiposAfec = await _TramiteBL.GetEquiposAsync(tramite.IdTramite, true);
                        var EquiposNoAfec = await _TramiteBL.GetEquiposAsync(tramite.IdTramite, false);
                        var InmueblesAfec = await _TramiteBL.GetBienesRaicesAsync(tramite.IdTramite, true);
                        var InmueblesNoAfec = await _TramiteBL.GetBienesRaicesAsync(tramite.IdTramite, false);
                        
                        //Recupera los Montos de realizacion que puso el Evaluador en Equipos e Inmuebles
                        decimal equiposAfec = EquiposAfec?.MontoRealizacionEvaluador ?? 0;
                        decimal equiposNoAfec = EquiposNoAfec?.MontoRealizacionEvaluador ?? 0;
                        decimal inmueblesAfec = InmueblesAfec?.MontoRealizacionEvaluador ?? 0;
                        decimal inmueblesNoAfec = InmueblesNoAfec?.MontoRealizacionEvaluador ?? 0;

                        EvaluarModel.BusoMaqUtilAfec = equiposAfec;
                        EvaluarModel.BusoMaqUtilNoAfec = equiposNoAfec;
                        EvaluarModel.BusoBienesRaicesAfec = inmueblesAfec;
                        EvaluarModel.BusoBienesRaicesNoAfec = inmueblesNoAfec;

                        #region Cargar CapacidadTecnicaxEquipo

                        EvaluarModel.lstCapacidadTecnicaxEquipo = _mapper.Map<List<ItemGrillaBGECapacidadTecnicaxEquipoDTO>, List<BalanceGeneral_CapTecxEquipoItem_Evaluar_Model>>(
                                                            await _TramiteEvaluacionBL.GetGrillaBGECapacidadTecnicaxEquipoAsync(tramite.IdTramite));
                        

                        #endregion
                        #region Calcula el detalle del monto comprometido de obras en Ejecucion

                        EvaluarModel.TotalEquiposAfecEvaluar = equiposAfec;
                        EvaluarModel.TotalEquiposNoAfecEvaluar = equiposNoAfec;
                        EvaluarModel.TotalInmueblesAfecEvaluar = inmueblesAfec;
                        EvaluarModel.TotalInmueblesNoAfecEvaluar = inmueblesNoAfec;

                        //Calcula la antigüedad de la empresa contra la fecha del balance para la evaluación,
                        //ya que el requisito de antigüedad se evalúa contra la fecha del balance y no contra la fecha actual.
                        int AniosAntiguedadContraBalance = 0;
                        if(EvaluarModel.FechaBalance.HasValue && datosEmpresa != null)
                        {
                            AniosAntiguedadContraBalance = EvaluarModel.FechaBalance.Value.Year - datosEmpresa.FechaInicioActividades.Year;
                            // Si todavía no pasó el aniversario en el año final, resto 1
                            if (EvaluarModel.FechaBalance.Value < datosEmpresa.FechaInicioActividades.AddYears(AniosAntiguedadContraBalance))
                            {
                                AniosAntiguedadContraBalance--;
                            }
                        }
                        //--
                        
                        EvaluarModel.ValoresCalculados.AniosAntiguedadEmpresa = AniosAntiguedadContraBalance;
                        EvaluarModel.indiceUVI = indiceUVI;
                        EvaluarModel.TotalContratadoObrasEjecucion = TotalContratadoObrasEjecucion;
                        EvaluarModel.EstadoDeudaBCRA = EstadoDeudaBCRA;
                        EvaluarModel.CoeficienteConceptual = PromedioCoeficienteConceptual;

                        DateTime hoy = DateTime.Today;
                        DateTime fechaCertificacion = await _TramiteBL.GetFechaCertificacionAsync(tramite.IdTramite) ?? DateTime.Now; 

                        EvaluarModel.lstDetalleObrasEnEjecucion = lstObrasEjecucion.Select(s => new BalanceGeneral_ObrasEjecucionItem_Evaluar_Model
                        {
                            IdObraPciaLp = s.IdObraPciaLp,
                            ObraNombre = s.ObraNombre,
                            Comitente = s.Comitente,
                            PeríodoBase = s.PeriodoBase,
                            TotalContratado = s.TotalContratado,
                            TotalCertificado = s.TotalCertificado,
                            Plazo = Convert.ToInt32(s.PlazoObra + s.PlazoAmpliacion) ,
                            FechaInicio = s.FechaInicio,
                            FechaCertificacion = fechaCertificacion,
                            DiasTranscurridos = (int) (fechaCertificacion - s.FechaInicio).TotalDays + 1,       // + 1 para que tome en cuenta el día actual o el ultimo día
                            CoefParticipacion = (s.PorcentajeParticipacionUTE.HasValue ? s.PorcentajeParticipacionUTE.Value / 100 : 1),
                            MontoMensual = s.MontoMensual,
                        }).ToList();

                        foreach (var item in EvaluarModel.lstDetalleObrasEnEjecucion)
                        {
                            item.MontoAnual = item.MontoMensual * 12;
                            item.PorcentajeCertificado = item.TotalCertificado / item.TotalContratado * 100;
                            item.CoefCertificado = (item.PorcentajeCertificado >= 40m ? 1m : 0.5m);
                            item.PorcentajeTiempo = (decimal)item.DiasTranscurridos / item.Plazo * 100;
                            item.CoefMatriz = await _TramiteEvaluacionBL.GetCoeficienteMatrizObrasEjecAsync(item.PorcentajeCertificado, item.PorcentajeTiempo);
                            item.MontoComprometido = item.MontoAnual * item.CoefMatriz * item.CoefParticipacion;
                        }

                        #endregion

                        EvaluarModel.lstCapacidadProduccion = _mapper.Map<List<BalanceGeneral_CapTecxEquipoItem_Evaluar_Model>>(await _AntecedentesBL.GetCapacidadProduccionAsync(tramite.IdTramite));

                        EvaluarModel.CargarDatosIniciales(lstAntecedentes,
                                                          lstSecciones,
                                                          lstSeccionesForm1,
                                                          lstObrasCapacidadTecnica2
                                                          );

                        EvaluarModel.FechaVencimientoConstanciaAnterior = await _TramiteBL.GetFechaVencimientoConstanciaInscripcion(tramite.IdTramite);


                        EvaluarModel.ActualizarCalculos();

                        //Se actualiza todas las tablas para que se actualice el UVI y todos los valores calculados
                        var dtoModel = _mapper.Map<TramitesBalanceGeneralEvaluarDTO>(EvaluarModel);
                        dtoModel = await _TramiteEvaluacionBL.InsertOrUpdateBalanceGeneralEvaluar(dtoModel);
                        EvaluarModel.IdTramiteBalanceGeneralEvaluar = dtoModel.IdTramiteBalanceGeneralEvaluar;
                    }
                }
                else
                    BalancesModel = _mapper.Map<List<TramitesBalanceGeneralDTO>, List<BalanceGeneralModel>>(balances);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        protected void AgregarBalanceClick()
        {
            navigationManager.NavigateTo($"/Tramites/BalanceGeneral/{Accion}/{IdentificadorUnico}/Agregar", true);
        }
        protected void EditarBalanceClick(int IdTramiteBalanceGeneral) 
        {
            navigationManager.NavigateTo($"/Tramites/BalanceGeneral/{Accion}/{IdentificadorUnico}/Editar/{IdTramiteBalanceGeneral}");
        }

        protected async Task EliminarBalanceClick(int Anio)
        {
            try
            {
                var respuesta = await DS.Confirm("¿Está seguro de eliminar el registro?", "Confirmación",
                    new ConfirmOptions() { CancelButtonText = "No", OkButtonText = "Sí" });

                if (respuesta ?? false)
                {
                    await _TramiteBL.EliminarBalanceGeneralAsync(tramite.IdTramite,Anio);
                    navigationManager.NavigateTo($"/Tramites/BalanceGeneral/{Accion}/{IdentificadorUnico}",true);
                }
            }
            catch (Exception ex)
            {
                notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
            }
        }
        protected Task VolverAlVisor()
        {
            
            navigationManager.NavigateTo($"/Tramites/{Accion}/{IdentificadorUnico}", false);
            return Task.CompletedTask;
        }

        protected async Task GuardarEvaluacionClick(TramiteFormularioEvaluadoDTO dto)
        {
            try
            {
                var dtoModel = _mapper.Map<TramitesBalanceGeneralEvaluarDTO>(EvaluarModel);
                await _TramiteEvaluacionBL.InsertOrUpdateBalanceGeneralEvaluar(dtoModel);
                await _TramiteEvaluacionBL.GuardarEvaluacionAsync(dto);
                await VolverAlVisor();
            }
            catch (Exception ex)
            {
                notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.VerySlow);
            }
        }
        protected Task CancelarEvaluacionClick()
        {
            VolverAlVisor();
            return Task.CompletedTask;
        }
        
    }
}
