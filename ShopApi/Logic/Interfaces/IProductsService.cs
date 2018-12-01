using System;
using System.Threading.Tasks;
using ShopApi.Logic.Dtos.Requests;
using ShopApi.Logic.Dtos.Responses;

namespace ShopApi.Logic.Interfaces
{
    public interface IProductsService
    {
        Task<FetchResult<ProductResponseDto>> GetAllAsync(FetchRequestDto request);
        Task<ProductWithIncludeResponseDto> GetAsync(Guid id);
        Task<Result<ProductWithIncludeResponseDto>> PostAsync(ProductRequestDto requestDto);
        Task<Result<ProductWithIncludeResponseDto>> PutAsync(Guid id, ProductRequestDto requestDto);
        Task<bool> DeleteAsync(Guid id);
    }
}