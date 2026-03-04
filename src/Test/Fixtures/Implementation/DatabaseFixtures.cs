using DataAccess;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Test.Fixtures
{
    public abstract class DatabaseFixtures : IDatabaseFixtures
    {
        protected DBContext _context;

        public DatabaseFixtures()
        {
        }

        public DatabaseFixtures(DBContext context)
        {
            _context = context;
        }

        public virtual DatabaseFixture<Laboratorios> GetLaboratoriosEntity()
        {
            throw new NotImplementedException();
        }

        public virtual DatabaseFixture<LaboratoriosProveedores> GetLaboratoriosProveedoresEntity()
        {
            throw new NotImplementedException();
        }

        public virtual DatabaseFixture<ProductosCompras> GetProductosComprasEntity()
        {
            throw new NotImplementedException();
        }

        public virtual DatabaseFixture<ProductosDescuentosProveedor> GetProductosDescuentosProveedorEntity()
        {
            throw new NotImplementedException();
        }

        public virtual DatabaseFixture<Productos> GetProductosEntity()
        {
            throw new NotImplementedException();
        }

        public virtual DatabaseFixture<ProductosPrecios> GetProductosPreciosEntity()
        {
            throw new NotImplementedException();
        }

        public virtual DatabaseFixture<Proveedores> GetProveedoresEntity()
        {
            throw new NotImplementedException();
        }

        public void SetContext(DBContext context)
        {
            _context = context;
        }
    }
}
