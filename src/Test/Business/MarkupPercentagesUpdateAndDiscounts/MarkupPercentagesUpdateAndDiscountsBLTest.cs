using Business.Interface;
using Business.MarkupPercentagesUpdateAndDiscounts;
using DataAccess;
using DataAccess.Entities;
using DataTransferObject;
using Microsoft.EntityFrameworkCore;
using Moq;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using UnitOfWork;
using Xunit;
using System.Text.Json;
using FluentAssertions;
using System.Data.Entity.Infrastructure;
using Test.Fixtures;

namespace Test.Business.MarkupPercentagesUpdateAndDiscounts
{
    public class MarkupPercentagesUpdateAndDiscountsBLTest
    {
        private readonly IProductosPreciosBL<ProductosPreciosDTO> _productsPricesService;
        private readonly IProductosDescuentosProveedorBL<ProductosDescuentosProveedorDTO> _productsSupplierDiscountsService;
        private readonly IMarkupPercentagesUpdateAndDiscountsFixtures _databaseFixtures;
        private readonly IConnectionFactory _connectionFactory;
        private readonly IBaseRepository<Productos> _productsRepository;
        private readonly IProductosBL<ProductosDTO> _productsService;

        public MarkupPercentagesUpdateAndDiscountsBLTest(
            IBaseRepository<Productos> productsRepository,
            IProductosBL<ProductosDTO> productsService,
            IProductosPreciosBL<ProductosPreciosDTO> productsPricesService,
            IProductosDescuentosProveedorBL<ProductosDescuentosProveedorDTO> productsSupplierDiscountsService,
            IMarkupPercentagesUpdateAndDiscountsFixtures databaseFixtures,
            IConnectionFactory connectionFactory)
        {
            _productsRepository = productsRepository;
            _productsService = productsService;
            _productsPricesService = productsPricesService;
            _productsSupplierDiscountsService = productsSupplierDiscountsService;
            _databaseFixtures = databaseFixtures;
            _connectionFactory = connectionFactory;
        }

        [Fact]
        public async Task ProcessRequestAsync_Should_Update_Discount_Only()
        {
            // Arrange
            var context = _connectionFactory.CreateContextForSQLite();

            IUnitOfWork unitOfWork = new TransactionScopeUnitOfWork(context, IsolationLevel.ReadUncommitted, TransactionScopeOption.Suppress);

            MarkupPercentagesUpdateAndDiscountsBL sut = new(
                _productsRepository,
                _productsService,
                _productsPricesService,
                _productsSupplierDiscountsService);

            #region Setting Data

            _databaseFixtures.SetContext(context);

            int idLaboratorio = _databaseFixtures.GetLaboratoriosEntity()
                .SaveToDatabase(x => x.IdLaboratorio);

            int idProducto = _databaseFixtures.GetProductosEntity()
                .WithChanges(x =>
                {
                    x.IdLaboratorio = idLaboratorio;
                })
                .SaveToDatabase(x => x.IdProducto);

            int idProveedor = _databaseFixtures.GetProveedoresEntity()
                .SaveToDatabase(x => x.IdProveedor);

            int idLaboratorioProveedor = _databaseFixtures.GetLaboratoriosProveedoresEntity()
                .WithChanges(x =>
                {
                    x.IdLaboratorio = idLaboratorio;
                    x.IdProveedor = idProveedor;
                })
                .SaveToDatabase(x => x.IdLaboratorioProveedor);

            int idProductoCompra = _databaseFixtures.GetProductosComprasEntity()
                .WithChanges(x =>
                {
                    x.IdProducto = idProducto;
                    x.IdProveedor = idProveedor;
                })
                .SaveToDatabase(x => x.IdProductoCompra);

            int intProductoPrecio = _databaseFixtures.GetProductosPreciosEntity()
                .WithChanges(x =>
                {
                    x.IdProducto = idProducto;
                    x.MarkUpProveedor = 4.11m;
                    x.MarkUpProveedorVh = 5.33m;
                })
                .SaveToDatabase(x => x.IdProductoPrecio);

            _databaseFixtures.GetProductosDescuentosProveedorEntity()
                .WithChanges(x =>
                {
                    x.IdProducto = idProducto;
                    x.IdDescuentoProveedor = 0;
                    x.CodigoDescuentoProducto = 1;
                    x.PorcentajeDescuentoProveedor = 74.12m;
                    x.PorcentajeDescuentoProveedorVh = null;
                })
                .SaveToDatabase();

            _databaseFixtures.GetProductosDescuentosProveedorEntity()
                .WithChanges(x =>
                {
                    x.IdProducto = idProducto;
                    x.IdDescuentoProveedor = 0;
                    x.CodigoDescuentoProducto = 2;
                    x.PorcentajeDescuentoProveedor = 34.19m;
                    x.PorcentajeDescuentoProveedorVh = null;
                })
                .SaveToDatabase();
            #endregion

            List<int> productIds = new() { idProducto };
            MarkUpDTO markUpDTO = new(null, null);
            List<DiscountDTO> discountDTOs = new() { new(1, 74.12m, null), new(2, 34.19m, null) };
            ProcessRequestDTO request = new(productIds, markUpDTO, discountDTOs, "blazor");

            // Act
            await sut.ProcessRequestAsync(request, unitOfWork);

            unitOfWork = new TransactionScopeUnitOfWork(context, IsolationLevel.ReadUncommitted, TransactionScopeOption.Suppress);
            var products = await sut.GetDataGridByPurchaseCategoryIdAsync(0, unitOfWork);

            // Assert
            Assert.Single(products);
            Assert.Equal(4.11m, products.First().ProductosPrecios__MarkUpProveedor);
            Assert.Equal(5.33m, products.First().ProductosPrecios__MarkUpProveedorVh);
            Assert.Equal("74.12 / 34.19", products.First().ProductosDescuentosProveedor__PorcentajeDescuentoProveedor);
            Assert.Equal("", products.First().ProductosDescuentosProveedor__PorcentajeDescuentoProveedorVh);
        }

        [Fact]
        public async Task ProcessRequestAsync_Should_Save_User_And_Date_Correctly()
        {
            // Arrange  
            var context = _connectionFactory.CreateContextForSQLite();

            IUnitOfWork unitOfWork = new TransactionScopeUnitOfWork(context, IsolationLevel.ReadUncommitted, TransactionScopeOption.Suppress);

            MarkupPercentagesUpdateAndDiscountsBL sut = new(
                _productsRepository,
                _productsService,
                _productsPricesService,
                _productsSupplierDiscountsService);

            #region Setting Data

            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };

            int idProducto = _databaseFixtures.GetProductosEntity()
                .SaveToDatabase(context, x => x.IdProducto);

            int idLaboratorio = _databaseFixtures.GetLaboratoriosEntity()
                .SaveToDatabase(context, x => x.IdLaboratorio);

            int idProveedor = _databaseFixtures.GetProveedoresEntity()
                .SaveToDatabase(context, x => x.IdProveedor);

            int idLaboratorioProveedor = _databaseFixtures.GetLaboratoriosProveedoresEntity()
                .WithChanges(x =>
                {
                    x.IdLaboratorio = idLaboratorio;
                    x.IdProveedor= idProveedor;
                })
                .SaveToDatabase(context, x => x.IdLaboratorioProveedor);

            int idProductoCompra = _databaseFixtures.GetProductosComprasEntity()
                .WithChanges(x =>
                {
                    x.IdProducto = idProducto;
                    x.IdProveedor = idProveedor;
                })
                .SaveToDatabase(context, x => x.IdProductoCompra);

            int idProductoPrecio = _databaseFixtures.GetProductosPreciosEntity()
                .WithChanges(x =>
                {
                    x.IdProducto = idProducto;
                    x.MarkUpProveedor = 4.11m;
                    x.MarkUpProveedorVh = 5.33m;
                })
                .SaveToDatabase(context, x => x.IdProductoPrecio);

            _databaseFixtures.GetProductosDescuentosProveedorEntity()
                .WithChanges(x =>
                {
                    x.IdProducto = idProducto;
                    x.IdDescuentoProveedor = 0;
                    x.CodigoDescuentoProducto = 1;
                    x.PorcentajeDescuentoProveedor = 5.11m;
                    x.PorcentajeDescuentoProveedorVh = 5.11m;
                })
                .SaveToDatabase(context);

