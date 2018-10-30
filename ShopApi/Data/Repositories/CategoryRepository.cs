using ShopApi.Data.Interfaces;
using ShopApi.Data.Models;

namespace ShopApi.Data.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }
    }
}