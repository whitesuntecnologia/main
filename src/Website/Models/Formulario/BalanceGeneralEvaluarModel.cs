using Radzen.Blazor.Rendering;
using Radzen.Blazor;
using System.Drawing;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using Business;
using Microsoft.AspNetCore.Components;
using Business.Interfaces;
using DataTransferObject;
using DataAccess.Entities;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Website.Models.Formulario
{
    public class BalanceGeneral_Evaluar_Model
    {

        #region Datos del Balance cargados por la Empresa
        public int? IdTramiteBalanceGeneralEvaluar { get; set; }
        public int? IdTramiteBalanceGeneral { get; set; }
        public int IdTramite { get; set; }
        public int IdTipoTramite { get; set; }
        public int? Anio { get; set; }
        public DateTime? FechaBalance { get; set; }
        public DateTime? FechaVigencia
        {
            get
            {
                return FechaBalance.HasValue ? FechaBalance.Value.AddMonths(18) : null;
            }
        }
        public DateTime? FechaVencimientoConstanciaAnterior { get; set; }
        public decimal? OoficDepositosCortoPlazo { get; set; }
        public decimal? OoficDepositosLargoPlazo { get; set; }
        public decimal? OoficInversionesCortoPlazo { get; set; }
        public decimal? OoficInversionesLargoPlazo { get; set; }
        public decimal? OoficOtrosConceptos { get; set; }
        public decimal? OpartDepositosCortoPlazo { get; set; }
        public decimal? OpartDepositosLargoPlazo { get; set; }
        public decimal? OpartInversionesCortoPlazo { get; set; }
        public decimal? OpartInversionesLargoPlazo { get; set; }
        public decimal? OpartInversiones { get; set; }
        public decimal? OpartOtrosConceptos { get; set; }
        public decimal? DispCajayBancos { get; set; }
        public decimal? BusoInversiones { get; set; }
        public decimal? BusoMaqUtilAfec { get; set; }
        public decimal? BusoMaqUtilNoAfec { get; set; }
        public decimal? BusoBienesRaicesAfec { get; set; }
        public decimal? BusoBienesRaicesNoAfec { get; set; }
        public decimal? BusoOtrosConceptos { get; set; }
        public decimal? BcamMateriales { get; set; }
        public decimal? BcamOtrosConceptos { get; set; }
        public decimal? DeuCortoPlazo { get; set; }
        public decimal? DeuLargoPlazo { get; set; }
        public int? IdFile { get; set; }
        public string? Filename { get; set; }
        public decimal? OoficDepositosCortoPlazoEmp { get; set; }
        public decimal? OoficDepositosLargoPlazoEmp { get; set; }
        public decimal? OoficInversionesCortoPlazoEmp { get; set; }
        public decimal? OoficInversionesLargoPlazoEmp { get; set; }
        public decimal? OoficOtrosConceptosEmp { get; set; }
        public decimal? OpartDepositosCortoPlazoEmp { get; set; }
        public decimal? OpartDepositosLargoPlazoEmp { get; set; }
        public decimal? OpartInversionesCortoPlazoEmp { get; set; }
        public decimal? OpartInversionesLargoPlazoEmp { get; set; }
        public decimal? OpartInversionesEmp { get; set; }
        public decimal? OpartOtrosConceptosEmp { get; set; }
        public decimal? DispCajayBancosEmp { get; set; }
        public decimal? BusoInversionesEmp { get; set; }
        public decimal? BusoMaqUtilAfecEmp { get; set; }
        public decimal? BusoMaqUtilNoAfecEmp { get; set; }
        public decimal? BusoBienesRaicesAfecEmp { get; set; }
        public decimal? BusoBienesRaicesNoAfecEmp { get; set; }
        public decimal? BusoOtrosConceptosEmp { get; set; }
        public decimal? BcamMaterialesEmp { get; set; }
        public decimal? BcamOtrosConceptosEmp { get; set; }
        public decimal? DeuCortoPlazoEmp { get; set; }
        public decimal? DeuLargoPlazoEmp { get; set; }
        public decimal ActivoTotal
        {
            get
            {
                return ActivoCorrienteTotal + ActivoNoCorrienteTotal;
            }
        }
        public decimal ActivoCorrienteTotal
        {
            get
            {
                decimal result = 0;
                result = OoficDepositosCortoPlazo.GetValueOrDefault() +
                        OoficInversionesCortoPlazo.GetValueOrDefault() +
                        OoficOtrosConceptos.GetValueOrDefault() +
                        OpartDepositosCortoPlazo.GetValueOrDefault() +
                        OpartInversionesCortoPlazo.GetValueOrDefault() +
                        OpartInversiones.GetValueOrDefault() +
                        OpartOtrosConceptos.GetValueOrDefault() +
                        DispCajayBancos.GetValueOrDefault() +
                        BcamMateriales.GetValueOrDefault() +
                        BcamOtrosConceptos.GetValueOrDefault();
                return result;
            }
        }
        public decimal ActivoNoCorrienteTotal
        {
            get
            {
                decimal result = 0;
                result = OoficDepositosLargoPlazo.GetValueOrDefault() +
                        OoficInversionesLargoPlazo.GetValueOrDefault() +
                        OpartDepositosLargoPlazo.GetValueOrDefault() +
                        OpartInversionesLargoPlazo.GetValueOrDefault() +
                        BusoInversiones.GetValueOrDefault() +
                        BusoMaqUtilAfec.GetValueOrDefault() +
                        BusoMaqUtilNoAfec.GetValueOrDefault() +
                        BusoBienesRaicesAfec.GetValueOrDefault() +
                        BusoBienesRaicesNoAfec.GetValueOrDefault() +
                        BusoOtrosConceptos.GetValueOrDefault();
                return result;
            }
        }
        public decimal PasivoTotal
        {
            get
            {
                return PasivoCorrienteTotal + PasivoNoCorrienteTotal;
            }
        }
        public decimal PasivoCorrienteTotal
        {
            get
            {
                decimal result = 0;
                result = DeuCortoPlazo.GetValueOrDefault();
                return result;
            }
        }
        public decimal PasivoNoCorrienteTotal
        {
            get
            {
                decimal result = 0;
                result = DeuLargoPlazo.GetValueOrDefault();
                return result;
            }
        }

        //Campos Agregados al Original
        private decimal? _CoeficienteConceptual;
        public decimal? CoeficienteConceptual
        {
            get { return _CoeficienteConceptual; }
            set
            {
                _CoeficienteConceptual = value;
                ActualizarCalculos();
            }
        }
        public decimal MontoObra { get; set; }
        public int PlazoObra { get; set; }
        public decimal CapacidadContratacionAnualRequerida 
        { 
            get
            {
                decimal result = MontoObra / ((PlazoObra == 0 ? 1 : PlazoObra) / 30m) * 12;
                return result;
            }
        }

        public int EstadoDeudaBCRA { get; set; }
        public decimal CoeficienteEstadoDeudaBCRA { get; set; } = 1;
        public decimal? TotalEquiposAfecEvaluar { get; set; }   //Vienen del formulario Equipos, los que puso el evaluador
        public decimal? TotalEquiposNoAfecEvaluar { get; set; }   //Vienen del formulario Equipos, los que puso el evaluador
        public decimal? TotalInmueblesAfecEvaluar { get; set; } //Vienen del formulario Inmuebles, los que puso el evaluador
        public decimal? TotalInmueblesNoAfecEvaluar { get; set; } //Vienen del formulario Inmuebles, los que puso el evaluador


        #endregion

        public BalanceGeneral_DatosEvaluacion_Model DatosEvaluacion { get; set; } = new();
        public BalanceGeneral_ValoresCalculados_Model ValoresCalculados { get; set; } = new();
        public List<BalanceGeneral_CapTecxEquipoItem_Evaluar_Model> lstCapacidadTecnicaxEquipo { get; set; } = new();
        public List<BalanceGeneral_CapTecxEquipoItem_Evaluar_Model> lstCapacidadProduccion { get; set; } = new();
        public List<BalanceGeneral_CapacidadEjecucionItem_Evaluar_Model> lstCapacidadEjecucion { get; set; } = new();
        public List<BalanceGeneral_CapTecItem_Evaluar_Model> lstCapacidadTecnica { get; set; } = new();
        public List<BalanceGeneral_ConstanciaCapacidadItem_Evaluar_Model> lstConstanciaCapacidad { get; set; } = new();
        public List<BalanceGeneral_ObrasEjecucionItem_Evaluar_Model> lstDetalleObrasEnEjecucion { get; set; } = new();

        public List<EspecialidadSeccionDTO> _lstSecciones { get; set; } = new();
        public List<TramitesAntecedentesDto> _lstAntecedentes { get; set; } = new();
        public decimal indiceUVI { get; set; } = 1m;
        public decimal TotalContratadoObrasEjecucion { get; set; }

        public void CargarDatosIniciales(List<TramitesAntecedentesDto> lstAntecedentes,
                    List<EspecialidadSeccionDTO> lstSecciones, List<EspecialidadSeccionDTO> lstSeccionesForm1,
                    List<BalanceGeneral_CapTecItem_Evaluar_Model> _lstCapacidadTecnica
                    )
        {
            _lstSecciones = lstSecciones;
            _lstAntecedentes = lstAntecedentes;

            //if (lstCapacidadProduccion.Count == 0)
            //{

            //    this.lstCapacidadProduccion =
            //    (from ant in _lstAntecedentes
            //     join sec in _lstSecciones on ant.IdSeccion equals sec.IdSeccion
            //     select new BalanceGeneral_CapTecxEquipoItem_Evaluar_Model
            //     {
            //         IdSeccion = sec.IdSeccion,
            //         DescripcionSeccion = sec.DescripcionSeccion,
            //         CoefCapacTecnicaxEquipo = sec.CoefCapacTecnicaxEquipo,
            //         MultiplicadorCapEconomica = sec.MultiplicadorCapEconomica,
            //         Importe = ant.TramitesAntecedentesDdjjmensual?.Sum(d => d.Monto) ?? 0
            //     }).ToList();
                
            //}
            

            if (this.lstCapacidadEjecucion.Count == 0)
            {
                this.lstCapacidadEjecucion = lstSecciones.Select(s => new BalanceGeneral_CapacidadEjecucionItem_Evaluar_Model
                {
                    IdSeccion = s.IdSeccion,
                    DescripcionSeccion = s.DescripcionSeccion,
                    ExistsInForm1 = lstSeccionesForm1.Exists(x => x.IdSeccion == s.IdSeccion)
                }).ToList();
            }

            if (this.EstadoDeudaBCRA == 1) CoeficienteEstadoDeudaBCRA = 1m;
            if (this.EstadoDeudaBCRA == 2) CoeficienteEstadoDeudaBCRA = 0.6m;
            if (this.EstadoDeudaBCRA == 3) CoeficienteEstadoDeudaBCRA = 0.1m;
            if (this.EstadoDeudaBCRA == 4) CoeficienteEstadoDeudaBCRA = 0m;

            lstCapacidadTecnica = _lstCapacidadTecnica.ToList();


            //Se crea la lista de Constancia de capacidad con las especialidades del Form1
            if (lstConstanciaCapacidad.Count == 0)
            {
                lstConstanciaCapacidad.AddRange(lstSeccionesForm1.Select(s => new BalanceGeneral_ConstanciaCapacidadItem_Evaluar_Model
                {
                    IdSeccion = s.IdSeccion,
                    Rama = s.Rama,
                    DescripcionSeccion = s.DescripcionSeccion,
                }).ToList());
            }

            ActualizarDatosEvaluacion();
        }

        private void ActualizarDatosEvaluacion()
        {
            #region Resultados Datos Balance

            DatosEvaluacion.OoficDepositosCortoPlazo = OoficDepositosCortoPlazo * DatosEvaluacion.CoefObrasOficialesDepositosCortoPlazo;
            DatosEvaluacion.OoficDepositosLargoPlazo = OoficDepositosLargoPlazo * DatosEvaluacion.CoefObrasOficialesDepositosLargoPlazo;
            DatosEvaluacion.OoficInversionesCortoPlazo = OoficInversionesCortoPlazo * DatosEvaluacion.CoefObrasOficialesInversionesCortoPlazo;
            DatosEvaluacion.OoficInversionesLargoPlazo = OoficInversionesLargoPlazo * DatosEvaluacion.CoefObrasOficialesInversionesLargoPlazo;
            DatosEvaluacion.OoficOtrosConceptos = OoficOtrosConceptos * DatosEvaluacion.CoefObrasOficialesOtrosConceptos;
            DatosEvaluacion.OpartDepositosCortoPlazo = OpartDepositosCortoPlazo * DatosEvaluacion.CoefObrasParticularesDepositosCortoPlazo;
            DatosEvaluacion.OpartDepositosLargoPlazo = OpartDepositosLargoPlazo * DatosEvaluacion.CoefObrasParticularesDepositosLargoPlazo;
            DatosEvaluacion.OpartInversionesCortoPlazo = OpartInversionesCortoPlazo * DatosEvaluacion.CoefObrasParticularesInversionesCortoPlazo;
            DatosEvaluacion.OpartInversionesLargoPlazo = OpartInversionesLargoPlazo * DatosEvaluacion.CoefObrasParticularesInversionesLargoPlazo;
            DatosEvaluacion.OpartInversiones = OpartInversiones * DatosEvaluacion.CoefObrasParticularesInversiones;
            DatosEvaluacion.OpartOtrosConceptos = OpartOtrosConceptos * DatosEvaluacion.CoefObrasParticularesOtrosConceptos;
            DatosEvaluacion.DispCajayBancos = DispCajayBancos * DatosEvaluacion.CoefDisponibilidadesCajaYBancos;
            DatosEvaluacion.BusoInversiones = BusoInversiones * DatosEvaluacion.CoefBienesDeUsoInversiones;
            DatosEvaluacion.BusoMaqUtilAfec = BusoMaqUtilAfec * DatosEvaluacion.CoefBienesDeUsoEquiposAfectados;
            DatosEvaluacion.BusoMaqUtilNoAfec = BusoMaqUtilNoAfec * DatosEvaluacion.CoefBienesDeUsoEquiposNoAfectados;
            DatosEvaluacion.BusoBienesRaicesAfec = BusoBienesRaicesAfec * DatosEvaluacion.CoefBienesDeUsoInmueblesAfectados;
            DatosEvaluacion.BusoBienesRaicesNoAfec = BusoBienesRaicesNoAfec * DatosEvaluacion.CoefBienesDeUsoInmueblesNoAfectados;
            DatosEvaluacion.BusoOtrosConceptos = BusoOtrosConceptos * DatosEvaluacion.CoefBienesDeUsoOtrosConceptos;
            DatosEvaluacion.BcamMateriales = BcamMateriales * DatosEvaluacion.CoefBienesDeCambioMaterialesEnDeposito;
            DatosEvaluacion.BcamOtrosConceptos = BcamOtrosConceptos * DatosEvaluacion.CoefBienesDeCambioOtrosConceptos;
            DatosEvaluacion.DeuCortoPlazo = DeuCortoPlazo * DatosEvaluacion.CoefDeudasCortoPlazo;
            DatosEvaluacion.DeuLargoPlazo = DeuLargoPlazo * DatosEvaluacion.CoefDeudasLargoPlazo;
            #endregion
        }
        public void ActualizarCalculos()
        {

            ActualizarDatosEvaluacion();

            #region Capital Real Especifico y Capacidad Economica
            ValoresCalculados.CapitalRealEspecifico = this.ActivoTotal - this.PasivoTotal;
            ValoresCalculados.CapitalRealEspecificoEvaluar = DatosEvaluacion.ActivoTotal_Evaluar - DatosEvaluacion.PasivoTotal_Evaluar;
            ValoresCalculados.CapacidadEconomicaArqRep = ValoresCalculados.CapitalRealEspecificoEvaluar * DatosEvaluacion.CoefCapacidadEconomicaArqRep;
            ValoresCalculados.CapacidadEconomicaByC = ValoresCalculados.CapitalRealEspecificoEvaluar * DatosEvaluacion.CoefCapacidadEconomicaByC;
            ValoresCalculados.CapacidadEconomicaAyC = ValoresCalculados.CapitalRealEspecificoEvaluar * DatosEvaluacion.CoefCapacidadEconomicaAyC;
            #endregion

            #region Capacidad Técnica por Equipo


            foreach (var item in lstCapacidadTecnicaxEquipo)
            {
                item.Importe = this.TotalEquiposAfecEvaluar * item.CoefCapacTecnicaxEquipo;
                item.ImporteResultante = ValoresCalculados.CapitalRealEspecificoEvaluar * item.MultiplicadorCapEconomica.GetValueOrDefault() * 0.8m;
            }

            #endregion

            #region Capacidad de produccion (Indices)

            // Indices de Solvencia y Liquidez
            ValoresCalculados.IndiceSolvencia = 0;
            if (this.PasivoTotal > 0)
                ValoresCalculados.IndiceSolvencia = this.ActivoTotal / this.PasivoTotal;

            ValoresCalculados.IndiceLiquidez = 0;
            if (this.PasivoCorrienteTotal > 0)
                ValoresCalculados.IndiceLiquidez = this.ActivoCorrienteTotal / this.PasivoCorrienteTotal;

            //Resultante del indice de solvencia
            //De acuerdo con el valor resultante del índice se adoptarán los siguientes coeficientes parciales: 
            //Índice 2 o más ............................................... Coeficiente 2
            //Índice 1 o menos............................................Coeficiente 0

            //Entre ambos límites el coeficiente se obtendrá por interpolación lineal recta.
            ValoresCalculados.IndiceSolvenciaResultante = (ValoresCalculados.IndiceSolvencia > 2 ? 2 : (ValoresCalculados.IndiceSolvencia <= 1 ? 0 : GetIndiceSolvenciaByInterpolacion(ValoresCalculados.IndiceSolvencia)));


            // Resultante del índice de Liquidez
            //Índice 1,75 o más ............................................... Coeficiente 2
            //Índice 1 o menos ................................................Coeficiente 0

            //Entre ambos límites el coeficiente se obtendrá por interpolación lineal recta.
            ValoresCalculados.IndiceLiquidezResultante = (ValoresCalculados.IndiceLiquidez >= 1.75m ? 2 : (ValoresCalculados.IndiceLiquidez <= 1 ? 0 : GetIndiceLiquidezByInterpolacion(ValoresCalculados.IndiceLiquidez)));
            //--

            //Indice de Antigüedad en el país
            ValoresCalculados.IndiceAntiguedadEmpresa = ValoresCalculados.AniosAntiguedadEmpresa * 0.1m;

            if (ValoresCalculados.IndiceAntiguedadEmpresa > 1)
                ValoresCalculados.IndiceAntiguedadEmpresa = 1;
            //-

            //Indice Relacion Equipos
            ValoresCalculados.IndiceRelacionEquipos = 0;
            if (this.ValoresCalculados.CapitalRealEspecifico > 0)
                ValoresCalculados.IndiceRelacionEquipos = Math.Round(this.TotalEquiposAfecEvaluar.GetValueOrDefault() / ValoresCalculados.CapitalRealEspecificoEvaluar,2);


            if (ValoresCalculados.IndiceRelacionEquipos < 0.3m)
                ValoresCalculados.IndiceRelacionEquiposResultante = 0;
            else if (ValoresCalculados.IndiceRelacionEquipos < 0.5m)
                ValoresCalculados.IndiceRelacionEquiposResultante = 0.25m;
            else if (ValoresCalculados.IndiceRelacionEquipos < 0.8m)
                ValoresCalculados.IndiceRelacionEquiposResultante = 0.5m;
            else if (ValoresCalculados.IndiceRelacionEquipos >= 0.8m)
                ValoresCalculados.IndiceRelacionEquiposResultante = 1m;

            //--

            ValoresCalculados.Factor = (ValoresCalculados.IndiceSolvenciaResultante + ValoresCalculados.IndiceLiquidezResultante) / 2
                                        + ValoresCalculados.IndiceAntiguedadEmpresa + ValoresCalculados.IndiceRelacionEquiposResultante;



            foreach (var item in lstCapacidadProduccion)
            {
                item.ImporteResultante = item.Importe * ValoresCalculados.Factor;
            }

            #endregion

            #region Capacidad de Ejecucion
            foreach (var item in lstCapacidadEjecucion)
            {
                var itemSeccion = _lstSecciones.FirstOrDefault(x => x.IdSeccion == item.IdSeccion);
                if (itemSeccion != null && item.ExistsInForm1)
                    item.CapacidadEconomica = ValoresCalculados.CapitalRealEspecificoEvaluar * itemSeccion.MultiplicadorCapEconomica;

                var itemCapacidadProduccion = lstCapacidadProduccion.FirstOrDefault(x => x.IdSeccion == item.IdSeccion);

                if (itemCapacidadProduccion != null && itemCapacidadProduccion.ImporteResultante.HasValue)
                    item.CapacidadProduccion = itemCapacidadProduccion.ImporteResultante;

                item.CapacidadEconomica080 = (item.CapacidadEconomica * 0.8m);
                item.CapacidadProduccion020 = (item.CapacidadProduccion * 0.2m);
                item.CapacidadEjecucion = item.CapacidadEconomica080.GetValueOrDefault() + item.CapacidadProduccion020.GetValueOrDefault();
                item.CoeficienteConceptual = _CoeficienteConceptual ?? 1;
                item.CapacidadEjecucionAnual = item.CapacidadEjecucion * item.CoeficienteConceptual;
                item.CoeficienteEstadoDeudaBCRA = this.CoeficienteEstadoDeudaBCRA;
                item.CapacidadEjecucionFinal = item.CapacidadEjecucionAnual * CoeficienteEstadoDeudaBCRA;
            }
            #endregion

            #region Capacidad Técnica Hoja 2

            var lstSumaPorSeccion = lstCapacidadTecnica
               .GroupBy(item => item.IdSeccion) // Agrupar por Idseccion
               .Select(grupo => new
               {
                   IdSeccion = grupo.Key,
                   Monto = grupo.Sum(item => item.CapacidadTecnica) / 4 ?? 0
               }).ToList(); 

            foreach (var item in lstCapacidadTecnica)
            {
                var itemCapTec = lstCapacidadTecnicaxEquipo.FirstOrDefault(x => x.IdSeccion == item.IdSeccion);
                if (itemCapTec != null)
                    item.CapacidadTecnicaxEquipo = itemCapTec.Importe;

                item.CoerficienteConceptual = this.CoeficienteConceptual ?? 1;

                var itemPorSeccion = lstSumaPorSeccion.FirstOrDefault(x => x.IdSeccion == item.IdSeccion);
                if (itemPorSeccion != null)
                {
                    if (itemPorSeccion.Monto > (item.CapacidadTecnicaxEquipo ?? 0))
                        item.CapacidadTecnicaFinal = itemPorSeccion.Monto * (item.CoerficienteConceptual ?? 0);
                    else
                        item.CapacidadTecnicaFinal = (item.CapacidadTecnicaxEquipo ?? 0) * (item.CoerficienteConceptual ?? 0);
                }


            }
            #endregion

            #region Constancia de Capacidad


            decimal TotalComprometidoObrasEjecucion = 0;
            if (lstDetalleObrasEnEjecucion.Count > 0)
                TotalComprometidoObrasEjecucion = lstDetalleObrasEnEjecucion.Sum(s => s.MontoComprometido);

            foreach (var item in lstConstanciaCapacidad)
            {
                var itemCapacidadTecnica = lstCapacidadTecnica.FirstOrDefault(x => x.IdSeccion == item.IdSeccion);
                var itemCapacidadTecnicaComplementaria = lstCapacidadTecnicaxEquipo.FirstOrDefault(x => x.IdSeccion == item.IdSeccion && x.Rama == item.Rama);

                var itemEjecucionAnual = lstCapacidadEjecucion.FirstOrDefault(x => x.IdSeccion == item.IdSeccion);
                
                if (itemCapacidadTecnica != null && item.Rama != "C")
                {
                    item.CapacidadTecnica = itemCapacidadTecnica.CapacidadTecnicaFinal;
                    item.CapacidadTecnicaUVI = itemCapacidadTecnica.CapacidadTecnicaFinal * indiceUVI;
                }
                else if(itemCapacidadTecnicaComplementaria != null && item.Rama == "C")
                {
                    item.CapacidadTecnica = itemCapacidadTecnicaComplementaria.Importe * _CoeficienteConceptual;
                    item.CapacidadTecnicaUVI = item.CapacidadTecnica * indiceUVI;
                }
                if (itemEjecucionAnual != null && itemEjecucionAnual.CapacidadEjecucionFinal > 0)
                {
                    item.EjecucaionAnual = itemEjecucionAnual.CapacidadEjecucionFinal;
                    item.EjecucaionAnualUVI = itemEjecucionAnual.CapacidadEjecucionFinal * indiceUVI;


                    item.CapacidadContratacion = itemEjecucionAnual.CapacidadEjecucionFinal - TotalComprometidoObrasEjecucion;
                    item.CapacidadContratacionUVI = item.CapacidadContratacion * indiceUVI;
                }



            }
            #endregion

        }
        public decimal GetIndiceSolvenciaByInterpolacion(decimal indice)
        {
            decimal result = 0m;

            // Usar un ciclo for para iterar a través de los valores decimales
            for (decimal rangoActual = 1m; rangoActual <= 2m; rangoActual += 0.05m)
            {
                if (indice > rangoActual && indice <= rangoActual + 0.05m)
                {
                    result = (rangoActual + 0.05m) * 2 - 2;
                    break; // Salir del bucle una vez que se haya encontrado el rango
                }
            }
            if (indice <= 1)
                result = 0m;
            else if (indice > 2)
                result = 2m;

            return result;
        }
        public decimal GetIndiceLiquidezByInterpolacion(decimal indice)
        {
            decimal result = 0m;
            int vez = 0;
            // Usar un ciclo for para iterar a través de los valores decimales
            for (decimal rangoActual = 1m; rangoActual <= 1.7m; rangoActual += 0.05m)
            {
                vez++;
                if (indice > rangoActual && indice <= rangoActual + 0.05m)
                {
                    result = vez * 0.125m + 0.125m;
                    break; // Salir del bucle una vez que se haya encontrado el rango
                }
            }

            if (indice <= 1)
                result = 0m;
            else if (indice > 1.70m)
                result = 2m;

            return result;
        }
    }
    public class BalanceGeneral_DatosEvaluacion_Model
    {
        #region Coeficientes 
        public decimal CoefObrasOficialesDepositosCortoPlazo { get; set; } = 1.0m;
        public decimal CoefObrasOficialesDepositosLargoPlazo { get; set; } = 0.5m;
        public decimal CoefObrasOficialesInversionesCortoPlazo { get; set; } = 0.9m;
        public decimal CoefObrasOficialesInversionesLargoPlazo { get; set; } = 0.45m;
        public decimal CoefObrasOficialesOtrosConceptos { get; set; } = 0.6m;
        public decimal CoefObrasParticularesDepositosCortoPlazo { get; set; } = 0.9m;
        public decimal CoefObrasParticularesDepositosLargoPlazo { get; set; } = 0.45m;
        public decimal CoefObrasParticularesInversionesCortoPlazo { get; set; } = 0.8m;
        public decimal CoefObrasParticularesInversionesLargoPlazo { get; set; } = 0.4m;
        public decimal CoefObrasParticularesInversiones { get; set; } = 0.9m;
        public decimal CoefObrasParticularesOtrosConceptos { get; set; } = 0.6m;
        public decimal CoefDisponibilidadesCajaYBancos { get; set; } = 1.0m;
        public decimal CoefBienesDeUsoInversiones { get; set; } = 0.45m;
        public decimal CoefBienesDeUsoEquiposAfectados { get; set; } = 0.9m;
        public decimal CoefBienesDeUsoEquiposNoAfectados { get; set; } = 0.5m;
        public decimal CoefBienesDeUsoInmueblesAfectados { get; set; } = 0.7m;
        public decimal CoefBienesDeUsoInmueblesNoAfectados { get; set; } = 0.5m;
        public decimal CoefBienesDeUsoOtrosConceptos { get; set; } = 0.6m;
        public decimal CoefBienesDeCambioMaterialesEnDeposito { get; set; } = 0.70m;
        public decimal CoefBienesDeCambioOtrosConceptos { get; set; } = 0.6m;
        public decimal CoefDeudasCortoPlazo { get; set; } = 1.0m;
        public decimal CoefDeudasLargoPlazo { get; set; } = 0.5m;
        // Coeficientes Capacidad Económica
        public decimal CoefCapacidadEconomicaArqRep { get; set; } = 10;
        public decimal CoefCapacidadEconomicaByC { get; set; } = 4;
        public decimal CoefCapacidadEconomicaAyC { get; set; } = 7;
        #endregion

        #region Datos Balance luego de la multiplicacion de los coeficientes
        public decimal? OoficDepositosCortoPlazo { get; set; }
        public decimal? OoficDepositosLargoPlazo { get; set; }
        public decimal? OoficInversionesCortoPlazo { get; set; }
        public decimal? OoficInversionesLargoPlazo { get; set; }
        public decimal? OoficOtrosConceptos { get; set; }
        public decimal? OpartDepositosCortoPlazo { get; set; }
        public decimal? OpartDepositosLargoPlazo { get; set; }
        public decimal? OpartInversionesCortoPlazo { get; set; }
        public decimal? OpartInversionesLargoPlazo { get; set; }
        public decimal? OpartInversiones { get; set; }
        public decimal? OpartOtrosConceptos { get; set; }
        public decimal? DispCajayBancos { get; set; }
        public decimal? BusoInversiones { get; set; }
        public decimal? BusoMaqUtilAfec { get; set; }
        public decimal? BusoMaqUtilNoAfec { get; set; }
        public decimal? BusoBienesRaicesAfec { get; set; }
        public decimal? BusoBienesRaicesNoAfec { get; set; }
        public decimal? BusoOtrosConceptos { get; set; }
        public decimal? BcamMateriales { get; set; }
        public decimal? BcamOtrosConceptos { get; set; }
        public decimal? DeuCortoPlazo { get; set; }
        public decimal? DeuLargoPlazo { get; set; }
        public decimal ActivoTotal_Evaluar
        {
            get
            {
                return ActivoCorrienteTotal_Evaluar + ActivoNoCorrienteTotal_Evaluar;
            }
        }
        public decimal ActivoCorrienteTotal_Evaluar
        {
            get
            {
                decimal result = 0;
                result = OoficDepositosCortoPlazo.GetValueOrDefault() +
                        OoficInversionesCortoPlazo.GetValueOrDefault() +
                        OoficOtrosConceptos.GetValueOrDefault() +
                        OpartDepositosCortoPlazo.GetValueOrDefault() +
                        OpartInversionesCortoPlazo.GetValueOrDefault() +
                        OpartInversiones.GetValueOrDefault() +
                        OpartOtrosConceptos.GetValueOrDefault() +
                        DispCajayBancos.GetValueOrDefault() +
                        BcamMateriales.GetValueOrDefault() +
                        BcamOtrosConceptos.GetValueOrDefault();
                return result;
            }
        }
        public decimal ActivoNoCorrienteTotal_Evaluar
        {
            get
            {
                decimal result = 0;
                result = OoficDepositosLargoPlazo.GetValueOrDefault() +
                        OoficInversionesLargoPlazo.GetValueOrDefault() +
                        OpartDepositosLargoPlazo.GetValueOrDefault() +
                        OpartInversionesLargoPlazo.GetValueOrDefault() +
                        BusoInversiones.GetValueOrDefault() +
                        BusoMaqUtilAfec.GetValueOrDefault() +
                        BusoMaqUtilNoAfec.GetValueOrDefault() +
                        BusoBienesRaicesAfec.GetValueOrDefault() +
                        BusoBienesRaicesNoAfec.GetValueOrDefault() +
                        BusoOtrosConceptos.GetValueOrDefault();
                return result;
            }
        }
        public decimal PasivoTotal_Evaluar
        {
            get
            {
                return PasivoCorrienteTotal_Evaluar + PasivoNoCorrienteTotal_Evaluar;
            }
        }
        public decimal PasivoCorrienteTotal_Evaluar
        {
            get
            {
                decimal result = 0;
                result = DeuCortoPlazo.GetValueOrDefault();
                return result;
            }
        }
        public decimal PasivoNoCorrienteTotal_Evaluar
        {
            get
            {
                decimal result = 0;
                result = DeuLargoPlazo.GetValueOrDefault();
                return result;
            }
        }
        #endregion
    }

    public class BalanceGeneral_ValoresCalculados_Model
    {
        // Capital Real Especifico y Capacidad Economica
        public decimal CapitalRealEspecifico { get; set; }
        public decimal CapitalRealEspecificoEvaluar { get; set; }
        public decimal CapacidadEconomicaArqRep { get; set; }
        public decimal CapacidadEconomicaByC { get; set; }
        public decimal CapacidadEconomicaAyC { get; set; }

        //Indices
        public decimal IndiceSolvencia { get; set; }
        public decimal IndiceSolvenciaResultante { get; set; }
        public decimal IndiceLiquidez { get; set; }
        public decimal IndiceLiquidezResultante { get; set; }
        public int AniosAntiguedadEmpresa { get; set; }
        public decimal IndiceAntiguedadEmpresa { get; set; }
        public decimal IndiceRelacionEquipos { get; set; }
        public decimal IndiceRelacionEquiposResultante { get; set; }
        public decimal Factor { get; set; }

    }
    public class BalanceGeneral_CapTecxEquipoItem_Evaluar_Model
    {
        public int IdEspecialidad { get; set; }
        public string? Rama { get; set; }
        public int IdSeccion { get; set; }
        public string? DescripcionSeccion { get; set; }
        public decimal? Importe { get; set; }
        public decimal? CoefCapacTecnicaxEquipo { get; set; }
        public decimal? MultiplicadorCapEconomica { get; set; }
        public decimal? ImporteResultante { get; set; }

    }
    public class BalanceGeneral_CapacidadEjecucionItem_Evaluar_Model
    {
        public int IdSeccion { get; set; }
        public string? DescripcionSeccion { get; set; }
        public decimal? CapacidadEconomica { get; set; }
        public decimal? CapacidadProduccion { get; set; }
        public decimal? CapacidadEconomica080 { get; set; }
        public decimal? CapacidadProduccion020 { get; set; }
        public decimal? CapacidadEjecucion { get; set; }
        public decimal? CoeficienteConceptual { get; set; }
        public decimal? CapacidadEjecucionAnual { get; set; }
        public decimal? CoeficienteEstadoDeudaBCRA { get; set; }
        public decimal? CapacidadEjecucionFinal { get; set; }
        public bool ExistsInForm1 { get; set; }
    }
    public class BalanceGeneral_CapTecItem_Evaluar_Model
    {
        public int IdSeccion { get; set; }
        public string Rama { get; set; }
        public string? DescripcionSeccion { get; set; }
        public decimal? Monto { get; set; }
        public int? Mes { get; set; }
        public int? Anio { get; set; }
        public string Periodo
        {
            get
            {
                string res = string.Empty;
                if(Mes > 0 && Anio > 0)
                    res = Mes.ToString().PadLeft(2, Convert.ToChar("0")) + "/" + Anio.ToString();
                return res;
            }
        }
        public decimal? CoefActualizacion { get; set; } 
        public decimal? MontoActualizado
        {
            get
            {
                return Monto * CoefActualizacion;
            }
        }
        public decimal? CoefTipoObra { get; set; } 
        public decimal? CapacidadTecnica
        {
            get
            {
                return MontoActualizado * CoefTipoObra;
            }
        }
        public decimal? CapacidadTecnicaxEquipo { get; set; }
        public decimal? CoerficienteConceptual { get; set; }
        public decimal CapacidadTecnicaFinal { get; set; }

    }
    public class BalanceGeneral_ConstanciaCapacidadItem_Evaluar_Model
    {
        public int IdSeccion { get; set; }
        public string? Rama { get; set; }
        public string? DescripcionSeccion { get; set; }
        public decimal? CapacidadTecnica { get; set; }
        public decimal? CapacidadTecnicaUVI { get; set; }
        public decimal? CapacidadContratacion { get; set; }
        public decimal? CapacidadContratacionUVI { get; set; }
        public decimal? EjecucaionAnual { get; set; }
        public decimal? EjecucaionAnualUVI { get; set; }

    }
    public class BalanceGeneral_ObrasEjecucionItem_Evaluar_Model
    {
        public int IdObraPciaLp { get; set; }
        public DateTime FechaCertificacion { get; set; }
        public string? ObraNombre { get; set; }
        public string? Comitente { get; set; }
        public string? PeríodoBase { get; set; }
        public DateTime FechaInicio { get; set; }
        public decimal TotalContratado { get; set; }
        public decimal TotalCertificado { get; set; }
        public decimal Saldo
        {
            get
            {
                return TotalContratado - TotalCertificado;
            }
        }
        public int Plazo { get; set; }
        public int DiasTranscurridos { get; set; }
        public int SaldoDias
        {
            get
            {
                return Plazo - DiasTranscurridos;
            }
        }

        public decimal MontoMensual { get; set; }
        public decimal MontoAnual { get; set; }
        public decimal CoefCertificado { get; set; }
        public decimal PorcentajeCertificado { get; set; }
        public decimal PorcentajeTiempo { get; set; }
        public decimal CoefMatriz { get; set; }
        public decimal CoefParticipacion { get; set; }
        public decimal MontoComprometido { get; set; }

    }
       
}


