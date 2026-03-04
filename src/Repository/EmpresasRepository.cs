using DataAccess.Entities;
using DataAccess.EntitiesCustom;
using DataTransferObject;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using StaticClass;
using StaticClass.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnitOfWork;

namespace Repository
{
    public class EmpresasRepository : BaseRepository<Empresas>
    {
        public EmpresasRepository(IUnitOfWork unit) : base(unit)
        {
        }

        public async Task<List<Empresas>> GetInformeEmpresasAsync(FiltroInformeEmpresasDTO filtro,CancellationToken cancellationToken = default)
        {


            IQueryable<Empresas> qEmpresas = Context.Empresas;
;

            if (filtro.EmpresasRegistradas)
                qEmpresas = qEmpresas.Where(x => x.Tramites.Select(s => s.IdEstado).Contains(Constants.TramitesEstados.Aprobado));
            else
                qEmpresas = qEmpresas.Where(x => !x.Tramites.Select(s => s.IdEstado).Contains(Constants.TramitesEstados.Aprobado));

            if (filtro.Cuit.HasValue)
                qEmpresas = qEmpresas.Where(x => x.CuitEmpresa == filtro.Cuit);

            if(!string.IsNullOrWhiteSpace(filtro.RazonSocial))
                qEmpresas = qEmpresas.Where(x=> x.RazonSocial.Contains(filtro.RazonSocial));

            if(filtro.FechaRegistroDesde.HasValue)
                qEmpresas = qEmpresas.Where(x => x.FechaInscripcion.Value >= filtro.FechaRegistroDesde.Value);
            
            if (filtro.FechaRegistroHasta.HasValue)
                qEmpresas = qEmpresas.Where(x => x.FechaInscripcion.Value <= filtro.FechaRegistroHasta);

            if (filtro.FechaVencimientoDesde.HasValue)
                qEmpresas = qEmpresas.Where(x => x.Vencimiento.Value >= filtro.FechaVencimientoDesde.Value);

            if (filtro.FechaVencimientoHasta.HasValue)
                qEmpresas = qEmpresas.Where(x => x.Vencimiento.Value <= filtro.FechaVencimientoHasta);

            

            var lstEmpresas = await qEmpresas.ToListAsync(cancellationToken);



            if (filtro.EspecialidadesSelected != null && filtro.EspecialidadesSelected.Count() > 0)
                lstEmpresas = lstEmpresas.Where(x => x.EmpresasEspecialidades.Select(s => s.IdEspecialidad)
                                         .Intersect(filtro.EspecialidadesSelected).ToList().Count() == filtro.EspecialidadesSelected.Count())
                                         .ToList();


            return lstEmpresas;

        }
        public async Task<(List<InformeCapacidadesxEmpresaDtoFlat> datos, List<EspecialidadInfo> especialidades)>
            GetCapacidadesxEmpresaFlatAsync(FiltroCapacidadesxEmpresaDto filtro)
        {
            IQueryable<Empresas> qEmpresas = Context.Empresas;

            

            var fechaActual = DateTime.Now;
            if (filtro.VencimientoSelected == 1)
            {
                // Todas - no filtrar
            }
            else if (filtro.VencimientoSelected == 2)
            {
                qEmpresas = qEmpresas.Where(x => x.Vencimiento.HasValue && x.Vencimiento.Value < fechaActual);
            }
            else if (filtro.VencimientoSelected == 3)
            {
                qEmpresas = qEmpresas.Where(x => x.Vencimiento.HasValue && x.Vencimiento.Value >= fechaActual);
            }

            if (filtro.IdEspecialidadSelected.HasValue && filtro.IdEspecialidadSelected.Value > 0)
            {
                qEmpresas = qEmpresas.Where(x => x.EmpresasEspecialidades
                    .Any(e => e.IdEspecialidad == filtro.IdEspecialidadSelected.Value));
            }

            var lstEmpresas = await qEmpresas.ToListAsync();

            // Obtener todas las especialidades únicas
            var SeccionesUnicas = lstEmpresas
                .SelectMany(e => e.EmpresasConstanciaCap
                    .Where(c => c.IdSeccionNavigation != null && (filtro.IdEspecialidadSelected.GetValueOrDefault() == 0 || c.IdSeccionNavigation.IdEspecialidad == filtro.IdEspecialidadSelected.Value) )
                    .Select(c => new
                    {
                        IdSeccion = c.IdSeccion,
                        Descripcion = c.DescripcionSeccion
                    }))
                .Distinct()
                .OrderBy(x => x.Descripcion)
                .Select(x => new EspecialidadInfo
                {
                    IdSeccion = x.IdSeccion!.Value,
                    Descripcion = x.Descripcion
                })
                .ToList();

            var resultado = new List<InformeCapacidadesxEmpresaDtoFlat>();

            //Calcula el Indice UVI para el período anterior a la fecha actual (último día del mes anterior)
            var fechaPeriodoAnterior = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            fechaPeriodoAnterior = fechaPeriodoAnterior.AddDays(-1);
            var indiceFechaAnterior = await Context.IndicesUnidadVivienda.FirstOrDefaultAsync(x => x.Mes == fechaPeriodoAnterior.Month && x.Anio == fechaPeriodoAnterior.Year);
            //--

            foreach (var empresa in lstEmpresas)
            {
                decimal IndiceUVICoeficiente = 0m;
                var informe = new InformeCapacidadesxEmpresaDtoFlat
                {
                    IdEmpresa = empresa.IdEmpresa,
                    CuitEmpresa = empresa.CuitEmpresa.ToString(),
                    RazonSocial = empresa.RazonSocial,
                    Domicilio = empresa.Domicilio,
                    FechaInscripcion = empresa.FechaInscripcion,
                    Vencimiento = empresa.Vencimiento
                };

                //Calcula el Indice UVI para el período del último balance registrado
                var fechaBalance = empresa.EmpresasBalancesGenerales.OrderByDescending(x => x.FechaBalance).FirstOrDefault()?.FechaBalance;
                if (fechaBalance.HasValue)
                {
                    var indiceFechaBalance = await Context.IndicesUnidadVivienda.FirstOrDefaultAsync(x => x.Mes == fechaBalance.Value.Month && x.Anio == fechaBalance.Value.Year);
                    // Si ninguno de los índices es nulo y el valor del índice del balance es mayor a 0, calcula el coeficiente UVI
                    if (indiceFechaAnterior != null && indiceFechaBalance != null)
                    {
                        if (indiceFechaBalance.Valor  > 0)
                            IndiceUVICoeficiente = indiceFechaAnterior.Valor / indiceFechaBalance.Valor;
                    }
                }
                

                // Agrupar capacidades por especialidad
                var capacidadesPorEspecialidad = empresa.EmpresasConstanciaCap.ToList();

                foreach (var seccion in capacidadesPorEspecialidad)
                {
                    
                    informe.SetCapacidad($"{seccion.IdSeccion}-{seccion.DescripcionSeccion}","CT", seccion.CapacidadTecnica.GetValueOrDefault());
                    informe.SetCapacidad($"{seccion.IdSeccion}-{seccion.DescripcionSeccion}", "CC", seccion.CapacidadContratacion.GetValueOrDefault());
                    informe.SetCapacidad($"{seccion.IdSeccion}-{seccion.DescripcionSeccion}", "CE", seccion.EjecucaionAnual.GetValueOrDefault());

                    informe.SetCapacidad($"{seccion.IdSeccion}-{seccion.DescripcionSeccion}", "CTU", seccion.CapacidadTecnicaUvi.GetValueOrDefault());
                    informe.SetCapacidad($"{seccion.IdSeccion}-{seccion.DescripcionSeccion}", "CCU", seccion.CapacidadContratacionUvi.GetValueOrDefault());
                    informe.SetCapacidad($"{seccion.IdSeccion}-{seccion.DescripcionSeccion}", "CEU", seccion.EjecucaionAnualUvi.GetValueOrDefault());

                    informe.SetCapacidad($"{seccion.IdSeccion}-{seccion.DescripcionSeccion}", "CTUA", seccion.CapacidadTecnica.GetValueOrDefault() * IndiceUVICoeficiente);
                    informe.SetCapacidad($"{seccion.IdSeccion}-{seccion.DescripcionSeccion}", "CCUA", seccion.CapacidadContratacion.GetValueOrDefault() * IndiceUVICoeficiente);
                    informe.SetCapacidad($"{seccion.IdSeccion}-{seccion.DescripcionSeccion}", "CEUA", seccion.EjecucaionAnual.GetValueOrDefault() * IndiceUVICoeficiente);

                }


                resultado.Add(informe);
            }

            return (resultado, SeccionesUnicas);
        }
    }
}

