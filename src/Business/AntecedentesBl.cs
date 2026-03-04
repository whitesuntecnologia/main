using Ardalis.GuardClauses;
using AutoMapper;
using Business.Interfaces;
using DataAccess.Entities;
using DataTransferObject;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork;

namespace Business
{
    public class AntecedentesBl : IAntecedentesBl
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IMapper _mapper;
        private readonly IUsuariosBL _usuarioBL;
        private readonly ITramitesBL _tramitesBL;

        public AntecedentesBl(IUnitOfWorkFactory uowFactory, IUsuariosBL usuarioBL, ITramitesBL tramitesBL)
        {
            _uowFactory = Guard.Against.Null(uowFactory, nameof(uowFactory));
            _usuarioBL = usuarioBL;
            _tramitesBL = tramitesBL;

            var config = new MapperConfiguration(cfg =>
            {
                #region Antecedentes

                cfg.CreateMap<TramitesAntecedentes, TramitesAntecedentesDto>()
                    .ForMember(dest => dest.DescripcionEspecialidad, opt => opt.MapFrom(src => src.IdTramiteEspecialidadNavigation.IdEspecialidadNavigation.NombreEspecialidad))
                    .ForMember(dest => dest.DescripcionSeccion, opt => opt.MapFrom(src => src.IdTramiteEspecialidadSeccionNavigation.IdSeccionNavigation.DescripcionSeccion))
                    .ForMember(dest => dest.NombreObra, opt => opt.MapFrom(src => src.IdObraPciaLpNavigation.ObraNombre))
                ;
                cfg.CreateMap<TramitesAntecedentesDto, TramitesAntecedentes>();
                cfg.CreateMap<TramitesAntecedentesDdjjmensual, TramitesAntecedentesDdjjMensualDto>().ReverseMap();

                #endregion
            });
            _mapper = config.CreateMapper();
        }

        #region Antecedentes
        public async Task<List<TramitesAntecedentesDto>> GetAntecedentesAsync(int IdTramite)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var repo = new TramitesAntecedentesRepository(uow);

