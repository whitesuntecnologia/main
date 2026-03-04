using Ardalis.GuardClauses;
using AutoMapper;
using Business.Interfaces;
using DataAccess;
using DataAccess.Entities;
using DataTransferObject.BLs;
using DataTransferObject;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork;
using Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Linq.Dynamic.Core;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
using StaticClass;

namespace Business
{
    public class TramitesEvaluacionBL: ITramitesEvaluacionBL
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IMapper _mapper;
        private readonly IUsuariosBL _usuarioBL;
        private readonly ITablasBL _tablasBL;
        
        public TramitesEvaluacionBL(IUnitOfWorkFactory uowFactory, IUsuariosBL usuarioBL, ITablasBL tablasBL)
        {
            _uowFactory = Guard.Against.Null(uowFactory);
            _usuarioBL = usuarioBL;
            _tablasBL = tablasBL;

            var config = new MapperConfiguration(cfg =>
            {
                #region Generales
                cfg.CreateMap<TramitesFormulariosEvaluados, TramiteFormularioEvaluadoDTO>();
                cfg.CreateMap< TramiteFormularioEvaluadoDTO, TramitesFormulariosEvaluados>()
                    .ForMember(dest=> dest.CreateDate, opt => opt.Ignore())
                ;
                cfg.CreateMap<TramitesBalancesGeneralesEvaluar, TramitesBalanceGeneralEvaluarDTO>().ReverseMap();
                cfg.CreateMap<TramitesBalancesGeneralesEvaluarCapProd, TramitesBalancesGeneralesEvaluarCapProdDTO>().ReverseMap();
                cfg.CreateMap<TramitesBalancesGeneralesEvaluarCapTecnica, TramitesBalancesGeneralesEvaluarCapTecnicaDTO>().ReverseMap();
                cfg.CreateMap<TramitesBalancesGeneralesEvaluarCapTecxEquipo, TramitesBalancesGeneralesEvaluarCapTecxEquipoDTO>().ReverseMap();
                cfg.CreateMap<TramitesBalancesGeneralesEvaluarConstanciaCap, TramitesBalancesGeneralesEvaluarConstanciaDTO>().ReverseMap();
                cfg.CreateMap<TramitesBalancesGeneralesEvaluarCapEjec, TramitesBalancesGeneralesEvaluarCapEjecDTO>().ReverseMap();
                cfg.CreateMap<TramitesBalancesGeneralesEvaluarDetalleObrasEjecucion, TramitesBalancesGeneralesEvaluarDetalleObrasEjecucionDTO>().ReverseMap();
                cfg.CreateMap<EspecialidadesSecciones, ItemGrillaBGECapacidadTecnicaxEquipoDTO>()
                    .ForMember(dest => dest.Rama, opt => opt.MapFrom(src => src.IdEspecialidadNavigation.Rama))
                ;

                cfg.CreateMap<ObrasProvinciaLaPampa, ObrasProvinciaLaPampaDTO>().ReverseMap();
                #endregion

            });
            _mapper = config.CreateMapper();
        }
        public async Task<TramiteFormularioEvaluadoDTO> GetEvaluacionAsync(int IdTramite, int Nronotificacion, int NroFormulario)
        {
            TramiteFormularioEvaluadoDTO result = null;
            using var uow = _uowFactory.GetUnitOfWork();

            var _repoFormulariosEvaluados = new TramitesFormulariosEvaluadosRepository(uow);
            var entity = await _repoFormulariosEvaluados.FirstOrDefaultAsync(x => x.IdTramite == IdTramite 
                                                                && x.NroNotificacion == Nronotificacion
                                                                && x.NroFormulario == NroFormulario);
            
            result = _mapper.Map<TramitesFormulariosEvaluados, TramiteFormularioEvaluadoDTO>(entity);

            return result;
        }
        public async Task<int> GetNumeroNotificacionActualAsync(int IdTramite)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            
            var repo = new TramitesHistorialEstadosRepository(uow);
            int NroNotificacion = await repo.Where(x => x.IdTramite == IdTramite && x.CodEstadoNuevo == "EVAL").CountAsync();
            
