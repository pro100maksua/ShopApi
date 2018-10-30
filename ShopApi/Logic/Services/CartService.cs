using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ShopApi.Data.Interfaces;
using ShopApi.Data.Models;
using ShopApi.Logic.Dtos.Responses;
using ShopApi.Logic.Interfaces;

namespace ShopApi.Logic.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CartResponseDto> GetCartAsync(Guid userId)
        {
            var items = await _unitOfWork.CartItemRepository.GetUserItemsAsync(userId);

            var total = items.Select(i => i.Product.Cost * i.Count).Sum();

            var itemDtos = _mapper.Map<IEnumerable<CartItem>, IEnumerable<CartItemResponseDto>>(items);

            return new CartResponseDto { Items = itemDtos, Total = Math.Round(total, 2) };
        }

        public async Task DeleteCartAsync(Guid userId)
        {
            var items = await _unitOfWork.CartItemRepository.GetUserItemsAsync(userId);

            _unitOfWork.CartItemRepository.RemoveRange(items);

            await _unitOfWork.SaveAsync();
        }

        public async Task AddToCartAsync(Guid productId, Guid userId)
        {
            var item = await _unitOfWork.CartItemRepository.FindAsync(i =>
                i.ProductId == productId && i.UserId == userId);

            if (item == null)
            {
                item = new CartItem
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ProductId = productId
                };

                await _unitOfWork.CartItemRepository.AddAsync(item);
            }

            item.Count++;

            await _unitOfWork.SaveAsync();
        }

        public async Task RemoveFromCartAsync(Guid productId, Guid userId)
        {
            var item = await _unitOfWork.CartItemRepository.FindAsync(i =>
                i.ProductId == productId && i.UserId == userId);

            if (item == null)
            {
                return;
            }

            item.Count--;

            if (item.Count == 0)
            {
                _unitOfWork.CartItemRepository.Remove(item);
            }

            await _unitOfWork.SaveAsync();
        }
    }
}