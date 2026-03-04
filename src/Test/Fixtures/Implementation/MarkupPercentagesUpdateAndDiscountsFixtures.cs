using DataAccess;
using DataAccess.Entities;
using System;

namespace Test.Fixtures
{
    public class MarkupPercentagesUpdateAndDiscountsFixtures : DatabaseFixtures, IMarkupPercentagesUpdateAndDiscountsFixtures
    {
        public MarkupPercentagesUpdateAndDiscountsFixtures() : base()
        {
        }

        public MarkupPercentagesUpdateAndDiscountsFixtures(DBContext context) : base(context)
        {
        }

        public override DatabaseFixture<Productos> GetProductosEntity()
        {
            return new DatabaseFixture<Productos>(new Productos
            {
                IdProducto = 0,
                NombreProducto = "ACCU -CHEK HOSP. GUIDE GLUC TIRAS ENV X 50",
                NombreProductoCompleto = "ACCU -CHEK HOSP. GUIDE GLUC TIRAS ENV X 50",
                IdDroga = -1,
                IdAccionTerapeutica = 533,
                IdLaboratorio = 0,
                IdTipoProducto = 1,
                IdMarca = -1,
                Numero = "A427939",
                CodigoPap = 56793108,
                PorcentajeMargenDrogueria = 9,
                EsProductoNuevo = true,
                Certificado = "18 -287",
                UnidadesMinimasPicking = 1,
                UnidadesMaximasPicking = 999999,
                SeTrabajaEnInstitucionales = true,
                ExclusivoDeInstitucionales = true,
                SeCompraEnInstitucionales = true,
                IdEstado = 29,
                FechaAlta = DateTime.Parse("2017-12-20T12:48:00"),
                UsuarioCreacion = "129CA9E1-FD71-4E63-996C-B0364DB3B984",
                FechaCreacion = DateTime.Parse("2021-10-25T21:36:54.523"),
                Troquel = "9911000",
            }, _context);
        }

        public override DatabaseFixture<Laboratorios> GetLaboratoriosEntity()
        {
            return new DatabaseFixture<Laboratorios>(new Laboratorios
            {
                IdLaboratorio = 0,
                Cuit = 30714921335,
                RazonSocial = "ROCHE DIABETES CARE ARGENTINA S.A.",
                Direccion = "AV. LEANDRO N ALEM 986 PISO 10",
                IdLocalidad = 0,
                IdProvincia = 0,
                Cpa = "1001",
                PideNumeroDeTransfer = false,
                AceptaMalEstado = true,
                IdEstado = 2,
                IdFormatoArchivo = -1,
                UsuarioCreacion = "129CA9E1-FD71-4E63-996C-B0364DB3B984",
                FechaCreacion = DateTime.Parse("2021-09-03T15:17:58.697"),
            }, _context);
        }

        public override DatabaseFixture<LaboratoriosProveedores> GetLaboratoriosProveedoresEntity()
        {
            return new DatabaseFixture<LaboratoriosProveedores>(new LaboratoriosProveedores
            {
                IdLaboratorioProveedor = 0,
                IdLaboratorio = 0,
                IdProveedor = 0,
                UsuarioCreacion = "129CA9E1-FD71-4E63-996C-B0364DB3B984",
                FechaCreacion = DateTime.Parse("2021-09-07T10:34:18.027"),
            }, _context);
        }

