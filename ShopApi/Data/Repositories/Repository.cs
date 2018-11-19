using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data.Interfaces;

namespace ShopApi.Data.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbSet<T> DbSet;

        protected Repository(DbContext context)
        {
            DbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync(int skip, int take, Expression<Func<T, bool>> filter = null)
        {
            var query = DbSet.AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            var list = await query
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return list;
        }

        public async Task<T> GetAsync(Guid id)
        {
            return await DbSet.FindAsync(id);
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> filter)
        {
            return await DbSet.SingleOrDefaultAsync(filter);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> filter)
        {
            return await DbSet.AnyAsync(filter);
        }

        public async Task AddAsync(T entity)
        {
            await DbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await DbSet.AddRangeAsync(entities);
        }

        public bool Remove(Guid id)
        {
            var entity = DbSet.Find(id);
            if (entity == null)
            {
                return false;
            }

            Remove(entity);
            return true;

        }

        public void Remove(T entity)
        {
            DbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            DbSet.RemoveRange(entities);
        }
    }
}
