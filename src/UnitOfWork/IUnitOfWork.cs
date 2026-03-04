using DataAccess;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        Task CommitAsync();
        Task CommitAsync(CancellationToken cancellationToken);
        AplicationDBContext Context { get; }
        void StartTransaction();
        void RollBack();
    }
}
