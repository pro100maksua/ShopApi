using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data.Interfaces;
using ShopApi.Data.Models;

namespace ShopApi.Data.Repositories
{
    public class CartItemRepository : RepositoryBase<CartItem>, ICartItemRepository
    {
        public CartItemRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CartItem>> GetUserItemsAsync(Guid userId)
        {
            var items = await DbSet.AsNoTracking()
                .Where(i => i.UserId == userId)
                .Include(i => i.Product)
                .ThenInclude(p => p.Category)
                .ToListAsync();

            return items;
        }
    }
}