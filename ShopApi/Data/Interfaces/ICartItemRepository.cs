using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShopApi.Data.Models;

namespace ShopApi.Data.Interfaces
{
    public interface ICartItemRepository : IRepository<CartItem>
    {
        Task<IEnumerable<CartItem>> GetUserItemsAsync(Guid userId);
    }
}