            var elements = await repo.Where(x => x.IdTramite == IdTramite).ToListAsync();
            var result = _mapper.Map<List<TramitesAntecedentes>, List<TramitesAntecedentesDto>>(elements);
            return result;

        }
        public async Task<TramitesAntecedentesDto> GetAntecedenteAsync(int IdTramiteAntecedente)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var repo = new TramitesAntecedentesRepository(uow);

            var element = await repo.Where(x => x.IdTramiteAntecedente == IdTramiteAntecedente).FirstOrDefaultAsync();
            var result = _mapper.Map<TramitesAntecedentes, TramitesAntecedentesDto>(element);
            return result;
        }
        public async Task<List<TramitesAntecedentesDto>> GetAntecedentesxEspecialidad(int IdTramiteEspecialidad, int IdTramiteEspecialidadSeccion)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var repo = new TramitesAntecedentesRepository(uow);

            var element = await repo.Where(x => x.IdTramiteEspecialidad == IdTramiteEspecialidad
                                           && x.IdTramiteEspecialidadSeccion == IdTramiteEspecialidadSeccion)
                                    .ToListAsync();
            var result = _mapper.Map<List<TramitesAntecedentes>, List<TramitesAntecedentesDto>>(element);
            return result;
        }
        public async Task<List<TramitesAntecedentesDdjjMensualDto>> GetAntecedentesDdjjMensualAsync(int IdTramiteEspecialidad, int IdTramiteEspecialidadSeccion)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var repo = new TramitesAntecedentesDDJJMensualRepository(uow);

            var element = await repo.Where(x => x.IdTramiteAntecedenteNavigation.IdTramiteEspecialidad == IdTramiteEspecialidad
                                           && x.IdTramiteAntecedenteNavigation.IdTramiteEspecialidadSeccion == IdTramiteEspecialidadSeccion)
                                    .ToListAsync();
            var result = _mapper.Map<List<TramitesAntecedentesDdjjmensual>, List<TramitesAntecedentesDdjjMensualDto>>(element);
            return result;
        }
        public async Task AgregarAntecedenteAsync(TramitesAntecedentesDto dto)
        {
            string userid = await _usuarioBL.GetCurrentUserid();
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repoAntecedentes = new TramitesAntecedentesRepository(uow);

                    var entityAntecedentes = await repoAntecedentes.FirstOrDefaultAsync(x => x.IdTramite == dto.IdTramite &&
                                                                x.IdTramiteEspecialidad == dto.IdTramiteEspecialidad &&
                                                                x.IdTramiteEspecialidadSeccion == dto.IdTramiteEspecialidadSeccion &&
                                                                x.IdObraPciaLp == dto.IdObraPciaLp
                                                                );

                    if (entityAntecedentes == null)
                    {
                        entityAntecedentes = _mapper.Map<TramitesAntecedentes>(dto);
                        entityAntecedentes.CreateUser = userid;
                        entityAntecedentes.CreateDate = DateTime.Now;
                        await repoAntecedentes.AddAsync(entityAntecedentes);
                        await uow.CommitAsync();
                    }
                    else
                        throw new ArgumentException("Ya existe un registro con los mismos datos.");

                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }
        }
        public async Task ActualizarAntecedenteAsync(TramitesAntecedentesDto dto)
        {
            string userid = await _usuarioBL.GetCurrentUserid();
            var repoRead = new TramitesAntecedentesRepository(_uowFactory.GetUnitOfWork());
            var entityDuplicado = await repoRead.FirstOrDefaultAsync(x =>
                                            x.IdTramiteEspecialidad == dto.IdTramiteEspecialidad &&
                                            x.IdTramiteEspecialidadSeccion == dto.IdTramiteEspecialidadSeccion &&
                                            x.IdObraPciaLp == dto.IdObraPciaLp &&
                                            x.IdTramiteAntecedente != dto.IdTramiteAntecedente);

            if (entityDuplicado != null)
                throw new ArgumentException($"Ya existe un registro con los mismos datos.");

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repoAntecedentes = new TramitesAntecedentesRepository(uow);

                    var entityAntecedentes = await repoAntecedentes.FirstOrDefaultAsync(x => x.IdTramiteAntecedente == dto.IdTramiteAntecedente);

                    if (entityAntecedentes == null)
                    {
                        entityAntecedentes = _mapper.Map<TramitesAntecedentes>(dto);
                        entityAntecedentes.CreateDate = DateTime.Now;
                        entityAntecedentes.CreateUser = userid;
                    }
                    else
                    {
                        _mapper.Map<TramitesAntecedentesDto, TramitesAntecedentes>(dto, entityAntecedentes);
                        entityAntecedentes.LastUpdateDate = DateTime.Now;
                        entityAntecedentes.LastUpdateUser = userid;
                        await repoAntecedentes.UpdateAsync(entityAntecedentes);
                    }

                    await repoAntecedentes.UpdateAsync(entityAntecedentes);
                    await uow.CommitAsync();

                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }

        }
        public async Task EliminarAntecedenteAsync(int IdTramiteAntecedente)
        {
            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repoAntecedentes = new TramitesAntecedentesRepository(uow);
                    var repoDDJJ = new TramitesAntecedentesDDJJMensualRepository(uow);

                    var entityAntecedentes = await repoAntecedentes.FirstOrDefaultAsync(x => x.IdTramiteAntecedente == IdTramiteAntecedente);

                    await repoDDJJ.RemoveRangeAsync(entityAntecedentes.TramitesAntecedentesDdjjmensual.ToList());
                    await repoAntecedentes.RemoveAsync(entityAntecedentes);

                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }

            }
        }
        public async Task ActualizarAntecedenteDdjjMensualAsync(List<TramitesAntecedentesDdjjMensualDto> lstDto)
        {

            using (var uow = _uowFactory.GetUnitOfWork(System.Transactions.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var repo = new TramitesAntecedentesDDJJMensualRepository(uow);

                    //Se eliminar todos los datos de las distintas obras (Obras = IdTramiteAntecedente)
                    var lstIdsEliminar = lstDto.Select(s => s.IdTramiteAntecedente).Distinct().ToList();
                    var lstEntityEliminar = await repo.Where(x => lstIdsEliminar.Contains(x.IdTramiteAntecedente)).ToListAsync();
                    await repo.RemoveRangeAsync(lstEntityEliminar);

                    //Agregar todos los datos que tuvieron monto.
                    var lstEntityAgregar = _mapper.Map<List<TramitesAntecedentesDdjjmensual>>(lstDto);
                    await repo.AddARangeAsync(lstEntityAgregar);
                    await uow.CommitAsync();

                }
                catch (Exception)
                {
                    uow.RollBack();
                    throw;
                }
            }

        }
        public async Task<List<TramitesAntecedentesDdjjFilaDto>> GetDdjjMensual5AniosAsync(DateTime FechaDesde, int IdTramiteEspecialidad, int IdTramiteEspecialidadSeccion)
        {
            List<TramitesAntecedentesDdjjFilaDto> lstResult = new();

            //Obtiene las Obras de la especialidad
            var lstAntecedentes = (await GetAntecedentesxEspecialidad(IdTramiteEspecialidad, IdTramiteEspecialidadSeccion))
                                  .OrderBy(o=> o.IdTramiteAntecedente).ToList();

            //Obtiene el detalle mensual de las obras de la especialidad
            var lstDdjjMensual = await GetAntecedentesDdjjMensualAsync(IdTramiteEspecialidad, IdTramiteEspecialidadSeccion);

            
            DateTime fecha = FechaDesde.AddMonths(-60); // mes anterior al actual y 5 años atras
            int totalMeses = 5 * 12; // 5 años = 60 meses

            for (int i = 0; i < totalMeses; i++)
            {

                var fila = new TramitesAntecedentesDdjjFilaDto
                {
                    Mes = fecha.Month,
                    Anio = fecha.Year,
                    MontoFila = 0
                };

                foreach (var Antecedente in lstAntecedentes)
                {
                    var filaInDb = lstDdjjMensual.FirstOrDefault(x => x.IdTramiteAntecedente == Antecedente.IdTramiteAntecedente &&
                                                    x.Mes == fecha.Month && x.Anio == fecha.Year);

                    var PeriodoActualNum = StaticClass.Functions.ConvertPeriodoToNumber($"{fecha.Month.ToString("00")}/{fecha.Year}") ?? 0;
                    bool PermiteEditarMonto = PeriodoActualNum >= (StaticClass.Functions.ConvertPeriodoToNumber(Antecedente.PeriodoInicio) ?? 0);
                    if (filaInDb != null)
                    {
                        fila.Antecedentes.Add(new TramitesAntecedentesDdjjColumnaDto
                        {
                            IdAntecedente = Antecedente.IdTramiteAntecedente,
                            NombreObra = Antecedente.NombreObra,
                            MontoMensual = (PermiteEditarMonto ? filaInDb.Monto : 0),
                            PermiteEditarMonto = PermiteEditarMonto
                        });
                        fila.MontoFila += (PermiteEditarMonto ? filaInDb.Monto : 0);
                    }
                    else
                    {
                        fila.Antecedentes.Add(new TramitesAntecedentesDdjjColumnaDto
                        {
                            IdAntecedente = Antecedente.IdTramiteAntecedente,
                            NombreObra = Antecedente.NombreObra,
                            MontoMensual = 0m,
                            PermiteEditarMonto = PermiteEditarMonto
                        });
                    }

                }

                lstResult.Add(fila);
                fecha = fecha.AddMonths(1);

            }

            return lstResult;

        }
        public async Task<TramitesAntecedentesResumen12MesesDto> GetResumenMejores12Meses(List<TramitesAntecedentesDdjjFilaDto> lstData)
        {
            TramitesAntecedentesResumen12MesesDto result = null!;
            var uow = _uowFactory.GetUnitOfWork();
            var repoAntecedentes = new TramitesAntecedentesRepository(uow);
            var repoBalance = new TramitesBalancesGeneralesRepository(uow);
            var repoICC = new IndicesCostoConstruccionRepository(uow);

            int ventana = 12; // 12 meses consecutivos
            int mejorMesInicio = 0;
            int mejorAnioInicio = 0;
            decimal CapacidadBasicaActual = 0;

            var lstAntecedentesTotales12Meses = new List<TramitesAntecedentesTotales12mesesDto>();
            if (lstData.Any())
            {

                var lstIdsAntecedentes = lstData[0].Antecedentes.Select(s => s.IdAntecedente).ToList();

                //Establece la cantidad de objetos de resultado a partir de los antecedentes de la matriz.
                foreach (int IdAntecedente in lstIdsAntecedentes)
                {
                    var Antecedente = await repoAntecedentes.FirstOrDefaultAsync(x => x.IdTramiteAntecedente == IdAntecedente);

                    //Obtiene el Indice de Construccion (base) del período de contratacion (base) del Antecedente (Obra)
                    var IndicePeriodoBase = repoICC.GetIndice(Antecedente.PeriodoBase);

                    //Obtiene la fecha de balance del ultimo balance
                    var FechaBalance = await repoBalance.Where(x => x.IdTramite == Antecedente.IdTramite).Select(s => s.FechaBalance).MaxAsync();

                    //Obtiene el indice de Construcción del período del balance
                    var IndicePeriodoBalance = repoICC.GetIndice(FechaBalance.Year, FechaBalance.Month);


                    decimal IndiceActivo = 0;
                    if (IndicePeriodoBase.GetValueOrDefault() > 0 && IndicePeriodoBalance.GetValueOrDefault() > 0)
                        IndiceActivo = IndicePeriodoBalance.Value / IndicePeriodoBase.Value;

                    lstAntecedentesTotales12Meses.Add(new TramitesAntecedentesTotales12mesesDto
                    {
                        IdAntecedente = IdAntecedente,
                        NombreObra = Antecedente.IdObraPciaLpNavigation.ObraNombre,
                        IndiceActivo = IndiceActivo,
                        MontoActivo = 0,
                        Total= 0,
                    });
                }

                //Cantidad de grupos de 12 que debemos recorrer
                int cantidadVeces = lstData.Count - ventana + 1;
                // Deslizar la ventana
                for (int i = 1; i <= cantidadVeces; i++)
                {
                    // seleccionar los 12 grupos de antecedentes a trabajar
                    var lstAntecedentes = lstData.Skip(i - 1).Take(ventana).SelectMany(s => s.Antecedentes).ToList();
                    //Limpiar valores de operacion anterior 
                    lstAntecedentesTotales12Meses.ForEach(f => { f.MontoActivo = 0; f.Total = 0; });
                    
                    //Calcular los valores de Monto Activo de cada obra dentro de los 12 registros seleccionados
                    //y establecer los totales dentro de la lista de antecedentes totales
                    foreach (int IdAntecedente in lstIdsAntecedentes)
                    {
                        var itemResultado = lstAntecedentesTotales12Meses.FirstOrDefault(x => x.IdAntecedente == IdAntecedente);
                        itemResultado.Total = lstAntecedentes.Where(x => x.IdAntecedente == IdAntecedente).Select(s => s.MontoMensual).Sum();
                        itemResultado.MontoActivo = itemResultado.Total * itemResultado.IndiceActivo;
                    }
                    //Calcula la capacidad básica del grupo de 12 meses elegidos
                    decimal CapacidadBasica = lstAntecedentesTotales12Meses.Select(s=> s.MontoActivo).Sum();

                    if(CapacidadBasica > CapacidadBasicaActual)
                    {
                        CapacidadBasicaActual = CapacidadBasica;
                        int posicion = (i - 1);
                        mejorMesInicio = lstData[posicion].Mes;
                        mejorAnioInicio = lstData[posicion].Anio;
                    }
                }


                if (mejorMesInicio > 0)
                {

                    var FechaFin = (new DateTime(mejorAnioInicio, mejorMesInicio, 1, 0, 0, 0, DateTimeKind.Utc)).AddMonths(11);

                    result = new TramitesAntecedentesResumen12MesesDto
                    {
                        MesInicio = mejorMesInicio,
                        AnioInicio = mejorAnioInicio,
                        MesFin = FechaFin.Month,
                        AnioFin = FechaFin.Year,
                        TotalMejores12Meses = CapacidadBasicaActual
                    };
                }

            }
            return result;
        }



        public async Task<List<BalanceGeneralCapacidadProduccionDto>> GetCapacidadProduccionAsync(int IdTramite)
        {
            List<BalanceGeneralCapacidadProduccionDto> lstResult = new();
            using var uow = _uowFactory.GetUnitOfWork();
            
            var repoAntecedentes = new TramitesAntecedentesRepository(uow);

            var FechaPresentacion = await _tramitesBL.GetFechaPresentacionAsync(IdTramite) ?? DateTime.Today;
            var lstAntecedentes = await repoAntecedentes.Where(x=> x.IdTramite == IdTramite)
                .Select(s=> new
                {
                    s.IdTramiteEspecialidad,
                    s.IdTramiteEspecialidadSeccion,
                    Especialidad = s.IdTramiteEspecialidadNavigation.IdEspecialidadNavigation,
                    Seccion = s.IdTramiteEspecialidadSeccionNavigation.IdSeccionNavigation
                })
                .Distinct()
                .ToListAsync();

            foreach (var itemAntecedente in lstAntecedentes)
            {

                var lstData = await GetDdjjMensual5AniosAsync(FechaPresentacion,itemAntecedente.IdTramiteEspecialidad, itemAntecedente.IdTramiteEspecialidadSeccion);
                var ResumenMejores12Meses = await GetResumenMejores12Meses(lstData);
                decimal ImporteProduccion = 0;
                if (ResumenMejores12Meses != null)
                {
                    var TotalesMejores12Meses = await GetTotalesMejores12MesesAsync(lstData, ResumenMejores12Meses.MesInicio, ResumenMejores12Meses.AnioInicio);
                    if(TotalesMejores12Meses != null)
                        ImporteProduccion = TotalesMejores12Meses.Sum(s => s.MontoActivo);
                }
                
                lstResult.Add(new BalanceGeneralCapacidadProduccionDto
                {
                    IdEspecialidad = itemAntecedente.Especialidad.IdEspecialidad,
                    Rama = itemAntecedente.Especialidad.Rama,
                    IdSeccion = itemAntecedente.Seccion.IdSeccion,
                    DescripcionSeccion = itemAntecedente.Seccion.DescripcionSeccion,
                    CoefCapacTecnicaxEquipo  = itemAntecedente.Seccion.CoefCapacTecnicaxEquipo,
                    MultiplicadorCapEconomica = itemAntecedente.Seccion.MultiplicadorCapEconomica,
                    Importe = ImporteProduccion,
                });

            }

            return lstResult;
        }

        public async Task<List<TramitesAntecedentesTotales12mesesDto>> GetTotalesMejores12MesesAsync(List<TramitesAntecedentesDdjjFilaDto> lstData, int MesInicio, int AnioInicio)
        {

            List<TramitesAntecedentesTotales12mesesDto> lstResult = new();
            var uow = _uowFactory.GetUnitOfWork();
            var repoAntecedentes = new TramitesAntecedentesRepository(uow);
            var repoBalance = new TramitesBalancesGeneralesRepository(uow);
            var repoICC = new IndicesCostoConstruccionRepository(uow);


            if (lstData.Any())
            {
                var lstIdsAntecedentes = lstData[0].Antecedentes.Select(s => s.IdAntecedente).ToList();

                //Establece la cantidad de objetos de resultado a partir de los antecedentes de la matriz.
                foreach (int IdAntecedente in lstIdsAntecedentes)
                {
                    var Antecedente = await repoAntecedentes.FirstOrDefaultAsync(x => x.IdTramiteAntecedente == IdAntecedente);

                    //Obtiene el Indice de Construccion del período de contratacion del Antecedente (Obra)
                    var IndicePeriodoContratacion = repoICC.GetIndice(Antecedente.PeriodoBase);

                    //Obtiene la fecha de balance del ultimo balance
                    var FechaBalance = await repoBalance.Where(x => x.IdTramite == Antecedente.IdTramite).Select(s => s.FechaBalance).MaxAsync();

                    //Obtiene el indice de Construcción del período del balance
                    var IndicePeriodoBalance = repoICC.GetIndice(FechaBalance.Year, FechaBalance.Month);

                    decimal IndiceActivo = 0;
                    if (IndicePeriodoContratacion.GetValueOrDefault() > 0 && IndicePeriodoBalance.GetValueOrDefault() > 0)
                        IndiceActivo = IndicePeriodoBalance.Value / IndicePeriodoContratacion.Value;

                    lstResult.Add(new TramitesAntecedentesTotales12mesesDto
                    {
                        IdAntecedente = IdAntecedente,
                        NombreObra = Antecedente.IdObraPciaLpNavigation.ObraNombre,
                        IndiceActivo = IndiceActivo,
                        MontoActivo = 0,
                        Total = 0,
                    });
                }


                //--

                var periodoInicio = $"{AnioInicio}{MesInicio.ToString("00")}";
                int MesActual = 0;
                foreach (var item in lstData)
                {
                    var periodoActual = $"{item.Anio}{item.Mes:00}";

                    if (Convert.ToInt32(periodoActual) >= Convert.ToInt32(periodoInicio) && MesActual < 12)
                    {

                        foreach (int IdAntecedente in lstIdsAntecedentes)
                        {
                            var itemResultado = lstResult.FirstOrDefault(x => x.IdAntecedente == IdAntecedente);
                            itemResultado.Total += item.Antecedentes.First(x => x.IdAntecedente == IdAntecedente).MontoMensual;
                            itemResultado.TotalObra += item.Antecedentes.First(x => x.IdAntecedente == IdAntecedente).MontoMensual;
                            itemResultado.MontoActivo = itemResultado.Total * itemResultado.IndiceActivo;
                            
                        }
                        MesActual += 1;
                    }
                    else
                    {
                        foreach (int IdAntecedente in lstIdsAntecedentes)
                        {
                            var itemResultado = lstResult.FirstOrDefault(x => x.IdAntecedente == IdAntecedente);
                            itemResultado.TotalObra += item.Antecedentes.First(x => x.IdAntecedente == IdAntecedente).MontoMensual;
                        }
                    }

                }
            }
            return lstResult;
        }
        #endregion
    }
}
