using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnitOfWork;

namespace Repository.Interface
{
    public interface IBaseRepository<T> where T : class
    {
        public Task<T> AddAsync(T entity, CancellationToken cancellationToken = default, bool detachEntry = false);
        public Task<T> RemoveAsync(T entity, CancellationToken cancellationToken = default);
        public Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        public IQueryable<T> Where(Expression<Func<T, bool>> predicate);
    }
}