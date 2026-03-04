using System.Transactions;

namespace UnitOfWork
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork GetUnitOfWork(IsolationLevel isolationLevel, TransactionScopeOption scopeOption);
        IUnitOfWork GetUnitOfWork(TransactionScopeOption scopeOption);
        IUnitOfWork GetUnitOfWork(IsolationLevel isolationLevel);
        IUnitOfWork GetUnitOfWork();
    }
}
