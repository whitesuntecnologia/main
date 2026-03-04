using DataAccess;
using DataAccess.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class ConnectionFactory : IDisposable, IConnectionFactory
    {

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        public DBContext CreateContextForInMemory()
        {
            var option = new DbContextOptionsBuilder<DBDKContextDefault>()
                .UseInMemoryDatabase(databaseName: "Test_Database").Options;

            var context = new DBContext(option);
            if (context != null)
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            return context;
        }

        public DBContext CreateContextForSQLite()
        {
            var connection = new SqliteConnection("DataSource=:memory:;Foreign Keys=False");
            connection.Open();

            var option = new DbContextOptionsBuilder<DBDKContextDefault>()
                .UseSqlite(connection).Options;

            var context = new DBContext(option);

            if (context != null)
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            return context;
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