            _databaseFixtures.GetProductosDescuentosProveedorEntity()
                .WithChanges(x =>
                {
                    x.IdProducto = idProducto;
                    x.IdDescuentoProveedor = 0;
                    x.CodigoDescuentoProducto = 2;
                    x.PorcentajeDescuentoProveedor = 3.11m;
                    x.PorcentajeDescuentoProveedorVh = 9.11m;
                })
                .SaveToDatabase(context);

            //Act  
            #endregion

            List<int> productIds = new() { idProducto };
            MarkUpDTO markUpDTO = new(33.14m, 48.58m);
            List<DiscountDTO> discountDTOs = new() { new(1, 74.12m, 36.35m), new(2, 34.19m, 78.48m) };
            var usuario = Guid.NewGuid().ToString();
            ProcessRequestDTO request = new(productIds, markUpDTO, discountDTOs, usuario);

            // Act
            var date = DateTime.Now;
            await sut.ProcessRequestAsync(request, unitOfWork);

            unitOfWork = new TransactionScopeUnitOfWork(context, IsolationLevel.ReadUncommitted, TransactionScopeOption.Suppress);

            var productPrice = (await _productsPricesService.GetByProductIdsAsync(productIds, x => new ProductosPreciosDTO 
            { 
                UsuarioUltimaModificacion = x.UsuarioUltimaModificacion,
                FechaUltimaModificacion = x.FechaUltimaModificacion,
            }, unitOfWork)).FirstOrDefault();

            var productSupplierDiscounts = (await _productsSupplierDiscountsService.GetByProductIdsAsync(productIds, x => new ProductosDescuentosProveedorDTO
            {
                UsuarioCreacion = x.UsuarioCreacion,
                UsuarioUltimaModificacion = x.UsuarioUltimaModificacion,
                FechaUltimaModificacion = x.FechaUltimaModificacion,
            }, unitOfWork)).FirstOrDefault();

            // Assert
            Assert.Equal(usuario, productPrice.UsuarioUltimaModificacion);
            Assert.Equal(date.ToString("f"), productPrice.FechaUltimaModificacion.Value.ToString("f"));
            Assert.Equal(usuario, productSupplierDiscounts.UsuarioCreacion);
            Assert.Equal(usuario, productSupplierDiscounts.UsuarioUltimaModificacion);
            Assert.Equal(date.ToString("f"), productSupplierDiscounts.FechaUltimaModificacion.Value.ToString("f"));
        }

        [Fact]
        public async Task ProcessRequestAsync_Should_Update_Discounts_Only()
        {
            // Arrange  
            var context = _connectionFactory.CreateContextForSQLite();

            IUnitOfWork unitOfWork = new TransactionScopeUnitOfWork(context, IsolationLevel.ReadUncommitted, TransactionScopeOption.Suppress);

            MarkupPercentagesUpdateAndDiscountsBL sut = new(
                _productsRepository,
                _productsService,
                _productsPricesService,
                _productsSupplierDiscountsService);

            #region Setting Data

            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };

