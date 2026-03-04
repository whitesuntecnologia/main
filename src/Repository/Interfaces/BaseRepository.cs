using DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using UnitOfWork;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;
using Microsoft.Data.SqlClient;
using Ardalis.GuardClauses;
using System.Security.Principal;

namespace Repository.Interface
{
    public abstract class BaseRepository
    {
        public AplicationDBContext Context { get; }

        public BaseRepository(IUnitOfWork uow)
        {
            Guard.Against.Null(uow, nameof(uow));
            Context = uow.Context;
        }

        public BaseRepository(AplicationDBContext context)
        {
            Guard.Against.Null(context, nameof(context));
            Context = context;
        }

        //NO AGREGAR METODOS, EF YA IMPLEMENTA UN REPOSITORIO BASE!
    }

    public abstract class BaseRepository<T> : BaseRepository, IBaseRepository<T>
        where T : class
    {
        public DbSet<T> DbSet { get; }

        public BaseRepository(IUnitOfWork uow) : base(uow)
        {
            DbSet = uow.Context.Set<T>();
        }

        public BaseRepository(AplicationDBContext context) : base(context)
        {
            DbSet = context.Set<T>();
        }

        public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            var entry = DbSet.Update(entity);
            await Context.SaveChangesAsync(cancellationToken);
            return entry.Entity;
        }
        public async Task UpdateARangeAsync(ICollection<T> entity, CancellationToken cancellationToken = default, bool detachEntry = false)
        {
            foreach (T item in entity)
            {
                var entry = DbSet.Update(item);
                if (detachEntry)
                {
                    entry.State = EntityState.Detached;
                }
            }
            await Context.SaveChangesAsync(cancellationToken);

        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default, bool detachEntry = false)
        {
            var entry = await DbSet.AddAsync(entity, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);

            if (detachEntry)
            {
                entry.State = EntityState.Detached;
            }

            return entry.Entity;
        }
        public async Task AddARangeAsync(ICollection<T> entity, CancellationToken cancellationToken = default, bool detachEntry = false)
        {
            foreach (T item in entity)
            {
                var entry =  await DbSet.AddAsync(item, cancellationToken);
                if (detachEntry)
                {
                    entry.State = EntityState.Detached;
                }
            }
            await Context.SaveChangesAsync(cancellationToken);
            
        }
        public async Task<T> RemoveAsync(T entity, CancellationToken cancellationToken = default)
        {
            var entry = DbSet.Remove(entity);
            entry.State = EntityState.Deleted;
            await Context.SaveChangesAsync(cancellationToken);
            return entry.Entity;
        }
        public async Task RemoveRangeAsync(ICollection<T> entity, CancellationToken cancellationToken = default)
        {
            foreach(T item in entity)
            {
                var entry = DbSet.Remove(item);
                entry.State = EntityState.Deleted;
            }
            await Context.SaveChangesAsync(cancellationToken);
        }
        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            var dbResult = await DbSet.FirstOrDefaultAsync(predicate);
            return dbResult;
        }
        public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            var dbResult = DbSet.Where(predicate);
            return dbResult;
        }
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.AnyAsync(predicate);
        }

    }
}
