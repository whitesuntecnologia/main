using DataAccess;
using DataAccess.Entities;
using DocumentFormat.OpenXml.Office2016.Drawing.Charts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork;

using Business.Interfaces;
using DataTransferObject.BLs;

namespace Business.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddBusinessLayer<TUnitOfWorkFactory>(
           this IServiceCollection services,
           IConfiguration configuration,
           ServiceLifetime uowFServiceLifetime = ServiceLifetime.Transient)
           where TUnitOfWorkFactory : class, IUnitOfWorkFactory
        {

            services.AddDbContext<AplicationDBContext>(ServiceLifetime.Transient);
            services.AddUnitOfWorkFactory<TUnitOfWorkFactory>(uowFServiceLifetime);
            services.AddBusinessLayerCommon(configuration);

        }
        public static void AddBusinessLayer(
            this IServiceCollection services,
            IConfiguration configuration,
            ServiceLifetime uowFServiceLifetime = ServiceLifetime.Transient)
        {
            services.AddBusinessLayer<UnitOfWorkFactory>(configuration, uowFServiceLifetime);
        }

        private static void AddBusinessLayerCommon(this IServiceCollection services, IConfiguration configuration)
        {
            
            services.AddScoped<ITramitesBL, TramitesBL>();
            services.AddScoped<ICombosBL, CombosBL>();
            services.AddScoped<IUsuariosBL, UsuariosBL>();
            services.AddScoped<ITablasBL, TablasBL>();
            services.AddScoped<IFilesBL, FilesBL>();
            services.AddScoped<IParametrosBL, ParametrosBL>();
            services.AddScoped<ILoginBL, LoginBL>();
            services.AddScoped<IMenuesBL, MenuesBL>();
            services.AddScoped<IPerfilesBL, PerfilesBL>();
            services.AddScoped<IEnvioMailBL, EnvioMailBL>();
            services.AddScoped<IIndiceConstruccionBL, IndiceConstruccionBL>();
            services.AddScoped<IIndiceBancoCentralBL, IndiceBancoCentralBL>();
            services.AddScoped<IIndiceUnidadViviendaBL, IndiceUnidadViviendaBL>();
            services.AddScoped<IEmpresasBL, EmpresasBL>();
            services.AddScoped<IEspecilidadesBL, EspecilidadesBL>();
            services.AddScoped<ITramitesEvaluacionBL, TramitesEvaluacionBL>();
            services.AddScoped<IObrasProvinciaLaPampaBL, ObrasProvinciaLaPampaBL>();
            services.AddScoped<INotificacionesBL, NotificacionesBL>();
            services.AddScoped<IReportesBL, ReportesBL>();
            services.AddScoped<IAntecedentesBl, AntecedentesBl>();
            services.AddScoped<IGedoBL, GedoBL>();
            

            services.AddSingleton<RouteHistoryService>();
            // Configuraciones Varias
            
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }
        private static void AddUnitOfWorkFactory<T>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
           where T : class, IUnitOfWorkFactory
        {
            switch (serviceLifetime)
            {
                case ServiceLifetime.Transient:
                    services.AddTransient<IUnitOfWorkFactory, T>();
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped<IUnitOfWorkFactory, T>();
                    break;
                case ServiceLifetime.Singleton:
                    services.AddSingleton<IUnitOfWorkFactory, T>();
                    break;
            }
        }
    }
}
