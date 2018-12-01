using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using ShopApi.Data.Interfaces;
using ShopApi.Data.Models;
using ShopApi.Logic.Dtos.Requests;
using ShopApi.Logic.Dtos.Responses;
using ShopApi.Logic.Interfaces;

namespace ShopApi.Logic.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoriesService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<FetchResult<CategoryResponseDto>> GetAllAsync(FetchRequestDto request)
        {
            var isSearchEmpty = string.IsNullOrWhiteSpace(request.SearchString);
            Expression<Func<Category, bool>> filter = c => isSearchEmpty || c.Name.Contains(request.SearchString);
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync(request.Skip, request.Take, filter);

            var categoryDtos = _mapper.Map<IEnumerable<Category>, IEnumerable<CategoryResponseDto>>(categories);
            var count = await _unitOfWork.CategoryRepository.CountAsync();

            return new FetchResult<CategoryResponseDto> { Data = categoryDtos, Count = count };
        }

        public async Task<CategoryResponseDto> GetAsync(Guid id)
        {
            var category = await _unitOfWork.CategoryRepository.GetAsync(id);
            if (category == null)
            {
                return null;
            }

            var responseDto = _mapper.Map<Category, CategoryResponseDto>(category);

            return responseDto;
        }

        public async Task<Result<CategoryResponseDto>> PostAsync(CategoryRequestDto requestDto)
        {
            var response = new Result<CategoryResponseDto>();

            var isDuplicate = await _unitOfWork.CategoryRepository.ExistsAsync(p => p.Name == requestDto.Name);
            if (isDuplicate)
            {
                response.Errors.Add($"Category name '{requestDto.Name}' is already taken.");
                return response;
            }

            var category = _mapper.Map<CategoryRequestDto, Category>(requestDto);

            await _unitOfWork.CategoryRepository.AddAsync(category);
            await _unitOfWork.SaveAsync();

            var responseDto = _mapper.Map<Category, CategoryResponseDto>(category);
            response.Data = responseDto;

            return response;
        }

        public async Task<Result<CategoryResponseDto>> PutAsync(Guid id, CategoryRequestDto requestDto)
        {
            var categoryFromDb = await _unitOfWork.CategoryRepository.GetAsync(id);
            if (categoryFromDb == null)
            {
                return null;
            }

            var response = new Result<CategoryResponseDto>();
            if (categoryFromDb.Name == requestDto.Name)
            {
                response.Errors.Add($"Category name '{requestDto.Name}' is already taken.");
                return response;
            }

            _mapper.Map(requestDto, categoryFromDb);
            await _unitOfWork.SaveAsync();

            var responseDto = _mapper.Map<Category, CategoryResponseDto>(categoryFromDb);

            response.Data = responseDto;
            return response;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var removed = _unitOfWork.CategoryRepository.Remove(id);
            if (!removed)
            {
                return false;
            }

            await _unitOfWork.SaveAsync();

            return true;
        }
    }
}
