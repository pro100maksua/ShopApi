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

        public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync(FetchRequest request)
        {
            var isSearchEmpty = string.IsNullOrWhiteSpace(request.SearchString);
            Expression<Func<Category, bool>> filter = c => isSearchEmpty || c.Name.Contains(request.SearchString);
            
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync(request.Skip, request.Take, filter);

            var categoryDtos = _mapper.Map<IEnumerable<Category>, IEnumerable<CategoryResponseDto>>(categories);

            return categoryDtos;
        }

        public async Task<CategoryResponseDto> GetAsync(Guid id)
        {
            var category = await _unitOfWork.CategoryRepository.GetAsync(id);

            if (category == null) return null;

            var responseDto = _mapper.Map<Category, CategoryResponseDto>(category);

            return responseDto;
        }

        public async Task<CategoryResponseDto> PostAsync(CategoryRequestDto requestDto)
        {
            var category = _mapper.Map<CategoryRequestDto, Category>(requestDto);
            category.Id = Guid.NewGuid();
            
            await _unitOfWork.CategoryRepository.AddAsync(category);
            await _unitOfWork.SaveAsync();

            var responseDto = _mapper.Map<Category, CategoryResponseDto>(category);

            return responseDto;
        }

        public async Task<CategoryResponseDto> PutAsync(Guid id, CategoryRequestDto requestDto)
        {
            var categoryFromDb = await _unitOfWork.CategoryRepository.GetAsync(id);

            if (categoryFromDb == null) return null;

            _mapper.Map(requestDto, categoryFromDb);

           // _unitOfWork.CategoryRepository.Update(categoryFromDb);
            await _unitOfWork.SaveAsync();

            var responseDto = _mapper.Map<Category, CategoryResponseDto>(categoryFromDb);

            return responseDto;
        }

        public async Task DeleteAsync(Guid id)
        {
            _unitOfWork.CategoryRepository.Remove(id);

            await _unitOfWork.SaveAsync();
        }
    }
}
