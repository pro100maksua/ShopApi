using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShopApi.Logic.Dtos.Requests;
using ShopApi.Logic.Dtos.Responses;

namespace ShopApi.Logic.Interfaces
{
    public interface ICategoriesService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllAsync(FetchRequest request);
        Task<CategoryResponseDto> GetAsync(Guid id);
        Task<Result<CategoryResponseDto>> PostAsync(CategoryRequestDto requestDto);
        Task<Result<CategoryResponseDto>> PutAsync(Guid id, CategoryRequestDto requestDto);
        Task<bool> DeleteAsync(Guid id);
    }
}