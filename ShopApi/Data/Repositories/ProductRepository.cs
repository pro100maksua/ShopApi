using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data.Interfaces;
using ShopApi.Data.Models;

namespace ShopApi.Data.Repositories
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(DbContext context) : base(context)
        {
        }

        public async Task<Product> GetWithCategoryAsync(Guid id)
        {
            return await DbSet.Where(p => p.Id == id).Include(p => p.Category).SingleOrDefaultAsync();
        }
    }
}