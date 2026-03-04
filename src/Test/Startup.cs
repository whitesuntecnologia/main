using Business;
using Business.Interface;
using Business.MarkupPercentagesUpdateAndDiscounts;
using DataAccess.Entities;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Radzen;
using Repository;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Fixtures;
using Test.Helpers.Regression;
using Xunit.Abstractions;
using Xunit.DependencyInjection;

namespace Test
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConnectionFactory, ConnectionFactory>();

            // Fixtures
            services.AddTransient<IMarkupPercentagesUpdateAndDiscountsFixtures, MarkupPercentagesUpdateAndDiscountsFixtures>();

            services.AddSingleton<NavigationManager>(new TestNavigationManager());

            // Componentes Radzen
            services.AddScoped<DialogService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<TooltipService>();
            services.AddScoped<ContextMenuService>();

            var driver = new ChromeDriver();
            services.AddTransient<IWebDriver>((x) => driver);
            services.AddTransient<IWait<IWebDriver>>((x) => new WebDriverWait(driver, TimeSpan.FromSeconds(10)));
            services.AddTransient<IUIActionHelper, UIActionHelper>();

            services.AddTransient<IMenuBL, MenuBL>();
            services.AddTransient<IRolesBL<RolesDTO>, RolesBL>();
            services.AddTransient<IRubrosComprasBL<RubrosComprasDTO>, RubrosComprasBL>();

            services.AddTransient<IAccionesTerapeuticasBL<AccionesTerapeuticasDTO>, AccionesTerapeuticasBL>();
            services.AddTransient<IDrogasBL<DrogasDTO>, DrogasBL>();

            services.AddTransient<IFamiliaBL<FamiliaDTO>, FamiliaBL>();
            services.AddTransient<IFleteBL<FleteDTO>, FleteBL>();

            //Repositories
            services.AddTransient<IFileTypesRepository, FileTypesRepository>();
            services.AddTransient<IProductosDescuentosProveedorRepository, ProductosDescuentosProveedorRepository>();
            services.AddTransient<IBaseRepository<Productos>, BaseRepository<Productos>>();

            //clientes farmacias
            services.AddTransient<IClientesBL<ClientesDTO>, ClientesBL>();
            services.AddTransient<ILocalidadesBL<LocalidadesDTO>, LocalidadesBL>();
            services.AddTransient<IProvinciasBL<ProvinciasDTO>, ProvinciasBL>();
            services.AddTransient<IEstadosBL<EstadosDTO>, EstadosBL>();
            services.AddTransient<ITiposDeDocumentosBL<TiposDeDocumentoDTO>, TiposDeDocumentosBL>();
            services.AddTransient<IClientesFarmaciasBL<ClientesFarmaciasDTO>, ClientesFarmaciasBL>();
            services.AddTransient<IFarmaciasBL<FarmaciasDTO>, FarmaciasBL>();
            services.AddTransient<ICategoriasClientesBL<CategoriasClientesDTO>, CategoriasClientesBL>();
            services.AddTransient<IClientesResponsabilidadesBL<ClientesResponsabilidadesDTO>, ClientesResponsabilidadesBL>();
            services.AddTransient<IMotivosAlicuotasIngBrutosBL<MotivosAlicuotasIngBrutosDTO>, MotivosAlicuotasIngBrutosBL>();
            services.AddTransient<IConvMultIngBrutosFarmaciasProvinciasBL<ConvMultIngBrutosFarmaciasProvinciasDTO>, ConvMultIngBrutosFarmaciasProvinciasBL>();
            services.AddTransient<IImpuestosNacionalesFarmaciasBL<ImpuestosNacionalesFarmaciasDTO>, ImpuestosNacionalesFarmaciasBL>();
            services.AddTransient<IImpuestosProvincialesFarmaciasBL<ImpuestosProvincialesFarmaciasDTO>, ImpuestosProvincialesFarmaciasBL>();
            services.AddTransient<IImpuestosMunicipalesFarmaciasBL<ImpuestosMunicipalesFarmaciasDTO>, ImpuestosMunicipalesFarmaciasBL>();
            services.AddTransient<IPuntosDeVentaPlanesObrasSocialesBL<PuntosDeVentaPlanesObrasSocialesDTO>, PuntosDeVentaPlanesObrasSocialesBL>();
            services.AddTransient<IPlanesObrasSociales<PlanesObrasSocialesDTO>, PlanesObrasSocialesBL>();
            //

            ///Puntos de venta
            services.AddTransient<IPuntosDeVentaBL<PuntosDeVentaDTO>, PuntosDeVentaBL>();
            services.AddTransient<ITiposDeClienteBL<TiposDeClienteDTO>, TiposDeClienteBL>();
            services.AddTransient<ITiposDeClienteInternoBL<TiposDeClienteInternoDTO>, TiposDeClienteInternoBL>();
            services.AddTransient<ISucursalesDrogueriaBL<SucursalesDrogueriaDTO>, SucursalesDrogueriaBL>();
            services.AddTransient<IPlazosDePagoBL<PlazosDePagoDTO>, PlazosDePagoBL>();
            services.AddTransient<IPuntosDeVentaContactosBL<PuntosDeVentaContactosDTO>, PuntosDeVentasContactosBL>();
            services.AddTransient<ITiposCargosBL<TiposCargosDTO>, TiposCargosBL>();
            services.AddTransient<IPuntosDeVentaCategoriasBL<PuntosDeVentaCategoriasDTO>, PuntosDeVentaCategoriasBL>();
            services.AddTransient<IPuntosDeVentaSucursalesAlternativasBL<PuntosDeVentaSucursalesAlternativasDTO>, PuntosDeVentaSucursalesAlternativasBL>();
            services.AddTransient<IFormasEnvioBL<FormasEnvioDTO>, FormasEnvioBL>();
            services.AddTransient<ICobrosEncomiendasBL<CobrosEncomiendasDTO>, CobrosEncomiendaBL>();
            services.AddTransient<IRepartosBL<RepartosDTO>, RepartosBL>();
            services.AddTransient<IPuntosDeVentaRepartosAlternativosBL<PuntosDeVentaRepartosAlternativosDTO>, PuntosDeVentaRepartosAlternativosBL>();
            services.AddTransient<ICuentasResumenesBL<CuentasResumenesDTO>, CuentasResumenesBL>();
            services.AddTransient<IPromotoresBL<PromotoresDTO>, PromotoresBL>();
            services.AddTransient<IEmpleadosBL<EmpleadosDTO>, EmpleadosBL>();
            services.AddTransient<ICuentasCorrientesBL<CuentasCorrientesDTO>, CuentasCorrientesBL>();
            services.AddTransient<IDiasHorariosCierreBL<DiasHorariosCierreDTO>, DiasHorariosCierreBL>();
            ///

            //Alfa Beta
            services.AddTransient<IParametrosBL<ParametrosDTO>, ParametrosBL>();

            services.AddTransient<ILaborariosBL<LaboratoriosDTO>, LaboratoriosBL>();
            services.AddTransient<ILaboratoriosProveedoresObrasSocialesBL<LaboratoriosProveedoresObrasSocialesDTO>, LaboratoriosProveedoresObrasSocialesBL>();
            services.AddTransient<IPlanesBL<PlanesDTO>, PlanesBL>();
            services.AddTransient<IPlanesObrasSociales<PlanesObrasSocialesDTO>, PlanesObrasSocialesBL>();
            services.AddTransient<ILaboratoriosProveedoresBL<LaboratoriosProveedoresDTO>, LaboratoriosProveedoresBL>();
            services.AddTransient<IAuditoriaEstadoLabBL<AuditoriaEstadoLabDTO>, AuditoriaEstadoLabBL>();

            // Proveedores
            services.AddTransient<IProveedoresBL<ProveedoresDTO>, ProveedoresBL>();
            services.AddTransient<ITipoDeProveedoresBL<TiposDeProveedoresDTO>, TipoDeProveedoresBL>();
            services.AddTransient<IProvinciasBL<ProvinciasDTO>, ProvinciasBL>();
            services.AddTransient<ILocalidadesBL<LocalidadesDTO>, LocalidadesBL>();
            services.AddTransient<ICondicionesDelIvaBL<CondicionesDeIvaDTO>, CondicionesDelIvaBL>();
            services.AddTransient<ICategoriasMonotributoBL<CategoriasMonotributoDTO>, CategoriasMonotributoBL>();
            services.AddTransient<ICondImpGananciasBL<CondImpGananciaDTO>, CondImpGananciasBL>();
            services.AddTransient<ICondMunicipalidadCb<CondMunicipalidadCbDTO>, CondMunicipalidadCbBL>();
            services.AddTransient<ICondIngresosBrutosBL<CondIngresosBrutosDTO>, CondIngresosBrutosBL>();
            services.AddTransient<ITipoDeComprasBL<TiposDeComprasDTO>, TipoDeComprasBL>();
            services.AddTransient<ITipoVencimientosBL<TiposDeVencimientosDTO>, TiposVencimientosBL>();
            services.AddTransient<IProveedoresVencimientosHistoricosBL<ProveedoresVencimientosHistoricoDTO>, ProveedoresVencimientosHistoricosBL>();

            services.AddTransient<IAuditoriaTablasBL<AuditoriaDeTablasDTO>, AuditoriaTablasBL>();
            services.AddTransient<IUsuariosBL<UsuarioDTO>, UsuariosBL>();

            //Categorias Comerciales
            services.AddTransient<ICategoriasComercialesBL<CategoriasComercialesDTO>, CategoriasComercialesBL>();
            services.AddTransient<IOfertasMensualesBL<OfertasMensualesDTO>, OfertasMensualesBL>();

            //Productos
            services.AddTransient<IProductosBL<ProductosDTO>, ProductosBL>();

            services.AddTransient<IProductosCategoriasAlmacenamientoBL<ProductosCategoriasAlmacenamientoDTO>, ProductosCategoriasAlmacenamientoBL>();
            services.AddTransient<IProductosCategoriasBL<ProductosCategoriasDTO>, ProductosCategoriasBL>();
            services.AddTransient<IProductosCodigosBarraBL<ProductosCodigosBarraDTO>, ProductosCodigosBarraBL>();
            services.AddTransient<IProductosComprasBL<ProductosComprasDTO>, ProductosComprasBL>();
            services.AddTransient<IProductosDepositoBL<ProductosDepositoDTO>, ProductosDepositoBL>();
            services.AddTransient<IProductosEstadosTrazablesBL<ProductosEstadosTrazablesDTO>, ProductosEstadosTrazablesBL>();
            services.AddTransient<IProductosFormasFarmaceuticasBL<ProductosFormasFarmaceuticasDTO>, ProductosFormasFarmaceuticasBL>();
            services.AddTransient<IProductosMarcasBL<ProductosMarcasDTO>, ProductosMarcasBL>();
            services.AddTransient<IProductosOrigenesBL<ProductosOrigenesDTO>, ProductosOrigenesBL>();
            services.AddTransient<IProductosOrigenesPreciosBL<ProductosOrigenesPreciosDTO>, ProductosOrigenesPreciosBL>();
            services.AddTransient<IProductosPeriodosRestVenFarmBL<ProductosPeriodosRestVenFarmDTO>, ProductosPeriodosRestVenFarmBL>();
            services.AddTransient<IProductosPreciosBL<ProductosPreciosDTO>, ProductosPreciosBL>();
            services.AddTransient<IProductosTemporadasBL<ProductosTemporadasDTO>, ProductosTemporadasBL>();
            services.AddTransient<IProductosTiposBL<ProductosTiposDTO>, ProductosTiposBL>();
            services.AddTransient<IProductosTiposVentasBL<ProductosTiposVentasDTO>, ProductosTiposVentasBL>();
            services.AddTransient<IProductosUbicacionesBL<ProductosUbicacionesDTO>, ProductosUbicacionesBL>();
            services.AddTransient<IProductosUbicacionesPickingBL<ProductosUbicacionesPickingDTO>, ProductosUbicacionesPickingBL>();
            services.AddTransient<IProductosVentasBL<ProductosVentasDTO>, ProductosVentasBL>();
            services.AddTransient<IListaPreciosBL, ListaPreciosBL>();
            services.AddTransient<IEmailsBL<EmailsDTO>, EmailsBL>();
            services.AddTransient<IParametrosBL<ParametrosDTO>, ParametrosBL>();
            services.AddTransient<IMarkupPercentagesUpdateAndDiscountsBL<DataGridItemDTO>, MarkupPercentagesUpdateAndDiscountsBL>();
            services.AddTransient<IProductosDescuentosProveedorBL<ProductosDescuentosProveedorDTO>, ProductosDescuentosProveedorBL>();
            services.AddTransient<ICombosBL<GenericComboDTO>, CombosBL>();

            services.AddTransient<IArchivosBL, ArchivosBL>();

            #region Archivos
            services.AddTransient<IFileTypesBL, FileTypesBL>();
            #endregion

            services.AddTransient<CombosBL>();

            //Dispensas
            services.AddTransient<IDispensasBL<DispensasDTO>, DispensasBL>();
        }
    }
}