            return NroNotificacion;
        }
        public async Task<TramiteFormularioEvaluadoDTO> GuardarEvaluacionAsync(TramiteFormularioEvaluadoDTO dto)
        {

            TramiteFormularioEvaluadoDTO result = null;
            string userid = await _usuarioBL.GetCurrentUserid();
            using var uow = _uowFactory.GetUnitOfWork();

            var _repoFormulariosEvaluados = new TramitesFormulariosEvaluadosRepository(uow);

            
            var entity = await _repoFormulariosEvaluados.FirstOrDefaultAsync(x => x.IdTramite == dto.IdTramite 
                                            && x.NroNotificacion == dto.NroNotificacion                                                
                                            && x.NroFormulario == dto.NroFormulario);

            if (entity == null)
            {
                entity = _mapper.Map<TramiteFormularioEvaluadoDTO, TramitesFormulariosEvaluados>(dto);
                entity.CreateDate = DateTime.Now;
                entity.CreateUser = userid;
                await _repoFormulariosEvaluados.AddAsync(entity);
            }
            else
            {

                _mapper.Map<TramiteFormularioEvaluadoDTO, TramitesFormulariosEvaluados>(dto,entity);
                entity.LastUpdateDate = DateTime.Now;
                entity.LastUpdateUser = userid;
                await _repoFormulariosEvaluados.UpdateAsync(entity);
            }

            result = _mapper.Map<TramitesFormulariosEvaluados, TramiteFormularioEvaluadoDTO>(entity);

            return result;
        }
        public async Task ActualizarEquiposMontoRealizacionEvaluador(int IdTramite, decimal MontoRealizacionAfectado, decimal? MontoRealizacionNoAfectado)
        {
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new TramitesEquiposRepository(uow);
                    var repobalances = new TramitesBalancesGeneralesRepository(uow);

                    var lstEquiposEntity = await repo.Where(x => x.IdTramite == IdTramite).ToListAsync();
                    var entityBalance = await repobalances.FirstOrDefaultAsync(x => x.IdTramite == IdTramite);

                    foreach (var item in lstEquiposEntity)
                    {
                        if (item.Afectado)
                        {
                            item.MontoRealizacionEvaluador = MontoRealizacionAfectado;
                            entityBalance.BusoMaqUtilAfec = MontoRealizacionAfectado;
                        }
                        else
                        {
                            item.MontoRealizacionEvaluador = MontoRealizacionNoAfectado;
                            entityBalance.BusoMaqUtilNoAfec = MontoRealizacionNoAfectado;
                        }
                        await repo.UpdateAsync(item);
                        await repobalances.UpdateAsync(entityBalance);
                    }
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }
        public async Task ActualizarBienesRaicesMontoRealizacionEvaluador(int IdTramite, decimal MontoRealizacionAfectado, decimal? MontoRealizacionNoAfectado)
        {
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {

              
                var repo = new TramitesBienesRaicesRepository(uow);
                var repobalances = new TramitesBalancesGeneralesRepository(uow);
                var lstBienesEntity = await repo.Where(x => x.IdTramite == IdTramite).ToListAsync();
                var entityBalance = await repobalances.FirstOrDefaultAsync(x => x.IdTramite == IdTramite);

                    foreach (var item in lstBienesEntity)
                    {
                        if (item.Afectado)
                        {
                            item.MontoRealizacionEvaluador = MontoRealizacionAfectado;
                            entityBalance.BusoBienesRaicesAfec = MontoRealizacionAfectado;
                        }
                        else
                        {
                            item.MontoRealizacionEvaluador = MontoRealizacionNoAfectado;
                            entityBalance.BusoBienesRaicesNoAfec = MontoRealizacionNoAfectado;
                        }

                        await repo.UpdateAsync(item);
                        await repobalances.UpdateAsync(entityBalance);
                    }
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }
        public async Task<bool> isAssignedAsync(int IdTramite, string userid)
        {
            bool result = false;
            
            using var uow = _uowFactory.GetUnitOfWork();
            var repoTramites = new TramitesRepository(uow);
            var entity = await repoTramites.FirstOrDefaultAsync(x=> x.IdTramite == IdTramite);

            result = (entity != null && entity.UsuarioAsignado == userid);

            return result;
        }
        public async Task<int> GetEstadoDeudaBCRAAsync(int IdTramite)
        {

            using var uow = _uowFactory.GetUnitOfWork();
            var repoTramites = new TramitesRepository(uow);
            var repoTramitesInfEmpDeudas = new TramitesInfEmpDeudasRepository(uow);
            var tramiteEntity = await repoTramites.FirstOrDefaultAsync(x => x.IdTramite == IdTramite);

            if (tramiteEntity == null)
            {
                throw new InvalidOperationException($"No se encontró el trámite con Id {IdTramite}");
            }

            var lstDeudaBCRA = tramiteEntity.IdTipoTramiteNavigation.IdGrupoTramite == Constants.GruposDeTramite.RegistroLicitadores
                                ? tramiteEntity.TramitesInfEmp?.TramitesInfEmpDeudas?.Select(s => new { Situacion = s.Situacion, Monto = s.Monto })
                                : tramiteEntity.TramitesInfEmpCons?.TramitesInfEmpConsDeudas?.Select(s => new { Situacion = s.Situacion, Monto = s.Monto })
                                ;

            decimal[] arrSituaciones = new decimal[4];
            decimal[] arrPorcentajes = new decimal[4];
            decimal TotalDeuda = 0m;
            int EstadoDeuda = 1;

            if (lstDeudaBCRA != null)
            {
                foreach (var item in lstDeudaBCRA)
                {
                    switch (item.Situacion)
                    {
                        case 1:
                            arrSituaciones[0] = item.Monto;
                            break;
                        case 2:
                            arrSituaciones[1] = item.Monto;
                            break;
                        case 3:
                            arrSituaciones[2] = item.Monto;
                            break;
                        default:
                            arrSituaciones[3] = item.Monto;
                            break;
                    }
                    TotalDeuda += item.Monto;
                }
            }

            if (TotalDeuda > 0)
            {
                arrPorcentajes[0] = arrSituaciones[0] / TotalDeuda * 100.0m;
                arrPorcentajes[1] = arrSituaciones[1] / TotalDeuda * 100.0m;
                arrPorcentajes[2] = arrSituaciones[2] / TotalDeuda * 100.0m;
                arrPorcentajes[3] = arrSituaciones[3] / TotalDeuda * 100.0m;

                if (arrSituaciones[3] > 0)
                    EstadoDeuda = 4;
                else if (arrPorcentajes[0] >= 90 && arrPorcentajes[2] <= 1m)
                {
                    EstadoDeuda = 1;
                }
                else if (arrPorcentajes[0] >= 60 && arrPorcentajes[2] <= 5m)
                {
                    EstadoDeuda = 2;
                }
                else if (arrPorcentajes[0] >= 40 && arrPorcentajes[2] <= 5m)
                {
                    EstadoDeuda = 3;
                }
                else
                    EstadoDeuda = 4;
            }
            return EstadoDeuda;
        }
        public async Task<List<ItemGrillaBGECapacidadTecnicaxEquipoDTO>> GetGrillaBGECapacidadTecnicaxEquipoAsync(int IdTramite)
        {
            List<ItemGrillaBGECapacidadTecnicaxEquipoDTO> lstResult;
            using var uow = _uowFactory.GetUnitOfWork();

            var repoEspecialidadesSecciones = new EspecialidadesSeccionesRepository(uow);
            var repoTramiteEspecialidadesSecciones = new TramitesEspecialidadesSeccionesRepository(uow);

            var lstSecciones = await repoEspecialidadesSecciones.Where(x => !x.Baja).ToListAsync();
            var lstSeccionesTramite = await repoTramiteEspecialidadesSecciones.Where(x => x.IdTramiteEspecialidadNavigation.IdTramite == IdTramite).ToListAsync();
            
            lstResult = _mapper.Map<List<EspecialidadesSecciones>, List<ItemGrillaBGECapacidadTecnicaxEquipoDTO>>(lstSecciones);

            // Agrega las secciones del tramite que no estan en la lista de secciones 
            //Esto solo agrega las secciones de la especialidad Obras complementarias RAMA C,
            //que permite agregar secciones de las RAMAS A y B.
            foreach (var itemSeccion in lstSeccionesTramite)
            {
                var seccion = lstSecciones.FirstOrDefault(x => x.IdEspecialidad == itemSeccion.IdTramiteEspecialidadNavigation.IdEspecialidad 
                                                            && x.IdSeccion == itemSeccion.IdSeccion);
                if (seccion == null)
                    lstResult.Add(new ItemGrillaBGECapacidadTecnicaxEquipoDTO
                    {
                        Rama = itemSeccion.IdTramiteEspecialidadNavigation.IdEspecialidadNavigation.Rama,
                        IdEspecialidad = itemSeccion.IdTramiteEspecialidadNavigation.IdEspecialidad,
                        IdSeccion = itemSeccion.IdSeccion,
                        DescripcionSeccion = "Comp. " + itemSeccion.IdSeccionNavigation.DescripcionSeccion,
                        CoefCapacTecnicaxEquipo = Constants.CoefCapacTecnicaxEquipoRamaC,
                        MultiplicadorCapEconomica = itemSeccion.IdSeccionNavigation.MultiplicadorCapEconomica
                    });
            }

            //Para las que se son Especialidad Electromecánica exclusivamente se multiplica x 4,
            //pero si tiene alguna especialidad mmás, se multiplica x 1.5 tanto sea en la RAMA A como en la C
            var lstIdsEspecialidadesCTxE = lstResult.Select(s => s.IdEspecialidad).Distinct().ToList();
            if (lstIdsEspecialidadesCTxE.Exists(x => x == 2) && lstIdsEspecialidadesCTxE.Count > 1)
            {
                var lstActualizarMultiplicador = lstResult.Where(x => (x.IdSeccion == 3 || x.IdSeccion == 4) && x.Rama != "C").ToList();
                foreach (var item in lstActualizarMultiplicador)
                {
                    item.CoefCapacTecnicaxEquipo = 1.5m;
                }
            }

            return lstResult;
        }
        public async Task<List<ItemGrillaBGECapacidadTecnica2DTO>> GetGrillaCapacidadTecnica2(int IdTramite )
        {
            List<ItemGrillaBGECapacidadTecnica2DTO> lstResult = new List<ItemGrillaBGECapacidadTecnica2DTO>();
            using var uow = _uowFactory.GetUnitOfWork();
            
            
            var _repoTramitesObras = new TramitesObrasRepository(uow);
            var _repoICC = new IndicesCostoConstruccionRepository(uow);
            var _repoBalances = new TramitesBalancesGeneralesRepository(uow);
            var _repoTramitesEspecialidadesSecciones = new TramitesEspecialidadesSeccionesRepository(uow);

            var ultimoBalance = await _repoBalances.Where(x => x.IdTramite == IdTramite).OrderByDescending( o=> o.Anio).FirstOrDefaultAsync();

            var lstObras = await _repoTramitesObras.Where(x => x.IdTramite == IdTramite).ToListAsync();
            //Obtiene las mejores 4 obras según su monto
            var lstMejoresObras = lstObras
                                .GroupBy(registro => registro.IdTramiteEspecialidadSeccionNavigation.IdSeccion) // Agrupar por IdSeccion
                                .SelectMany(grupo => grupo.OrderByDescending(registro => registro.Monto).Take(4))
                                .ToList(); // Ordenar y seleccionar los primeros 4 registros por grupo
;
           
            decimal TotalSeccion = 0;
            foreach (var obra in lstMejoresObras)
            {
                // Obtiene el Indice del costo de construcción desde la tablas de parametros 
                IndicesCostoConstruccion indiceConstruccionBase = null;
                IndicesCostoConstruccion indiceConstruccionBalance = null;
                decimal indice = 1m;
                int MesBase = 0;
                int AnioBase = 0;
                int MesBalance = 0;
                int AnioBalance = 0;
                if (!string.IsNullOrWhiteSpace(obra.PeriodoBase))
                {
                    MesBase = int.Parse(obra.PeriodoBase.Substring(0, 2));
                    AnioBase = int.Parse(obra.PeriodoBase.Substring(obra.PeriodoBase.Length - 4, 4));
                    indiceConstruccionBase = await _repoICC.FirstOrDefaultAsync(x => x.Mes == MesBase && x.Anio == AnioBase);
                }
                if(ultimoBalance != null)
                {
                    MesBalance = ultimoBalance.FechaBalance.Month;
                    AnioBalance = ultimoBalance.FechaBalance.Year;
                    indiceConstruccionBalance = await _repoICC.FirstOrDefaultAsync(x => x.Mes == MesBalance && x.Anio == AnioBalance);
                }

                if (indiceConstruccionBase != null && indiceConstruccionBalance != null && indiceConstruccionBase.Valor != 0m)
                    indice = indiceConstruccionBalance.Valor / indiceConstruccionBase.Valor;

                string DescripcionSeccion = obra.IdTramiteEspecialidadSeccionNavigation.IdSeccionNavigation.DescripcionSeccion;
                if(obra.IdTramiteEspecialidadSeccionNavigation.IdTramiteEspecialidadNavigation.IdEspecialidadNavigation.Rama == "C")
                    DescripcionSeccion = "Comp. " + DescripcionSeccion;

                var result = new ItemGrillaBGECapacidadTecnica2DTO()
                {
                    IdSeccion = obra.IdTramiteEspecialidadSeccionNavigation.IdSeccion,
                    DescripcionSeccion = DescripcionSeccion,
                    Mes = MesBase,
                    Anio = AnioBase,
                    Monto = obra.Monto,
                    CoefActualizacion = indice,
                    CoefTipoObra = obra.IdTipoObraNavigation.Coeficiente
                };
                lstResult.Add(result);
                TotalSeccion += obra.Monto;
            }

            var lstEspecialidadesSecciones = await _repoTramitesEspecialidadesSecciones
                                           .Where(x => x.IdTramiteEspecialidadNavigation.IdTramite == IdTramite)
                                           .ToListAsync();

            var lstIdsFaltanres = lstEspecialidadesSecciones.Select(s => s.IdSeccion).Except(lstResult.Select(s => s.IdSeccion)).ToList();
            lstResult.AddRange(lstEspecialidadesSecciones.Where(x => lstIdsFaltanres.Contains(x.IdSeccion)).Select(s => new ItemGrillaBGECapacidadTecnica2DTO
            {
                IdSeccion = s.IdSeccion,
                Rama = s.IdSeccionNavigation.IdEspecialidadNavigation.Rama,
                DescripcionSeccion = s.IdSeccionNavigation.DescripcionSeccion,
            }).ToList());

            return lstResult;
        }
        public async Task<decimal> GetPromedioCoeficienteConceptualAsync(int IdTramite)
        {
            //Retorna el promedio de los coeficientes conceptuales de las obras en ejecución.
            decimal result = 1m;
            using var uow = _uowFactory.GetUnitOfWork();
            
            var repotramites = new TramitesRepository(uow);
            var repoObrasLP = new ObrasProvinciaLaPampaRepository(uow);

            var tramiteEntity = await repotramites.FirstOrDefaultAsync(x => x.IdTramite == IdTramite);
            string CuitEmpresa = tramiteEntity.IdEmpresaNavigation.CuitEmpresa.ToString();
            var FechaPresentacion = await repotramites.GetFechaPresentacionAsync(IdTramite);

            var lstCoeficientes = await repoObrasLP.Where(x => x.CuitEmpresa == CuitEmpresa
                                && !x.BajaLogica
                                && x.FechaInformeCoeficiente.HasValue 
                                && x.FechaInformeCoeficiente.Value >= FechaPresentacion.AddMonths(-60) && x.FechaInformeCoeficiente.Value <= FechaPresentacion
                                && x.CoeficienteConceptual.HasValue)
                                .Select(s =>
                                    (s.CoeficienteConceptual.GetValueOrDefault() == 0 ? 1 : s.CoeficienteConceptual.GetValueOrDefault())
                                ).ToListAsync();

            if(lstCoeficientes.Any())
                result = lstCoeficientes.Average();

            return result;
        }

        public async Task<ObrasProvinciaLaPampaDTO> GetDatosObraLicitarAsync(int IdTramite)
        {
            //Retorna los datos de la obra a licitar
            using var uow = _uowFactory.GetUnitOfWork();
            var repoTramites = new TramitesRepository(uow);

            var tramiteEntity = await repoTramites.FirstOrDefaultAsync(x => x.IdTramite == IdTramite);

            if (tramiteEntity?.IdTipoTramite != Constants.TiposDeTramite.Reli_Licitar)
                throw new ArgumentException("Los datos de la obra solo se pueden obtener en el caso de un certificado para licitar.");

            if (tramiteEntity.IdObraPciaLpNavigation == null)
                throw new InvalidOperationException($"No se encontró la obra asociada al trámite {IdTramite}");

            var result = _mapper.Map<ObrasProvinciaLaPampaDTO>(tramiteEntity.IdObraPciaLpNavigation);

            return result;
        }

        public async Task<TramitesBalanceGeneralEvaluarDTO> InsertOrUpdateBalanceGeneralEvaluar(TramitesBalanceGeneralEvaluarDTO dto)
        {
            TramitesBalanceGeneralEvaluarDTO result = null;
            string userid = await _usuarioBL.GetCurrentUserid();
            TramitesBalancesGeneralesEvaluar entity = null;

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repoBalanceGeneralEvaluar = new TramitesBalancesGeneralesEvaluarRepository(uow);
                    var repoCapEjec = new TramitesBalancesGeneralesEvaluarCapEjecRepository(uow);
                    var repoCapProd = new TramitesBalancesGeneralesEvaluarCapProdRepository(uow);
                    var repoCapTecnica = new TramitesBalancesGeneralesEvaluarCapTecnicaRepository(uow);
                    var repoCapTecxEquipo = new TramitesBalancesGeneralesEvaluarCapTecxEquipoRepository(uow);
                    var repoConstanciaCap = new TramitesBalancesGeneralesEvaluarConstanciaCapRepository(uow);
                    var repoDetalleObrasEjec = new TramitesBalancesGeneralesEvaluarDetalleObrasEjecucionRepository(uow);
                    
                    if (dto.IdTramiteBalanceGeneralEvaluar == 0)
                    {
                        entity = _mapper.Map<TramitesBalancesGeneralesEvaluar>(dto);
                        entity.CreateUser = userid;
                        entity.CreateDate = DateTime.Now;
                        entity = await repoBalanceGeneralEvaluar.AddAsync(entity);
                    }
                    else
                    {
                        entity = await repoBalanceGeneralEvaluar.FirstOrDefaultAsync(x => x.IdTramiteBalanceGeneralEvaluar == dto.IdTramiteBalanceGeneralEvaluar);

                        
                        await repoCapEjec.RemoveRangeAsync(entity.TramitesBalancesGeneralesEvaluarCapEjec);
                        await repoCapProd.RemoveRangeAsync(entity.TramitesBalancesGeneralesEvaluarCapProd);
                        await repoCapTecnica.RemoveRangeAsync(entity.TramitesBalancesGeneralesEvaluarCapTecnica);
                        await repoCapTecxEquipo.RemoveRangeAsync(entity.TramitesBalancesGeneralesEvaluarCapTecxEquipo);
                        await repoConstanciaCap.RemoveRangeAsync(entity.TramitesBalancesGeneralesEvaluarConstanciaCap);
                        await repoDetalleObrasEjec.RemoveRangeAsync(entity.TramitesBalancesGeneralesEvaluarDetalleObrasEjecucion);
                        
                         _mapper.Map<TramitesBalanceGeneralEvaluarDTO, TramitesBalancesGeneralesEvaluar>(dto, entity);
                        entity.LastUpdateDate = DateTime.Now;
                        entity.LastUpdateUser = userid;
                        entity = await repoBalanceGeneralEvaluar.UpdateAsync(entity);
                    }

                    await uow.CommitAsync();
                }
                catch (Exception ex)
                {
                    uow.RollBack();
                    throw;
                }
                
            }

            result = _mapper.Map<TramitesBalanceGeneralEvaluarDTO>(entity);
            return result;
        }
        public async Task<TramitesBalanceGeneralEvaluarDTO> GetBalanceGeneralEvaluar(int IdTramiteBalanceGeneral)
        {
            TramitesBalanceGeneralEvaluarDTO result = null;
            var uow = _uowFactory.GetUnitOfWork();
            var repoBalanceGeneralEvaluar = new TramitesBalancesGeneralesEvaluarRepository(uow);
            var element = await repoBalanceGeneralEvaluar.FirstOrDefaultAsync(x => x.IdTramiteBalanceGeneral == IdTramiteBalanceGeneral);

            result = _mapper.Map<TramitesBalanceGeneralEvaluarDTO>(element);

            return result;
        }
        public async Task<decimal> GetCoeficienteMatrizObrasEjecAsync(decimal PorcentajeCertificado, decimal PorcentajeTiempo)
        {
            decimal result = 0;
            var uow = _uowFactory.GetUnitOfWork();
            var repo = new MatrizObrasEnEjecucionRepository(uow);

            var element = await repo.FirstOrDefaultAsync(x => PorcentajeCertificado >= x.PorcCertifificadoDesde 
                                                         && PorcentajeCertificado <= x.PorcCertifificadoHasta );

            int TiempoColumna = (int) Math.Floor(PorcentajeTiempo / 10) * 10;

            switch(TiempoColumna)
            {
                case 0: result = element.Tiempo0; break;
                case 10: result = element.Tiempo10; break;
                case 20: result = element.Tiempo20; break;
                case 30: result = element.Tiempo30; break;
                case 40: result = element.Tiempo40; break;
                case 50: result = element.Tiempo50; break;
                case 60: result = element.Tiempo60; break;
                case 70: result = element.Tiempo70; break;
                case 80: result = element.Tiempo80; break;
                case 90: result = element.Tiempo90; break;
                case >=100: result = element.Tiempo100; break;
            }

            return result;
        }
        public async Task ActualizarEvaluadorAsignadoAsync(int IdTramite, string useridNuevoEvaludor)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            string userid = await _usuarioBL.GetCurrentUserid();

            var repo = new TramitesRepository(uow);
            var tramiteEntity = await repo.FirstOrDefaultAsync(x => x.IdTramite == IdTramite);

            if(tramiteEntity != null)
            {
                tramiteEntity.UsuarioAsignado = useridNuevoEvaludor;
                tramiteEntity.FechaAsignacion = DateTime.Now;
                tramiteEntity.LastUpdateUser = userid;
                tramiteEntity.LastUpdateDate = DateTime.Now;
                await repo.UpdateAsync(tramiteEntity);

            }
            uow.Dispose();
        }
        public async Task ActualizarTramiteNumeroGEDOAsync(int IdTramite, string numeroGEDO)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            string userid = await _usuarioBL.GetCurrentUserid();

            var repo = new TramitesRepository(uow);
            var tramiteEntity = await repo.FirstOrDefaultAsync(x => x.IdTramite == IdTramite);

            if (tramiteEntity != null)
            {
                tramiteEntity.NumeroGedo = numeroGEDO;
                tramiteEntity.LastUpdateUser = userid;
                tramiteEntity.LastUpdateDate = DateTime.Now;
                await repo.UpdateAsync(tramiteEntity);

            }
            uow.Dispose();
        }
        public async Task ActualizarTramiteFileGEDOAsync(int IdTramite, int IdFileGEDO)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            string userid = await _usuarioBL.GetCurrentUserid();

            var repo = new TramitesRepository(uow);
            var tramiteEntity = await repo.FirstOrDefaultAsync(x => x.IdTramite == IdTramite);

            if (tramiteEntity != null)
            {
                tramiteEntity.IdFileGedo = IdFileGEDO;
                tramiteEntity.LastUpdateUser = userid;
                tramiteEntity.LastUpdateDate = DateTime.Now;
                await repo.UpdateAsync(tramiteEntity);

            }
            uow.Dispose();
        }
        public async Task ActualizarCamposBalanceGeneralEvaluadorAsync(TramitesBalanceGeneralDTO dto)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var repo = new TramitesBalancesGeneralesRepository(uow);

            var entity = await repo.FirstOrDefaultAsync(x => x.IdTramiteBalanceGeneral == dto.IdTramiteBalanceGeneral);

            if (entity != null)
            {
                entity.OoficDepositosCortoPlazo = dto.OoficDepositosCortoPlazo;
                entity.OoficDepositosLargoPlazo = dto.OoficDepositosLargoPlazo;
                entity.OoficInversionesCortoPlazo = dto.OoficInversionesCortoPlazo;
                entity.OoficInversionesLargoPlazo = dto.OoficInversionesLargoPlazo;
                entity.OoficOtrosConceptos = dto.OoficOtrosConceptos;
                entity.OpartDepositosCortoPlazo = dto.OpartDepositosCortoPlazo;
                entity.OpartDepositosLargoPlazo = dto.OpartDepositosLargoPlazo;
                entity.OpartInversionesCortoPlazo = dto.OpartInversionesCortoPlazo;
                entity.OpartInversionesLargoPlazo = dto.OpartInversionesLargoPlazo;
                entity.OpartInversiones = dto.OpartInversiones;
                entity.OpartOtrosConceptos = dto.OpartOtrosConceptos;
                entity.DispCajayBancos = dto.DispCajayBancos;
                entity.BusoInversiones = dto.BusoInversiones;
                entity.BusoMaqUtilAfec = dto.BusoMaqUtilAfec;
                entity.BusoMaqUtilNoAfec = dto.BusoMaqUtilNoAfec;
                entity.BusoBienesRaicesAfec = dto.BusoBienesRaicesAfec;
                entity.BusoBienesRaicesNoAfec = dto.BusoBienesRaicesNoAfec;
                entity.BusoOtrosConceptos = dto.BusoOtrosConceptos;
                entity.BcamMateriales = dto.BcamMateriales;
                entity.BcamOtrosConceptos = dto.BcamOtrosConceptos;
                entity.DeuCortoPlazo = dto.DeuCortoPlazo;
                entity.DeuLargoPlazo = dto.DeuLargoPlazo;
                entity.LastUpdateDate = DateTime.Now;
                entity.LastUpdateUser = await _usuarioBL.GetCurrentUserid();
                await repo.UpdateAsync(entity);
            }

            uow.Dispose();
        }

    }
}
