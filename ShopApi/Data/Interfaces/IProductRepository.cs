using System;
using System.Threading.Tasks;
using ShopApi.Data.Models;

namespace ShopApi.Data.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product> GetWithCategoryAsync(Guid id);
    }
}