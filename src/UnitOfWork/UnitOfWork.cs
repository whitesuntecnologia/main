using DataAccess;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool disposed = false;
        private readonly AplicationDBContext _db;

        private readonly TransactionScope transactionScope;

        public UnitOfWork(IsolationLevel isolationLevel)
        {
            this.transactionScope = new TransactionScope(
                    TransactionScopeOption.Required,
                    new TransactionOptions
                    {
                        IsolationLevel = isolationLevel,
                        Timeout = TransactionManager.MaximumTimeout
                    }, TransactionScopeAsyncFlowOption.Enabled);
            _db = new AplicationDBContext();
        }

        public UnitOfWork(AplicationDBContext context, IsolationLevel isolationLevel)
        {
            transactionScope = new TransactionScope(
                    TransactionScopeOption.Required,
                    new TransactionOptions
                    {
                        IsolationLevel = isolationLevel,
                        Timeout = TransactionManager.MaximumTimeout
                    }, TransactionScopeAsyncFlowOption.Enabled);
            _db = context;
        }

        public UnitOfWork(AplicationDBContext context, IsolationLevel isolationLevel, TransactionScopeOption scopeOption)
        {
            transactionScope = new TransactionScope(
                    scopeOption,
                    new TransactionOptions
                    {
                        IsolationLevel = isolationLevel,
                        Timeout = TransactionManager.MaximumTimeout
                    }, TransactionScopeAsyncFlowOption.Enabled);
            _db = context;
        }

        public UnitOfWork(IsolationLevel isolationLevel, TransactionScopeOption scopeOption)
        {
            transactionScope = new TransactionScope(
                    scopeOption,
                    new TransactionOptions
                    {
                        IsolationLevel = isolationLevel,
                        Timeout = TransactionManager.MaximumTimeout
                    }, TransactionScopeAsyncFlowOption.Enabled);
            _db = new AplicationDBContext();
        }

        public UnitOfWork(TransactionScopeOption scopeOption)
        {
            transactionScope = new TransactionScope(
                    scopeOption,
                    TransactionScopeAsyncFlowOption.Enabled);
            _db = new AplicationDBContext();
        }

        /// <summary>
        /// Unit OF Work Without transaction SCOPE 
        /// </summary>
        public UnitOfWork(AplicationDBContext context)
        {
            _db = context;
        }

        /// <summary>
        /// Unit OF Work Without transaction SCOPE 
        /// </summary>
        public UnitOfWork()
        {
            _db = new AplicationDBContext();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    transactionScope?.Dispose();
                }

                disposed = true;
            }
        }

        public void Commit()
        {
            _db.SaveChanges();
            this.transactionScope.Complete();
        }

        public async Task CommitAsync()
        {
            await _db.SaveChangesAsync();
            transactionScope.Complete();
        }

        public AplicationDBContext Context
        {
            get { return _db; }
        } 

        public void StartTransaction()
        {
            throw new NotImplementedException();
        }

        public void RollBack()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            await _db.SaveChangesAsync(cancellationToken);
            transactionScope.Complete();
        }
    }
}
