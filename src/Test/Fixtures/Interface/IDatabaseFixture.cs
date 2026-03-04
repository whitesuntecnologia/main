using DataAccess;
using System;

namespace Test.Fixtures
{
    public interface IDatabaseFixture<T> : IFixture<T> where T : class
    {
        void SaveToDatabase(DBContext context);
        TReturn SaveToDatabase<TReturn>(DBContext context, Func<T, TReturn> idSelector);
        void SaveToDatabase();
        TReturn SaveToDatabase<TReturn>(Func<T, TReturn> idSelector);
        void SetContext(DBContext context);
    }
}