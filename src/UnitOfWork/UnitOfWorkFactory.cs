using Castle.Core.Configuration;
using System;
using System.Transactions;

namespace UnitOfWork
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        
        private readonly IConfiguration configuration;

        public UnitOfWorkFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public UnitOfWorkFactory() {}

        public IUnitOfWork GetUnitOfWork(IsolationLevel isolationLevel)
        {
            return new UnitOfWork(isolationLevel);
        }

        public IUnitOfWork GetUnitOfWork(IsolationLevel isolationLevel, TransactionScopeOption scopeOption)
        {
            return new UnitOfWork(isolationLevel, scopeOption);
        }

        public IUnitOfWork GetUnitOfWork(TransactionScopeOption scopeOption)
        {
            return new UnitOfWork(scopeOption);
        }

        public IUnitOfWork GetUnitOfWork()
        {
            try
            {
                return new UnitOfWork();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}
