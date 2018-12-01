using System;
using System.Threading.Tasks;
using ShopApi.Data.Models;

namespace ShopApi.Data.Interfaces
{
    public interface IProductRepository : IRepositoryBase<Product>
    {
        Task<Product> GetWithCategoryAsync(Guid id);
    }
}