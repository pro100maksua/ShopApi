using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShopApi.Logic.Dtos.Requests;
using ShopApi.Logic.Dtos.Responses;

namespace ShopApi.Logic.Interfaces
{
    public interface IProductsService
    {
        Task<IEnumerable<ProductResponseDto>> GetAllAsync(FetchRequest request);
        Task<ProductResponseDto> GetAsync(Guid id);
        Task<ProductResponseDto> PostAsync(ProductRequestDto requestDto);
        Task<ProductResponseDto> PutProductAsync(Guid id, ProductRequestDto requestDto);
        Task DeleteAsync(Guid id);
    }
}