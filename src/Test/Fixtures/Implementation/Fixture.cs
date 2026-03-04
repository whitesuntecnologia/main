using DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Fixtures
{
    public class Fixture<T> : IFixture<T> where T : class
    {
        protected readonly T _fixture;

        public Fixture(T fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Obtiene el objeto del fixture.
        /// </summary>
        /// <param name="changes"></param>
        /// <returns>El fixture con los cambios realizados.</returns>
        public T GetFixture()
        {
            return _fixture;
        }

        /// <summary>
        /// Realiza los cambios especificados en el fixture.
        /// </summary>
        /// <param name="changes"></param>
        /// <returns>El fixture con los cambios realizados.</returns>
        public virtual Fixture<T> WithChanges(Action<T> changes)
        {
            changes.Invoke(_fixture);
            return this;
        }

        public static implicit operator T(Fixture<T> fixture)
        {
            return fixture._fixture;
        }
    }
}
