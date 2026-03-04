using DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Fixtures
{
    public class DatabaseFixture<T> : Fixture<T>, IDatabaseFixture<T> where T : class
    {
        private DBContext _context;

        public DatabaseFixture(T fixture) : base(fixture)
        {
        }

        public DatabaseFixture(T fixture, DBContext context) : base(fixture)
        {
            _context = context;
        }

        public override DatabaseFixture<T> WithChanges(Action<T> changes)
        {
            changes.Invoke(_fixture);
            return this;
        }

        public void SetContext(DBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Guarda el fixture en la base de datos.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void SaveToDatabase()
        {
            if (_context is null)
            {
                throw new Exception("El contexto actual es nulo");
            }

            _context.Entry(_fixture).State = EntityState.Added;
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
        }

        /// <summary>
        /// Guarda el fixture en la base de datos.
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="idSelector"></param>
        /// <returns>La selección pasada como argumento.</returns>
        /// <exception cref="Exception"></exception>
        public TReturn SaveToDatabase<TReturn>(Func<T, TReturn> selector)
        {
            if (_context is null)
            {
                throw new Exception("El contexto actual es nulo");
            }

            _context.Entry(_fixture).State = EntityState.Added;
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            return selector.Invoke(_fixture);
        }

        /// <summary>
        /// Guarda el fixture en la base de datos.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void SaveToDatabase(DBContext context)
        {
            context.Entry(_fixture).State = EntityState.Added;
            context.SaveChanges();
            context.ChangeTracker.Clear();
        }

        /// <summary>
        /// Guarda el fixture en la base de datos.
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="idSelector"></param>
        /// <returns>La selección pasada como argumento.</returns>
        /// <exception cref="Exception"></exception>
        public TReturn SaveToDatabase<TReturn>(DBContext context, Func<T, TReturn> selector)
        {
            context.Entry(_fixture).State = EntityState.Added;
            context.SaveChanges();
            context.ChangeTracker.Clear();
            return selector.Invoke(_fixture);
        }
    }
}
