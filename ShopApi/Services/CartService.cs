﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShopApi.Dtos;
using ShopApi.Models;

namespace ShopApi.Services
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(string userId);
        Task<bool> DeleteCartAsync(string userId);
        Task AddToCartAsync(string productId, string userId);
        Task<bool> RemoveFromCartAsync(string productId, string userId);
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

        public async Task<CartDto> GetCartAsync(string userId)
        {
            var items = await _context.CartItems
                .Where(i => i.UserId == userId)
                .Include(i => i.Product)
                .ThenInclude(p => p.Category)
                .ToListAsync();

            var total = items.Select(i => i.Product.Cost * i.Count).Sum();

            var itemDtos = _mapper.Map<IEnumerable<CartItem>, IEnumerable<CartItemResponseDto>>(items);

            return new CartDto { Items = itemDtos, Total = Math.Round(total, 2) };
        }

        public async Task<bool> DeleteCartAsync(string userId)
        {
            var items = await _context.CartItems.Where(i => i.UserId == userId).ToListAsync();

            if (!items.Any()) return false;

            _context.RemoveRange(items);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task AddToCartAsync(string productId, string userId)
        {
            var item = _context.CartItems.SingleOrDefault(i => i.ProductId == productId);

            if (item == null)
            {
                item = new CartItem
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    ProductId = productId
                };

                _context.CartItems.Add(item);
            }

            item.Count++;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveFromCartAsync(string productId, string userId)
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