using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShopApi.Dtos.Requests;
using ShopApi.Dtos.Responses;
using ShopApi.Models;

namespace ShopApi.Services
{
    public interface ICategoriesService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllAsync(FetchRequest request);
        Task<CategoryResponseDto> GetAsync(Guid id);
        Task<CategoryResponseDto> PostAsync(CategoryRequestDto requestDto);
        Task<CategoryResponseDto> PutAsync(Guid id, CategoryRequestDto requestDto);
        Task<bool> DeleteAsync(Guid id);
    }

    public class CategoriesService : ICategoriesService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CategoriesService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync(FetchRequest request)
        {
            var isSearchEmpty = string.IsNullOrWhiteSpace(request.SearchString);

            var categories = await _context.Categories
                .AsNoTracking()
                .Where(c => isSearchEmpty || c.Name.Contains(request.SearchString))
                .Skip(request.Skip)
                .Take(request.Take)
                .ToListAsync();

            var categoryDtos = _mapper.Map<IEnumerable<Category>, IEnumerable<CategoryResponseDto>>(categories);

            return categoryDtos;
        }

        public async Task<CategoryResponseDto> GetAsync(Guid id)
        {
            var category = await _context.Categories.SingleOrDefaultAsync(c => c.Id == id);

            if (category == null) return null;

            var responseDto = _mapper.Map<Category, CategoryResponseDto>(category);

            return responseDto;
        }

        public async Task<CategoryResponseDto> PostAsync(CategoryRequestDto requestDto)
        {
            var category = _mapper.Map<CategoryRequestDto, Category>(requestDto);
            category.Id = Guid.NewGuid();

            _context.Categories.Add(category);

            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<Category, CategoryResponseDto>(category);

            return responseDto;
        }

        public async Task<CategoryResponseDto> PutAsync(Guid id, CategoryRequestDto requestDto)
        {
            var categoryFromDb = await _context.Categories.SingleOrDefaultAsync(c => c.Id == id);

            if (categoryFromDb == null) return null;

            _mapper.Map(requestDto, categoryFromDb);

            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<Category, CategoryResponseDto>(categoryFromDb);

            return responseDto;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var category = await _context.Categories.SingleOrDefaultAsync(c => c.Id == id);

            if (category == null) return false;

            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