            var producto = JsonSerializer.Deserialize<Productos>(@"
            {
                ""IdProducto"": 362843,
                ""NombreProducto"": ""ACCU -CHEK HOSP. GUIDE GLUC TIRAS ENV X 50"",
                ""NombreProductoCompleto"": ""ACCU -CHEK HOSP. GUIDE GLUC TIRAS ENV X 50"",
                ""IdDroga"": -1,
                ""IdAccionTerapeutica"": 533,
                ""IdLaboratorio"": 2479,
                ""IdTipoProducto"": 1,
                ""IdMarca"": -1,
                ""Numero"": ""A427939"",
                ""CodigoPAP"": 56793108,
                ""PorcentajeMargenDrogueria"": 9.00,
                ""EsProductoNuevo"": true,
                ""Certificado"": ""18 -287"",
                ""UnidadesMinimasPicking"": 1,
                ""UnidadesMaximasPicking"": 999999,
                ""SeTrabajaEnInstitucionales"": true,
                ""ExclusivoDeInstitucionales"": true,
                ""SeCompraEnInstitucionales"": true,
                ""IdEstado"": 29,
                ""FechaAlta"": ""2017-12-20T12:48:00"",
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-10-25T21:36:54.523"",
                ""Troquel"": ""9911000""
            }", options);

            var laboratorio = JsonSerializer.Deserialize<Laboratorios>(@"
            {
                ""IdLaboratorio"": 2479,
                ""Cuit"": 30714921335,
                ""RazonSocial"": ""ROCHE DIABETES CARE ARGENTINA S.A."",
                ""Direccion"": ""AV. LEANDRO N ALEM 986 PISO 10"",
                ""IdLocalidad"": 93,
                ""IdProvincia"": 2,
                ""CPA"": ""1001"",
                ""PideNumeroDeTransfer"": false,
                ""AceptaMalEstado"": true,
                ""IdEstado"": 2,
                ""IdFormatoArchivo"": -1,
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-09-03T15:17:58.697""
            }", options);

            var laboratorioProveedor = JsonSerializer.Deserialize<LaboratoriosProveedores>(@"
            {
                ""IdLaboratorioProveedor"": 6422,
                ""IdLaboratorio"": 2479,
                ""IdProveedor"": 4696,
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-09-07T10:34:18.027""
            }", options);

            var proveedor = JsonSerializer.Deserialize<Proveedores>(@"
            {
                ""IdProveedor"": 4696,
                ""Cuit"": 30538474858,
                ""RazonSocial"": ""ROFINA S.A.I.C.F."",
                ""Direccion"": ""HIPOLITO YRIGOYEN 476"",
                ""IdLocalidad"": 93,
                ""IdProvincia"": 2,
                ""CPA"": ""C1086AAF"",
                ""Telefono"": ""(011) 4334-9812\/15\/17"",
                ""EMail"": ""fizumi@rofina.com.ar"",
                ""SitioWeb"": ""Certificados rperez@rofina.com.ar ; joneglia@rofina.com.ar"",
                ""NumeroCuentaCorriente"": 140,
                ""DestinatarioDelCheque"": ""ROFINA S.A.I.C.F."",
                ""NCRObSocPorLaboratorio"": true,
                ""PideNumeroDeTransfer"": true,
                ""GeneraOrdenesDeCompraEspeciales"": false,
                ""GLN"": ""7798157560005"",
                ""IdTipoProv"": 1,
                ""CondicionEnDias"": 30,
                ""GeneraComprobanteObraSocial"": false,
                ""ProveedorSiempreAPicking"": false,
                ""EmiteFacturaDeCredito"": false,
                ""AplicaNCObrasSociales"": false,
                ""IdEstado"": 2,
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-09-01T15:45:46.277""
            }", options);

            var productoCompra = JsonSerializer.Deserialize<ProductosCompras>(@"
            {
                ""IdProductoCompra"": 56115,
                ""IdProducto"": 362843,
                ""DisponibleParaLaCompra"": true,
                ""IdProveedor"": 4696,
                ""IdRubroCompra"": 3639,
                ""CodigoInternoEnProveedor"": ""437453736036"",
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-10-28T00:30:56.143"",
                ""EnviaEmpaqueCerradoEnGR"": false,
                ""NetoParaElProveedor"": false,
                ""AceptaVencidos"": false
            }", options);

            var productoPrecio = JsonSerializer.Deserialize<ProductosPrecios>(@"
            {
                ""IdProductoPrecio"": 761884,
                ""IdProducto"": 362843,
                ""Precio"": 570.00,
                ""Costo"": 324.90,
                ""FechaPrecio"": ""2022-02-05"",
                ""FechaCosto"": ""2022-02-05"",
                ""PVPNeto"": 0.00,
                ""IdProductoOrigenPrecio"": 4,
                ""NumeroListaPrecios"": ""3"",
                ""FechaListaPrecios"": ""2018-10-24"",
                ""LoginCambioPrecio"": ""blazor"",
                ""MarkUpProveedor"": 4.11,
                ""CostoVH"": 324.90,
                ""PrecioVH"": 324.90,
                ""FechaCostoVH"": ""2022-02-05"",
                ""FechaPrecioVH"": ""2022-02-05"",
                ""MarkUpProveedorVH"": 5.33,
                ""LoginCambioPrecioVH"": ""blazor"",
                ""PorcentajeARestarAlDtoDirectoDelCliente"": 0.00,
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-10-27T23:05:58.700"",
                ""UsuarioUltimaModificacion"": ""67e7cb16-0d54-4cb2-9e1b-0f3998b8d57f"",
                ""FechaUltimaModificacion"": ""2022-02-17T10:52:12.170""
            }", options);

            var productoDescuentosProveedor = JsonSerializer.Deserialize<List<ProductosDescuentosProveedor>>(@"
            [{
                ""IdDescuentoProveedor"": 54028,
                ""IdProducto"": 362843,
                ""CodigoDescuentoProducto"": 1,
                ""PorcentajeDescuentoProveedor"": 5.11,
                ""PorcentajeDescuentoProveedorVH"": 5.11,
                ""UsuarioCreacion"": ""67e7cb16-0d54-4cb2-9e1b-0f3998b8d57f"",
                ""FechaCreacion"": ""2022-02-17T10:52:12.170"",
                ""UsuarioUltimaModificacion"": ""67e7cb16-0d54-4cb2-9e1b-0f3998b8d57f"",
                ""FechaUltimaModificacion"": ""2022-02-17T10:52:12.170""
            },
            {
                ""IdDescuentoProveedor"": 54029,
                ""IdProducto"": 362843,
                ""CodigoDescuentoProducto"": 2,
                ""PorcentajeDescuentoProveedor"": 3.11,
                ""PorcentajeDescuentoProveedorVH"": 9.11,
                ""UsuarioCreacion"": ""67e7cb16-0d54-4cb2-9e1b-0f3998b8d57f"",
                ""FechaCreacion"": ""2022-02-17T10:52:12.170"",
                ""UsuarioUltimaModificacion"": ""67e7cb16-0d54-4cb2-9e1b-0f3998b8d57f"",
                ""FechaUltimaModificacion"": ""2022-02-17T10:52:12.170""
            }]", options);

            //Act  

            context.Productos.Add(producto);
            context.Laboratorios.Add(laboratorio);
            context.LaboratoriosProveedores.Add(laboratorioProveedor);
            context.Proveedores.Add(proveedor);
            context.ProductosCompras.Add(productoCompra);
            context.ProductosPrecios.Add(productoPrecio);
            context.ProductosDescuentosProveedor.AddRange(productoDescuentosProveedor);
            context.SaveChanges();
            context.ChangeTracker.Clear();
            #endregion

            List<int> productIds = new() { 362843 };
            MarkUpDTO markUpDTO = new(null, null);
            List<DiscountDTO> discountDTOs = new() { new(1, 74.12m, 36.35m), new(2, 34.19m, 78.48m) };
            ProcessRequestDTO request = new(productIds, markUpDTO, discountDTOs, "blazor");

            // Act
            await sut.ProcessRequestAsync(request, unitOfWork);

            unitOfWork = new TransactionScopeUnitOfWork(context, IsolationLevel.ReadUncommitted, TransactionScopeOption.Suppress);
            var products = await sut.GetDataGridByPurchaseCategoryIdAsync(3639, unitOfWork);

            // Assert
            Assert.Single(products);
            Assert.Equal(4.11m, products.First().ProductosPrecios__MarkUpProveedor);
            Assert.Equal(5.33m, products.First().ProductosPrecios__MarkUpProveedorVh);
            Assert.Equal("74.12 / 34.19", products.First().ProductosDescuentosProveedor__PorcentajeDescuentoProveedor);
            Assert.Equal("36.35 / 78.48", products.First().ProductosDescuentosProveedor__PorcentajeDescuentoProveedorVh);
        }

        [Fact]
        public async Task ProcessRequestAsync_Should_Update_MarkupVh_Only()
        {
            // Arrange  
            var context = _connectionFactory.CreateContextForSQLite();

            IUnitOfWork unitOfWork = new TransactionScopeUnitOfWork(context, IsolationLevel.ReadUncommitted, TransactionScopeOption.Suppress);

            MarkupPercentagesUpdateAndDiscountsBL sut = new(
                _productsRepository,
                _productsService,
                _productsPricesService,
                _productsSupplierDiscountsService);

            #region Setting Data

            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };

            var producto = JsonSerializer.Deserialize<Productos>(@"
            {
                ""IdProducto"": 362843,
                ""NombreProducto"": ""ACCU -CHEK HOSP. GUIDE GLUC TIRAS ENV X 50"",
                ""NombreProductoCompleto"": ""ACCU -CHEK HOSP. GUIDE GLUC TIRAS ENV X 50"",
                ""IdDroga"": -1,
                ""IdAccionTerapeutica"": 533,
                ""IdLaboratorio"": 2479,
                ""IdTipoProducto"": 1,
                ""IdMarca"": -1,
                ""Numero"": ""A427939"",
                ""CodigoPAP"": 56793108,
                ""PorcentajeMargenDrogueria"": 9.00,
                ""EsProductoNuevo"": true,
                ""Certificado"": ""18 -287"",
                ""UnidadesMinimasPicking"": 1,
                ""UnidadesMaximasPicking"": 999999,
                ""SeTrabajaEnInstitucionales"": true,
                ""ExclusivoDeInstitucionales"": true,
                ""SeCompraEnInstitucionales"": true,
                ""IdEstado"": 29,
                ""FechaAlta"": ""2017-12-20T12:48:00"",
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-10-25T21:36:54.523"",
                ""Troquel"": ""9911000""
            }", options);

            var laboratorio = JsonSerializer.Deserialize<Laboratorios>(@"
            {
                ""IdLaboratorio"": 2479,
                ""Cuit"": 30714921335,
                ""RazonSocial"": ""ROCHE DIABETES CARE ARGENTINA S.A."",
                ""Direccion"": ""AV. LEANDRO N ALEM 986 PISO 10"",
                ""IdLocalidad"": 93,
                ""IdProvincia"": 2,
                ""CPA"": ""1001"",
                ""PideNumeroDeTransfer"": false,
                ""AceptaMalEstado"": true,
                ""IdEstado"": 2,
                ""IdFormatoArchivo"": -1,
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-09-03T15:17:58.697""
            }", options);

            var laboratorioProveedor = JsonSerializer.Deserialize<LaboratoriosProveedores>(@"
            {
                ""IdLaboratorioProveedor"": 6422,
                ""IdLaboratorio"": 2479,
                ""IdProveedor"": 4696,
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-09-07T10:34:18.027""
            }", options);

            var proveedor = JsonSerializer.Deserialize<Proveedores>(@"
            {
                ""IdProveedor"": 4696,
                ""Cuit"": 30538474858,
                ""RazonSocial"": ""ROFINA S.A.I.C.F."",
                ""Direccion"": ""HIPOLITO YRIGOYEN 476"",
                ""IdLocalidad"": 93,
                ""IdProvincia"": 2,
                ""CPA"": ""C1086AAF"",
                ""Telefono"": ""(011) 4334-9812\/15\/17"",
                ""EMail"": ""fizumi@rofina.com.ar"",
                ""SitioWeb"": ""Certificados rperez@rofina.com.ar ; joneglia@rofina.com.ar"",
                ""NumeroCuentaCorriente"": 140,
                ""DestinatarioDelCheque"": ""ROFINA S.A.I.C.F."",
                ""NCRObSocPorLaboratorio"": true,
                ""PideNumeroDeTransfer"": true,
                ""GeneraOrdenesDeCompraEspeciales"": false,
                ""GLN"": ""7798157560005"",
                ""IdTipoProv"": 1,
                ""CondicionEnDias"": 30,
                ""GeneraComprobanteObraSocial"": false,
                ""ProveedorSiempreAPicking"": false,
                ""EmiteFacturaDeCredito"": false,
                ""AplicaNCObrasSociales"": false,
                ""IdEstado"": 2,
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-09-01T15:45:46.277""
            }", options);

            var productoCompra = JsonSerializer.Deserialize<ProductosCompras>(@"
            {
                ""IdProductoCompra"": 56115,
                ""IdProducto"": 362843,
                ""DisponibleParaLaCompra"": true,
                ""IdProveedor"": 4696,
                ""IdRubroCompra"": 3639,
                ""CodigoInternoEnProveedor"": ""437453736036"",
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-10-28T00:30:56.143"",
                ""EnviaEmpaqueCerradoEnGR"": false,
                ""NetoParaElProveedor"": false,
                ""AceptaVencidos"": false
            }", options);

            var productoPrecio = JsonSerializer.Deserialize<ProductosPrecios>(@"
            {
                ""IdProductoPrecio"": 761884,
                ""IdProducto"": 362843,
                ""Precio"": 570.00,
                ""Costo"": 324.90,
                ""FechaPrecio"": ""2022-02-05"",
                ""FechaCosto"": ""2022-02-05"",
                ""PVPNeto"": 0.00,
                ""IdProductoOrigenPrecio"": 4,
                ""NumeroListaPrecios"": ""3"",
                ""FechaListaPrecios"": ""2018-10-24"",
                ""LoginCambioPrecio"": ""blazor"",
                ""MarkUpProveedor"": 4.11,
                ""CostoVH"": 324.90,
                ""PrecioVH"": 324.90,
                ""FechaCostoVH"": ""2022-02-05"",
                ""FechaPrecioVH"": ""2022-02-05"",
                ""MarkUpProveedorVH"": 5.33,
                ""LoginCambioPrecioVH"": ""blazor"",
                ""PorcentajeARestarAlDtoDirectoDelCliente"": 0.00,
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-10-27T23:05:58.700"",
                ""UsuarioUltimaModificacion"": ""67e7cb16-0d54-4cb2-9e1b-0f3998b8d57f"",
                ""FechaUltimaModificacion"": ""2022-02-17T10:52:12.170""
            }", options);

            var productoDescuentosProveedor = JsonSerializer.Deserialize<List<ProductosDescuentosProveedor>>(@"
            [{
                ""IdDescuentoProveedor"": 54028,
                ""IdProducto"": 362843,
                ""CodigoDescuentoProducto"": 1,
                ""PorcentajeDescuentoProveedor"": 5.11,
                ""PorcentajeDescuentoProveedorVH"": 5.11,
                ""UsuarioCreacion"": ""67e7cb16-0d54-4cb2-9e1b-0f3998b8d57f"",
                ""FechaCreacion"": ""2022-02-17T10:52:12.170"",
                ""UsuarioUltimaModificacion"": ""67e7cb16-0d54-4cb2-9e1b-0f3998b8d57f"",
                ""FechaUltimaModificacion"": ""2022-02-17T10:52:12.170""
            },
            {
                ""IdDescuentoProveedor"": 54029,
                ""IdProducto"": 362843,
                ""CodigoDescuentoProducto"": 2,
                ""PorcentajeDescuentoProveedor"": 3.11,
                ""PorcentajeDescuentoProveedorVH"": 9.11,
                ""UsuarioCreacion"": ""67e7cb16-0d54-4cb2-9e1b-0f3998b8d57f"",
                ""FechaCreacion"": ""2022-02-17T10:52:12.170"",
                ""UsuarioUltimaModificacion"": ""67e7cb16-0d54-4cb2-9e1b-0f3998b8d57f"",
                ""FechaUltimaModificacion"": ""2022-02-17T10:52:12.170""
            }]", options);

            //Act  

            context.Productos.Add(producto);
            context.Laboratorios.Add(laboratorio);
            context.LaboratoriosProveedores.Add(laboratorioProveedor);
            context.Proveedores.Add(proveedor);
            context.ProductosCompras.Add(productoCompra);
            context.ProductosPrecios.Add(productoPrecio);
            context.ProductosDescuentosProveedor.AddRange(productoDescuentosProveedor);
            context.SaveChanges();
            context.ChangeTracker.Clear();
            #endregion

            List<int> productIds = new() { 362843 };
            MarkUpDTO markUpDTO = new(null, 55.57m);
            List<DiscountDTO> discountDTOs = new() { };
            ProcessRequestDTO request = new(productIds, markUpDTO, discountDTOs, "blazor");

            // Act
            await sut.ProcessRequestAsync(request, unitOfWork);

            unitOfWork = new TransactionScopeUnitOfWork(context, IsolationLevel.ReadUncommitted, TransactionScopeOption.Suppress);
            var products = await sut.GetDataGridByPurchaseCategoryIdAsync(3639, unitOfWork);

            // Assert
            Assert.Single(products);
            Assert.Equal(4.11m, products.First().ProductosPrecios__MarkUpProveedor);
            Assert.Equal(55.57m, products.First().ProductosPrecios__MarkUpProveedorVh);
            Assert.Equal("5.11 / 3.11", products.First().ProductosDescuentosProveedor__PorcentajeDescuentoProveedor);
            Assert.Equal("5.11 / 9.11", products.First().ProductosDescuentosProveedor__PorcentajeDescuentoProveedorVh);
        }

        [Fact]
        public async Task ProcessRequestAsync_Should_Update_Markup_Only()
        {
            // Arrange  
            var context = _connectionFactory.CreateContextForSQLite();

            IUnitOfWork unitOfWork = new TransactionScopeUnitOfWork(context, IsolationLevel.ReadUncommitted, TransactionScopeOption.Suppress);

            MarkupPercentagesUpdateAndDiscountsBL sut = new(
                _productsRepository,
                _productsService,
                _productsPricesService,
                _productsSupplierDiscountsService);

            #region Setting Data

            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };

            var producto = JsonSerializer.Deserialize<Productos>(@"
            {
                ""IdProducto"": 362843,
                ""NombreProducto"": ""ACCU -CHEK HOSP. GUIDE GLUC TIRAS ENV X 50"",
                ""NombreProductoCompleto"": ""ACCU -CHEK HOSP. GUIDE GLUC TIRAS ENV X 50"",
                ""IdDroga"": -1,
                ""IdAccionTerapeutica"": 533,
                ""IdLaboratorio"": 2479,
                ""IdTipoProducto"": 1,
                ""IdMarca"": -1,
                ""Numero"": ""A427939"",
                ""CodigoPAP"": 56793108,
                ""PorcentajeMargenDrogueria"": 9.00,
                ""EsProductoNuevo"": true,
                ""Certificado"": ""18 -287"",
                ""UnidadesMinimasPicking"": 1,
                ""UnidadesMaximasPicking"": 999999,
                ""SeTrabajaEnInstitucionales"": true,
                ""ExclusivoDeInstitucionales"": true,
                ""SeCompraEnInstitucionales"": true,
                ""IdEstado"": 29,
                ""FechaAlta"": ""2017-12-20T12:48:00"",
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-10-25T21:36:54.523"",
                ""Troquel"": ""9911000""
            }", options);

            var laboratorio = JsonSerializer.Deserialize<Laboratorios>(@"
            {
                ""IdLaboratorio"": 2479,
                ""Cuit"": 30714921335,
                ""RazonSocial"": ""ROCHE DIABETES CARE ARGENTINA S.A."",
                ""Direccion"": ""AV. LEANDRO N ALEM 986 PISO 10"",
                ""IdLocalidad"": 93,
                ""IdProvincia"": 2,
                ""CPA"": ""1001"",
                ""PideNumeroDeTransfer"": false,
                ""AceptaMalEstado"": true,
                ""IdEstado"": 2,
                ""IdFormatoArchivo"": -1,
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-09-03T15:17:58.697""
            }", options);

            var laboratorioProveedor = JsonSerializer.Deserialize<LaboratoriosProveedores>(@"
            {
                ""IdLaboratorioProveedor"": 6422,
                ""IdLaboratorio"": 2479,
                ""IdProveedor"": 4696,
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-09-07T10:34:18.027""
            }", options);

            var proveedor = JsonSerializer.Deserialize<Proveedores>(@"
            {
                ""IdProveedor"": 4696,
                ""Cuit"": 30538474858,
                ""RazonSocial"": ""ROFINA S.A.I.C.F."",
                ""Direccion"": ""HIPOLITO YRIGOYEN 476"",
                ""IdLocalidad"": 93,
                ""IdProvincia"": 2,
                ""CPA"": ""C1086AAF"",
                ""Telefono"": ""(011) 4334-9812\/15\/17"",
                ""EMail"": ""fizumi@rofina.com.ar"",
                ""SitioWeb"": ""Certificados rperez@rofina.com.ar ; joneglia@rofina.com.ar"",
                ""NumeroCuentaCorriente"": 140,
                ""DestinatarioDelCheque"": ""ROFINA S.A.I.C.F."",
                ""NCRObSocPorLaboratorio"": true,
                ""PideNumeroDeTransfer"": true,
                ""GeneraOrdenesDeCompraEspeciales"": false,
                ""GLN"": ""7798157560005"",
                ""IdTipoProv"": 1,
                ""CondicionEnDias"": 30,
                ""GeneraComprobanteObraSocial"": false,
                ""ProveedorSiempreAPicking"": false,
                ""EmiteFacturaDeCredito"": false,
                ""AplicaNCObrasSociales"": false,
                ""IdEstado"": 2,
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-09-01T15:45:46.277""
            }", options);

            var productoCompra = JsonSerializer.Deserialize<ProductosCompras>(@"
            {
                ""IdProductoCompra"": 56115,
                ""IdProducto"": 362843,
                ""DisponibleParaLaCompra"": true,
                ""IdProveedor"": 4696,
                ""IdRubroCompra"": 3639,
                ""CodigoInternoEnProveedor"": ""437453736036"",
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-10-28T00:30:56.143"",
                ""EnviaEmpaqueCerradoEnGR"": false,
                ""NetoParaElProveedor"": false,
                ""AceptaVencidos"": false
            }", options);

            var productoPrecio = JsonSerializer.Deserialize<ProductosPrecios>(@"
            {
                ""IdProductoPrecio"": 761884,
                ""IdProducto"": 362843,
                ""Precio"": 570.00,
                ""Costo"": 324.90,
                ""FechaPrecio"": ""2022-02-05"",
                ""FechaCosto"": ""2022-02-05"",
                ""PVPNeto"": 0.00,
                ""IdProductoOrigenPrecio"": 4,
                ""NumeroListaPrecios"": ""3"",
                ""FechaListaPrecios"": ""2018-10-24"",
                ""LoginCambioPrecio"": ""blazor"",
                ""MarkUpProveedor"": 4.11,
                ""CostoVH"": 324.90,
                ""PrecioVH"": 324.90,
                ""FechaCostoVH"": ""2022-02-05"",
                ""FechaPrecioVH"": ""2022-02-05"",
                ""MarkUpProveedorVH"": 5.33,
                ""LoginCambioPrecioVH"": ""blazor"",
                ""PorcentajeARestarAlDtoDirectoDelCliente"": 0.00,
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-10-27T23:05:58.700"",
                ""UsuarioUltimaModificacion"": ""67e7cb16-0d54-4cb2-9e1b-0f3998b8d57f"",
                ""FechaUltimaModificacion"": ""2022-02-17T10:52:12.170""
            }", options);

            var productoDescuentosProveedor = JsonSerializer.Deserialize<List<ProductosDescuentosProveedor>>(@"
            [{
                ""IdDescuentoProveedor"": 54028,
                ""IdProducto"": 362843,
                ""CodigoDescuentoProducto"": 1,
                ""PorcentajeDescuentoProveedor"": 5.11,
                ""PorcentajeDescuentoProveedorVH"": 5.11,
                ""UsuarioCreacion"": ""67e7cb16-0d54-4cb2-9e1b-0f3998b8d57f"",
                ""FechaCreacion"": ""2022-02-17T10:52:12.170"",
                ""UsuarioUltimaModificacion"": ""67e7cb16-0d54-4cb2-9e1b-0f3998b8d57f"",
                ""FechaUltimaModificacion"": ""2022-02-17T10:52:12.170""
            },
            {
                ""IdDescuentoProveedor"": 54029,
                ""IdProducto"": 362843,
                ""CodigoDescuentoProducto"": 2,
                ""PorcentajeDescuentoProveedor"": 3.11,
                ""PorcentajeDescuentoProveedorVH"": 9.11,
                ""UsuarioCreacion"": ""67e7cb16-0d54-4cb2-9e1b-0f3998b8d57f"",
                ""FechaCreacion"": ""2022-02-17T10:52:12.170"",
                ""UsuarioUltimaModificacion"": ""67e7cb16-0d54-4cb2-9e1b-0f3998b8d57f"",
                ""FechaUltimaModificacion"": ""2022-02-17T10:52:12.170""
            }]", options);

            //Act  

            context.Productos.Add(producto);
            context.Laboratorios.Add(laboratorio);
            context.LaboratoriosProveedores.Add(laboratorioProveedor);
            context.Proveedores.Add(proveedor);
            context.ProductosCompras.Add(productoCompra);
            context.ProductosPrecios.Add(productoPrecio);
            context.ProductosDescuentosProveedor.AddRange(productoDescuentosProveedor);
            context.SaveChanges();
            context.ChangeTracker.Clear();
            #endregion

            List<int> productIds = new() { 362843 };
            MarkUpDTO markUpDTO = new(55.57m, null);
            List<DiscountDTO> discountDTOs = new() { };
            ProcessRequestDTO request = new(productIds, markUpDTO, discountDTOs, "blazor");

            // Act
            await sut.ProcessRequestAsync(request, unitOfWork);

            unitOfWork = new TransactionScopeUnitOfWork(context, IsolationLevel.ReadUncommitted, TransactionScopeOption.Suppress);
            var products = await sut.GetDataGridByPurchaseCategoryIdAsync(3639, unitOfWork);

            //Assert 
            Assert.Single(products);
            Assert.Equal(55.57m, products.First().ProductosPrecios__MarkUpProveedor);
            Assert.Equal(5.33m, products.First().ProductosPrecios__MarkUpProveedorVh);
            Assert.Equal("5.11 / 3.11", products.First().ProductosDescuentosProveedor__PorcentajeDescuentoProveedor);
            Assert.Equal("5.11 / 9.11", products.First().ProductosDescuentosProveedor__PorcentajeDescuentoProveedorVh);
        }

        [Fact]
        public async Task ProcessRequestAsync_Should_Not_Update_Vh_WhenProduct_IsNot_Institutional()
        {
            // Arrange  
            var context = _connectionFactory.CreateContextForSQLite();

            IUnitOfWork unitOfWork = new TransactionScopeUnitOfWork(context, IsolationLevel.ReadUncommitted, TransactionScopeOption.Suppress);

            MarkupPercentagesUpdateAndDiscountsBL sut = new(
                _productsRepository,
                _productsService,
                _productsPricesService,
                _productsSupplierDiscountsService);

            #region Setting Data

            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };

            var producto = JsonSerializer.Deserialize<Productos>(@"
            {
                ""IdProducto"": 362843,
                ""NombreProducto"": ""ACCU -CHEK HOSP. GUIDE GLUC TIRAS ENV X 50"",
                ""NombreProductoCompleto"": ""ACCU -CHEK HOSP. GUIDE GLUC TIRAS ENV X 50"",
                ""IdDroga"": -1,
                ""IdAccionTerapeutica"": 533,
                ""IdLaboratorio"": 2479,
                ""IdTipoProducto"": 1,
                ""IdMarca"": -1,
                ""Numero"": ""A427939"",
                ""CodigoPAP"": 56793108,
                ""PorcentajeMargenDrogueria"": 9.00,
                ""EsProductoNuevo"": true,
                ""Certificado"": ""18 -287"",
                ""UnidadesMinimasPicking"": 1,
                ""UnidadesMaximasPicking"": 999999,
                ""SeTrabajaEnInstitucionales"": false,
                ""ExclusivoDeInstitucionales"": true,
                ""SeCompraEnInstitucionales"": false,
                ""IdEstado"": 29,
                ""FechaAlta"": ""2017-12-20T12:48:00"",
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-10-25T21:36:54.523"",
                ""Troquel"": ""9911000""
            }", options);

            var laboratorio = JsonSerializer.Deserialize<Laboratorios>(@"
            {
                ""IdLaboratorio"": 2479,
                ""Cuit"": 30714921335,
                ""RazonSocial"": ""ROCHE DIABETES CARE ARGENTINA S.A."",
                ""Direccion"": ""AV. LEANDRO N ALEM 986 PISO 10"",
                ""IdLocalidad"": 93,
                ""IdProvincia"": 2,
                ""CPA"": ""1001"",
                ""PideNumeroDeTransfer"": false,
                ""AceptaMalEstado"": true,
                ""IdEstado"": 2,
                ""IdFormatoArchivo"": -1,
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-09-03T15:17:58.697""
            }", options);

            var laboratorioProveedor = JsonSerializer.Deserialize<LaboratoriosProveedores>(@"
            {
                ""IdLaboratorioProveedor"": 6422,
                ""IdLaboratorio"": 2479,
                ""IdProveedor"": 4696,
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-09-07T10:34:18.027""
            }", options);

            var proveedor = JsonSerializer.Deserialize<Proveedores>(@"
            {
                ""IdProveedor"": 4696,
                ""Cuit"": 30538474858,
                ""RazonSocial"": ""ROFINA S.A.I.C.F."",
                ""Direccion"": ""HIPOLITO YRIGOYEN 476"",
                ""IdLocalidad"": 93,
                ""IdProvincia"": 2,
                ""CPA"": ""C1086AAF"",
                ""Telefono"": ""(011) 4334-9812\/15\/17"",
                ""EMail"": ""fizumi@rofina.com.ar"",
                ""SitioWeb"": ""Certificados rperez@rofina.com.ar ; joneglia@rofina.com.ar"",
                ""NumeroCuentaCorriente"": 140,
                ""DestinatarioDelCheque"": ""ROFINA S.A.I.C.F."",
                ""NCRObSocPorLaboratorio"": true,
                ""PideNumeroDeTransfer"": true,
                ""GeneraOrdenesDeCompraEspeciales"": false,
                ""GLN"": ""7798157560005"",
                ""IdTipoProv"": 1,
                ""CondicionEnDias"": 30,
                ""GeneraComprobanteObraSocial"": false,
                ""ProveedorSiempreAPicking"": false,
                ""EmiteFacturaDeCredito"": false,
                ""AplicaNCObrasSociales"": false,
                ""IdEstado"": 2,
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-09-01T15:45:46.277""
            }", options);

            var productoCompra = JsonSerializer.Deserialize<ProductosCompras>(@"
            {
                ""IdProductoCompra"": 56115,
                ""IdProducto"": 362843,
                ""DisponibleParaLaCompra"": true,
                ""IdProveedor"": 4696,
                ""IdRubroCompra"": 3639,
                ""CodigoInternoEnProveedor"": ""437453736036"",
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-10-28T00:30:56.143"",
                ""EnviaEmpaqueCerradoEnGR"": false,
                ""NetoParaElProveedor"": false,
                ""AceptaVencidos"": false
            }", options);

            var productoPrecio = JsonSerializer.Deserialize<ProductosPrecios>(@"
            {
                ""IdProductoPrecio"": 761884,
                ""IdProducto"": 362843,
                ""Precio"": 570.00,
                ""Costo"": 324.90,
                ""FechaPrecio"": ""2022-02-05"",
                ""FechaCosto"": ""2022-02-05"",
                ""PVPNeto"": 0.00,
                ""IdProductoOrigenPrecio"": 4,
                ""NumeroListaPrecios"": ""3"",
                ""FechaListaPrecios"": ""2018-10-24"",
                ""LoginCambioPrecio"": ""blazor"",
                ""MarkUpProveedor"": 4.00,
                ""CostoVH"": 324.90,
                ""PrecioVH"": 324.90,
                ""FechaCostoVH"": ""2022-02-05"",
                ""FechaPrecioVH"": ""2022-02-05"",
                ""MarkUpProveedorVH"": null,
                ""LoginCambioPrecioVH"": ""blazor"",
                ""PorcentajeARestarAlDtoDirectoDelCliente"": 0.00,
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-10-27T23:05:58.700"",
                ""UsuarioUltimaModificacion"": ""67e7cb16-0d54-4cb2-9e1b-0f3998b8d57f"",
                ""FechaUltimaModificacion"": ""2022-02-17T10:52:12.170""
            }", options);

            //Act  

            context.Productos.Add(producto);
            context.Laboratorios.Add(laboratorio);
            context.LaboratoriosProveedores.Add(laboratorioProveedor);
            context.Proveedores.Add(proveedor);
            context.ProductosCompras.Add(productoCompra);
            context.ProductosPrecios.Add(productoPrecio);
            context.SaveChanges();
            context.ChangeTracker.Clear();
            #endregion

            List<int> productIds = new() { 362843 };
            MarkUpDTO markUpDTO = new(55.55m, 10.11m);
            List<DiscountDTO> discountDTOs = new() { new(1, 74.12m, 36.35m), new(2, 34.19m, 78.48m) };
            ProcessRequestDTO request = new(productIds, markUpDTO, discountDTOs, "blazor");

            // Act
            await sut.ProcessRequestAsync(request, unitOfWork);

            unitOfWork = new TransactionScopeUnitOfWork(context, IsolationLevel.ReadUncommitted, TransactionScopeOption.Suppress);
            var products = await sut.GetDataGridByPurchaseCategoryIdAsync(3639, unitOfWork);

            //Assert 
            Assert.Single(products);
            Assert.Equal(55.55m, products.First().ProductosPrecios__MarkUpProveedor);
            Assert.Null(products.First().ProductosPrecios__MarkUpProveedorVh);
            Assert.Equal("74.12 / 34.19", products.First().ProductosDescuentosProveedor__PorcentajeDescuentoProveedor);
            Assert.Empty(products.First().ProductosDescuentosProveedor__PorcentajeDescuentoProveedorVh);
        }

        [Fact]
        public async Task ProcessRequestAsync_Should_Update_Vh_WhenProduct_Is_Institutional()
        {
            // Arrange  
            var context = _connectionFactory.CreateContextForSQLite();

            IUnitOfWork unitOfWork = new TransactionScopeUnitOfWork(context, IsolationLevel.ReadUncommitted, TransactionScopeOption.Suppress);

            MarkupPercentagesUpdateAndDiscountsBL sut = new(
                _productsRepository,
                _productsService,
                _productsPricesService,
                _productsSupplierDiscountsService);

            #region Setting Data

            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };

            var producto = JsonSerializer.Deserialize<Productos>(@"
            {
                ""IdProducto"": 362843,
                ""NombreProducto"": ""ACCU -CHEK HOSP. GUIDE GLUC TIRAS ENV X 50"",
                ""NombreProductoCompleto"": ""ACCU -CHEK HOSP. GUIDE GLUC TIRAS ENV X 50"",
                ""IdDroga"": -1,
                ""IdAccionTerapeutica"": 533,
                ""IdLaboratorio"": 2479,
                ""IdTipoProducto"": 1,
                ""IdMarca"": -1,
                ""Numero"": ""A427939"",
                ""CodigoPAP"": 56793108,
                ""PorcentajeMargenDrogueria"": 9.00,
                ""EsProductoNuevo"": true,
                ""Certificado"": ""18 -287"",
                ""UnidadesMinimasPicking"": 1,
                ""UnidadesMaximasPicking"": 999999,
                ""SeTrabajaEnInstitucionales"": true,
                ""ExclusivoDeInstitucionales"": true,
                ""SeCompraEnInstitucionales"": true,
                ""IdEstado"": 29,
                ""FechaAlta"": ""2017-12-20T12:48:00"",
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-10-25T21:36:54.523"",
                ""Troquel"": ""9911000""
            }", options);

            var laboratorio = JsonSerializer.Deserialize<Laboratorios>(@"
            {
                ""IdLaboratorio"": 2479,
                ""Cuit"": 30714921335,
                ""RazonSocial"": ""ROCHE DIABETES CARE ARGENTINA S.A."",
                ""Direccion"": ""AV. LEANDRO N ALEM 986 PISO 10"",
                ""IdLocalidad"": 93,
                ""IdProvincia"": 2,
                ""CPA"": ""1001"",
                ""PideNumeroDeTransfer"": false,
                ""AceptaMalEstado"": true,
                ""IdEstado"": 2,
                ""IdFormatoArchivo"": -1,
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-09-03T15:17:58.697""
            }", options);

            var laboratorioProveedor = JsonSerializer.Deserialize<LaboratoriosProveedores>(@"
            {
                ""IdLaboratorioProveedor"": 6422,
                ""IdLaboratorio"": 2479,
                ""IdProveedor"": 4696,
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-09-07T10:34:18.027""
            }", options);

            var proveedor = JsonSerializer.Deserialize<Proveedores>(@"
            {
                ""IdProveedor"": 4696,
                ""Cuit"": 30538474858,
                ""RazonSocial"": ""ROFINA S.A.I.C.F."",
                ""Direccion"": ""HIPOLITO YRIGOYEN 476"",
                ""IdLocalidad"": 93,
                ""IdProvincia"": 2,
                ""CPA"": ""C1086AAF"",
                ""Telefono"": ""(011) 4334-9812\/15\/17"",
                ""EMail"": ""fizumi@rofina.com.ar"",
                ""SitioWeb"": ""Certificados rperez@rofina.com.ar ; joneglia@rofina.com.ar"",
                ""NumeroCuentaCorriente"": 140,
                ""DestinatarioDelCheque"": ""ROFINA S.A.I.C.F."",
                ""NCRObSocPorLaboratorio"": true,
                ""PideNumeroDeTransfer"": true,
                ""GeneraOrdenesDeCompraEspeciales"": false,
                ""GLN"": ""7798157560005"",
                ""IdTipoProv"": 1,
                ""CondicionEnDias"": 30,
                ""GeneraComprobanteObraSocial"": false,
                ""ProveedorSiempreAPicking"": false,
                ""EmiteFacturaDeCredito"": false,
                ""AplicaNCObrasSociales"": false,
                ""IdEstado"": 2,
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-09-01T15:45:46.277""
            }", options);

            var productoCompra = JsonSerializer.Deserialize<ProductosCompras>(@"
            {
                ""IdProductoCompra"": 56115,
                ""IdProducto"": 362843,
                ""DisponibleParaLaCompra"": true,
                ""IdProveedor"": 4696,
                ""IdRubroCompra"": 3639,
                ""CodigoInternoEnProveedor"": ""437453736036"",
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-10-28T00:30:56.143"",
                ""EnviaEmpaqueCerradoEnGR"": false,
                ""NetoParaElProveedor"": false,
                ""AceptaVencidos"": false
            }", options);

            var productoPrecio = JsonSerializer.Deserialize<ProductosPrecios>(@"
            {
                ""IdProductoPrecio"": 761884,
                ""IdProducto"": 362843,
                ""Precio"": 570.00,
                ""Costo"": 324.90,
                ""FechaPrecio"": ""2022-02-05"",
                ""FechaCosto"": ""2022-02-05"",
                ""PVPNeto"": 0.00,
                ""IdProductoOrigenPrecio"": 4,
                ""NumeroListaPrecios"": ""3"",
                ""FechaListaPrecios"": ""2018-10-24"",
                ""LoginCambioPrecio"": ""blazor"",
                ""MarkUpProveedor"": 4.00,
                ""CostoVH"": 324.90,
                ""PrecioVH"": 324.90,
                ""FechaCostoVH"": ""2022-02-05"",
                ""FechaPrecioVH"": ""2022-02-05"",
                ""MarkUpProveedorVH"": null,
                ""LoginCambioPrecioVH"": ""blazor"",
                ""PorcentajeARestarAlDtoDirectoDelCliente"": 0.00,
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-10-27T23:05:58.700"",
                ""UsuarioUltimaModificacion"": ""67e7cb16-0d54-4cb2-9e1b-0f3998b8d57f"",
                ""FechaUltimaModificacion"": ""2022-02-17T10:52:12.170""
            }", options);

            //Act  

            context.Productos.Add(producto);
            context.Laboratorios.Add(laboratorio);
            context.LaboratoriosProveedores.Add(laboratorioProveedor);
            context.Proveedores.Add(proveedor);
            context.ProductosCompras.Add(productoCompra);
            context.ProductosPrecios.Add(productoPrecio);
            context.SaveChanges();
            context.ChangeTracker.Clear();
            #endregion

            List<int> productIds = new() { 362843 };
            MarkUpDTO markUpDTO = new(55.55m, 10.11m);
            List<DiscountDTO> discountDTOs = new() { new(1, 74.12m, 36.35m), new(2, 34.19m, 78.48m) };
            ProcessRequestDTO request = new(productIds, markUpDTO, discountDTOs, "blazor");

            // Act
            await sut.ProcessRequestAsync(request, unitOfWork);

            unitOfWork = new TransactionScopeUnitOfWork(context, IsolationLevel.ReadUncommitted, TransactionScopeOption.Suppress);
            var products = await sut.GetDataGridByPurchaseCategoryIdAsync(3639, unitOfWork);

            //Assert 
            Assert.Single(products);
            Assert.Equal(55.55m, products.First().ProductosPrecios__MarkUpProveedor);
            Assert.Equal(10.11m, products.First().ProductosPrecios__MarkUpProveedorVh);
            Assert.Equal("74.12 / 34.19", products.First().ProductosDescuentosProveedor__PorcentajeDescuentoProveedor);
            Assert.Equal("36.35 / 78.48", products.First().ProductosDescuentosProveedor__PorcentajeDescuentoProveedorVh);
        }

        [Fact]
        public async Task GetDataGridByPurchaseCategoryIdAsync_ShouldReturn_Discounts_Correctly()
        {
            // Arrange  
            var context = _connectionFactory.CreateContextForSQLite();

            IUnitOfWork unitOfWork = new TransactionScopeUnitOfWork(context, IsolationLevel.ReadUncommitted, TransactionScopeOption.Suppress);

            MarkupPercentagesUpdateAndDiscountsBL sut = new(
                _productsRepository,
                _productsService,
                _productsPricesService,
                _productsSupplierDiscountsService);

            #region Setting Data

            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };

            var producto = JsonSerializer.Deserialize<Productos>(@"
            {
                ""IdProducto"": 362843,
                ""NombreProducto"": ""ACCU -CHEK HOSP. GUIDE GLUC TIRAS ENV X 50"",
                ""NombreProductoCompleto"": ""ACCU -CHEK HOSP. GUIDE GLUC TIRAS ENV X 50"",
                ""IdDroga"": -1,
                ""IdAccionTerapeutica"": 533,
                ""IdLaboratorio"": 2479,
                ""IdTipoProducto"": 1,
                ""IdMarca"": -1,
                ""Numero"": ""A427939"",
                ""CodigoPAP"": 56793108,
                ""PorcentajeMargenDrogueria"": 9.00,
                ""EsProductoNuevo"": true,
                ""Certificado"": ""18 -287"",
                ""UnidadesMinimasPicking"": 1,
                ""UnidadesMaximasPicking"": 999999,
                ""SeTrabajaEnInstitucionales"": true,
                ""ExclusivoDeInstitucionales"": true,
                ""SeCompraEnInstitucionales"": true,
                ""IdEstado"": 29,
                ""FechaAlta"": ""2017-12-20T12:48:00"",
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-10-25T21:36:54.523"",
                ""Troquel"": ""9911000""
            }", options);

            var laboratorio = JsonSerializer.Deserialize<Laboratorios>(@"
            {
                ""IdLaboratorio"": 2479,
                ""Cuit"": 30714921335,
                ""RazonSocial"": ""ROCHE DIABETES CARE ARGENTINA S.A."",
                ""Direccion"": ""AV. LEANDRO N ALEM 986 PISO 10"",
                ""IdLocalidad"": 93,
                ""IdProvincia"": 2,
                ""CPA"": ""1001"",
                ""PideNumeroDeTransfer"": false,
                ""AceptaMalEstado"": true,
                ""IdEstado"": 2,
                ""IdFormatoArchivo"": -1,
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-09-03T15:17:58.697""
            }", options);

            var laboratorioProveedor = JsonSerializer.Deserialize<LaboratoriosProveedores>(@"
            {
                ""IdLaboratorioProveedor"": 6422,
                ""IdLaboratorio"": 2479,
                ""IdProveedor"": 4696,
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-09-07T10:34:18.027""
            }", options);

            var proveedor = JsonSerializer.Deserialize<Proveedores>(@"
            {
                ""IdProveedor"": 4696,
                ""Cuit"": 30538474858,
                ""RazonSocial"": ""ROFINA S.A.I.C.F."",
                ""Direccion"": ""HIPOLITO YRIGOYEN 476"",
                ""IdLocalidad"": 93,
                ""IdProvincia"": 2,
                ""CPA"": ""C1086AAF"",
                ""Telefono"": ""(011) 4334-9812\/15\/17"",
                ""EMail"": ""fizumi@rofina.com.ar"",
                ""SitioWeb"": ""Certificados rperez@rofina.com.ar ; joneglia@rofina.com.ar"",
                ""NumeroCuentaCorriente"": 140,
                ""DestinatarioDelCheque"": ""ROFINA S.A.I.C.F."",
                ""NCRObSocPorLaboratorio"": true,
                ""PideNumeroDeTransfer"": true,
                ""GeneraOrdenesDeCompraEspeciales"": false,
                ""GLN"": ""7798157560005"",
                ""IdTipoProv"": 1,
                ""CondicionEnDias"": 30,
                ""GeneraComprobanteObraSocial"": false,
                ""ProveedorSiempreAPicking"": false,
                ""EmiteFacturaDeCredito"": false,
                ""AplicaNCObrasSociales"": false,
                ""IdEstado"": 2,
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-09-01T15:45:46.277""
            }", options);

            var productoCompra = JsonSerializer.Deserialize<ProductosCompras>(@"
            {
                ""IdProductoCompra"": 56115,
                ""IdProducto"": 362843,
                ""DisponibleParaLaCompra"": true,
                ""IdProveedor"": 4696,
                ""IdRubroCompra"": 3639,
                ""CodigoInternoEnProveedor"": ""437453736036"",
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-10-28T00:30:56.143"",
                ""EnviaEmpaqueCerradoEnGR"": false,
                ""NetoParaElProveedor"": false,
                ""AceptaVencidos"": false
            }", options);

            var productoPrecio = JsonSerializer.Deserialize<ProductosPrecios>(@"
            {
                ""IdProductoPrecio"": 761884,
                ""IdProducto"": 362843,
                ""Precio"": 570.00,
                ""Costo"": 324.90,
                ""FechaPrecio"": ""2022-02-05"",
                ""FechaCosto"": ""2022-02-05"",
                ""PVPNeto"": 0.00,
                ""IdProductoOrigenPrecio"": 4,
                ""NumeroListaPrecios"": ""3"",
                ""FechaListaPrecios"": ""2018-10-24"",
                ""LoginCambioPrecio"": ""blazor"",
                ""MarkUpProveedor"": 4.00,
                ""CostoVH"": 324.90,
                ""PrecioVH"": 324.90,
                ""FechaCostoVH"": ""2022-02-05"",
                ""FechaPrecioVH"": ""2022-02-05"",
                ""MarkUpProveedorVH"": 4.00,
                ""LoginCambioPrecioVH"": ""blazor"",
                ""PorcentajeARestarAlDtoDirectoDelCliente"": 0.00,
                ""UsuarioCreacion"": ""129CA9E1-FD71-4E63-996C-B0364DB3B984"",
                ""FechaCreacion"": ""2021-10-27T23:05:58.700"",
                ""UsuarioUltimaModificacion"": ""67e7cb16-0d54-4cb2-9e1b-0f3998b8d57f"",
                ""FechaUltimaModificacion"": ""2022-02-17T10:52:12.170""
            }", options);

            var productoDescuentosProveedor = JsonSerializer.Deserialize<List<ProductosDescuentosProveedor>>(@"
            [{
                ""IdDescuentoProveedor"": 54028,
                ""IdProducto"": 362843,
                ""CodigoDescuentoProducto"": 1,
                ""PorcentajeDescuentoProveedor"": 5.11,
                ""PorcentajeDescuentoProveedorVH"": 5.11,
                ""UsuarioCreacion"": ""67e7cb16-0d54-4cb2-9e1b-0f3998b8d57f"",
                ""FechaCreacion"": ""2022-02-17T10:52:12.170"",
                ""UsuarioUltimaModificacion"": ""67e7cb16-0d54-4cb2-9e1b-0f3998b8d57f"",
                ""FechaUltimaModificacion"": ""2022-02-17T10:52:12.170""
            },
            {
                ""IdDescuentoProveedor"": 54029,
                ""IdProducto"": 362843,
                ""CodigoDescuentoProducto"": 2,
                ""PorcentajeDescuentoProveedor"": 3.11,
                ""PorcentajeDescuentoProveedorVH"": 9.11,
                ""UsuarioCreacion"": ""67e7cb16-0d54-4cb2-9e1b-0f3998b8d57f"",
                ""FechaCreacion"": ""2022-02-17T10:52:12.170"",
                ""UsuarioUltimaModificacion"": ""67e7cb16-0d54-4cb2-9e1b-0f3998b8d57f"",
                ""FechaUltimaModificacion"": ""2022-02-17T10:52:12.170""
            }]", options);

            //Act  

            context.Productos.Add(producto);
            context.Laboratorios.Add(laboratorio);
            context.LaboratoriosProveedores.Add(laboratorioProveedor);
            context.Proveedores.Add(proveedor);
            context.ProductosCompras.Add(productoCompra);
            context.ProductosPrecios.Add(productoPrecio);
            context.ProductosDescuentosProveedor.AddRange(productoDescuentosProveedor);
            context.SaveChanges();
            #endregion

            // Act
            var products = await sut.GetDataGridByPurchaseCategoryIdAsync(3639, unitOfWork);

            //Assert 
            Assert.Single(products);
            Assert.Equal("5.11 / 3.11", products.First().ProductosDescuentosProveedor__PorcentajeDescuentoProveedor);
            Assert.Equal("5.11 / 9.11", products.First().ProductosDescuentosProveedor__PorcentajeDescuentoProveedorVh);
        }

        [Fact]
        public void FormatDataGridItemResult_Should_Format_DataGridDTOList_Correctly()
        {
            // Arrange
            MarkupPercentagesUpdateAndDiscountsBL sut = new(
                _productsRepository,
                _productsService,
                _productsPricesService,
                _productsSupplierDiscountsService);

            var unformattedProducts = new List<DataGridItemDTO>()
            {
                new DataGridItemDTO()
                {
                    Productos__IdProducto = 0,
                    Proveedores__RazonSocial = "Test",
                    Productos__NombreProducto = "Test",
                    Laboratorios__RazonSocial = "Test",
                    ProductosPrecios__MarkUpProveedor = 12.11m,
                    ProductosPrecios__MarkUpProveedorVh = 13.12m,
                    ProductosDescuentosProveedor__PorcentajeDescuentoProveedor = "10.33",
                    ProductosDescuentosProveedor__PorcentajeDescuentoProveedorVh = "2.67",
                },
                new DataGridItemDTO()
                {
                    Productos__IdProducto = 0,
                    Proveedores__RazonSocial = "Test",
                    Productos__NombreProducto = "Test",
                    Laboratorios__RazonSocial = "Test",
                    ProductosPrecios__MarkUpProveedor = 12.11m,
                    ProductosPrecios__MarkUpProveedorVh = 13.12m,
                    ProductosDescuentosProveedor__PorcentajeDescuentoProveedor = "17.74",
                    ProductosDescuentosProveedor__PorcentajeDescuentoProveedorVh = "23.68",
                },
            };

            var expected = new DataGridItemDTO()
            {
                Productos__IdProducto = 0,
                Proveedores__RazonSocial = "Test",
                Productos__NombreProducto = "Test",
                Laboratorios__RazonSocial = "Test",
                ProductosPrecios__MarkUpProveedor = 12.11m,
                ProductosPrecios__MarkUpProveedorVh = 13.12m,
                ProductosDescuentosProveedor__PorcentajeDescuentoProveedor = "10.33 / 17.74",
                ProductosDescuentosProveedor__PorcentajeDescuentoProveedorVh = "2.67 / 23.68",
            };

            // Act
            var formattedProducts = sut.FormatDataGridItemResult(unformattedProducts);

            // Assert
            Assert.Equal(expected, formattedProducts.First());
        }
    }
}
