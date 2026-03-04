using DataAccess;

namespace Test
{
    public interface IConnectionFactory
    {
        DBContext CreateContextForInMemory();
        DBContext CreateContextForSQLite();
        void Dispose();
    }
}