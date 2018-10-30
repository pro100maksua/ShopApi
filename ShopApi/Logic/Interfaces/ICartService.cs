using System;
using System.Threading.Tasks;
using ShopApi.Logic.Dtos.Responses;

namespace ShopApi.Logic.Interfaces
{
    public interface ICartService
    {
        Task<CartResponseDto> GetCartAsync(Guid userId);
        Task DeleteCartAsync(Guid userId);
        Task AddToCartAsync(Guid productId, Guid userId);
        Task RemoveFromCartAsync(Guid productId, Guid userId);
    }
}