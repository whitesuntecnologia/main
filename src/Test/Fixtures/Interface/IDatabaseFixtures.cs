using DataAccess;
using DataAccess.Entities;

namespace Test.Fixtures
{
    public interface IDatabaseFixtures
    {
        public void SetContext(DBContext context);
        DatabaseFixture<Laboratorios> GetLaboratoriosEntity();
        DatabaseFixture<LaboratoriosProveedores> GetLaboratoriosProveedoresEntity();
        DatabaseFixture<ProductosCompras> GetProductosComprasEntity();
        DatabaseFixture<ProductosDescuentosProveedor> GetProductosDescuentosProveedorEntity();
        DatabaseFixture<Productos> GetProductosEntity();
        DatabaseFixture<ProductosPrecios> GetProductosPreciosEntity();
        DatabaseFixture<Proveedores> GetProveedoresEntity();
    }
}