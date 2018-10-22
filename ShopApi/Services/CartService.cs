using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShopApi.Dtos.Responses;
using ShopApi.Models;

namespace ShopApi.Services
{
    public interface ICartService
    {
        Task<CartResponseDto> GetCartAsync(Guid userId);
        Task<bool> DeleteCartAsync(Guid userId);
        Task AddToCartAsync(Guid productId, Guid userId);
        Task<bool> RemoveFromCartAsync(Guid productId, Guid userId);
    }

    public class CartService : ICartService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CartService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CartResponseDto> GetCartAsync(Guid userId)
        {
            var items = await _context.CartItems
                .AsNoTracking()
                .Where(i => i.UserId == userId)
                .Include(i => i.Product)
                .ThenInclude(p => p.Category)
                .ToListAsync();

            var total = items.Select(i => i.Product.Cost * i.Count).Sum();

            var itemDtos = _mapper.Map<IEnumerable<CartItem>, IEnumerable<CartItemResponseDto>>(items);

            return new CartResponseDto { Items = itemDtos, Total = Math.Round(total, 2) };
        }

        public async Task<bool> DeleteCartAsync(Guid userId)
        {
            var items = await _context.CartItems
                .Where(i => i.UserId == userId)
                .ToListAsync();

            if (!items.Any()) return false;

            _context.RemoveRange(items);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task AddToCartAsync(Guid productId, Guid userId)
        {
            var item = await _context.CartItems.SingleOrDefaultAsync(
                i => i.ProductId == productId && i.UserId == userId);

            if (item == null)
            {
                item = new CartItem
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ProductId = productId
                };

                _context.CartItems.Add(item);
            }

            item.Count++;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveFromCartAsync(Guid productId, Guid userId)
        {
            var item = await _context.CartItems.SingleOrDefaultAsync(
                i => i.ProductId == productId && i.UserId == userId);

            if (item == null) return false;

            item.Count--;

            if (item.Count == 0)
            {
                _context.CartItems.Remove(item);
            }

            await _context.SaveChangesAsync();

            return true;
        }
    }
}