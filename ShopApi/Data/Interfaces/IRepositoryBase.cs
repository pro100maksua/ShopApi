using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ShopApi.Data.Interfaces
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(int skip, int take, Expression<Func<T, bool>> filter = null);
        Task<T> GetAsync(Guid id);
        Task<T> FindAsync(Expression<Func<T, bool>> filter);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> filter);
        Task<int> CountAsync();

        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        bool Remove(Guid id);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}   