        public override DatabaseFixture<Proveedores> GetProveedoresEntity()
        {
            return new DatabaseFixture<Proveedores>(new Proveedores
            {
                IdProveedor = 0,
                Cuit = 30538474858,
                RazonSocial = "ROFINA S.A.I.C.F.",
                Direccion = "HIPOLITO YRIGOYEN 476",
                IdLocalidad = 0,
                IdProvincia = 0,
                Cpa = "C1086AAF",
                Telefono = "(011) 4334-9812/15/17",
                Email = "fizumi@rofina.com.ar",
                SitioWeb = "Certificados rperez@rofina.com.ar ; joneglia@rofina.com.ar",
                NumeroCuentaCorriente = 140,
                DestinatarioDelCheque = "ROFINA S.A.I.C.F.",
                NcrobSocPorLaboratorio = true,
                PideNumeroDeTransfer = true,
                GeneraOrdenesDeCompraEspeciales = false,
                Gln = "7798157560005",
                IdTipoProv = 1,
                CondicionEnDias = 30,
                GeneraComprobanteObraSocial = false,
                ProveedorSiempreApicking = false,
                EmiteFacturaDeCredito = false,
                AplicaNcobrasSociales = false,
                IdEstado = 2,
                UsuarioCreacion = "129CA9E1-FD71-4E63-996C-B0364DB3B984",
                FechaCreacion = DateTime.Parse("2021-09-01T15:45:46.277"),
            }, _context);
        }

        public override DatabaseFixture<ProductosCompras> GetProductosComprasEntity()
        {
            return new DatabaseFixture<ProductosCompras>(new ProductosCompras
            {
                IdProductoCompra = 0,
                IdProducto = 0,
                DisponibleParaLaCompra = true,
                IdProveedor = 0,
                IdRubroCompra = 0,
                CodigoInternoEnProveedor = "437453736036",
                UsuarioCreacion = "129CA9E1-FD71-4E63-996C-B0364DB3B984",
                FechaCreacion = DateTime.Parse("2021-10-28T00:30:56.143"),
                EnviaEmpaqueCerradoEnGr = false,
                NetoParaElProveedor = false,
                AceptaVencidos = false,
            }, _context);
        }

        public override DatabaseFixture<ProductosPrecios> GetProductosPreciosEntity()
        {
            return new DatabaseFixture<ProductosPrecios>(new ProductosPrecios
            {
                IdProductoPrecio = 0,
                IdProducto = 0,
                Precio = 570,
                Costo = 324.9m,
                FechaPrecio = DateTime.Parse("2022-02-05"),
                FechaCosto = DateTime.Parse("2022-02-05"),
                Pvpneto = 0,
                IdProductoOrigenPrecio = 4,
                NumeroListaPrecios = "3",
                FechaListaPrecios = DateTime.Parse("2018-10-24"),
                LoginCambioPrecio = "blazor",
                MarkUpProveedor = 4.11m,
                CostoVh = 324.9m,
                PrecioVh = 324.9m,
                FechaCostoVh = DateTime.Parse("2022-02-05"),
                FechaPrecioVh = DateTime.Parse("2022-02-05"),
                MarkUpProveedorVh = 5.33m,
                LoginCambioPrecioVh = "blazor",
                PorcentajeArestarAlDtoDirectoDelCliente = 0,
                UsuarioCreacion = "129CA9E1-FD71-4E63-996C-B0364DB3B984",
                FechaCreacion = DateTime.Parse("2021-10-27T23:05:58.700"),
                UsuarioUltimaModificacion = "67e7cb16-0d54-4cb2-9e1b-0f3998b8d57f",
                FechaUltimaModificacion = DateTime.Parse("2022-02-17T10:52:12.170"),
            }, _context);
        }

        public override DatabaseFixture<ProductosDescuentosProveedor> GetProductosDescuentosProveedorEntity()
        {
            return new DatabaseFixture<ProductosDescuentosProveedor>(new ProductosDescuentosProveedor
            {
                IdDescuentoProveedor = 0,
                IdProducto = 0,
                CodigoDescuentoProducto = 2,
                PorcentajeDescuentoProveedor = 3.11m,
                PorcentajeDescuentoProveedorVh = 9.11m,
                UsuarioCreacion = "67e7cb16-0d54-4cb2-9e1b-0f3998b8d57f",
                FechaCreacion = DateTime.Parse("2022-02-17T10:52:12.170"),
                UsuarioUltimaModificacion = "67e7cb16-0d54-4cb2-9e1b-0f3998b8d57f",
                FechaUltimaModificacion = DateTime.Parse("2022-02-17T10:52:12.170"),
            }, _context);
        }
    }
}
