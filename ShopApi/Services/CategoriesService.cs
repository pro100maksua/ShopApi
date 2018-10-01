using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShopApi.Dtos;
using ShopApi.Models;

namespace ShopApi.Services
{
    public interface ICategoriesService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllAsync(FetchRequestDto requestDto);
        Task<CategoryResponseDto> GetAsync(string id);
        Task<CategoryResponseDto> PostAsync(CategoryRequestDto requestDto);
        Task<CategoryResponseDto> PutAsync(string id, CategoryRequestDto requestDto);
        Task<bool> DeleteAsync(string id);
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

        public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync(FetchRequestDto requestDto)
        {
            IQueryable<Category> query = _context.Categories;

            if (requestDto.SearchString.Length >= 3)
            {
                query = query.Where(c => c.Name.Contains(requestDto.SearchString));
            }

            var categories = await query
                .Skip(requestDto.Skip)
                .Take(requestDto.Take)
                .ToListAsync();

            var categoryDtos = _mapper.Map<IEnumerable<Category>, IEnumerable<CategoryResponseDto>>(categories);

            return categoryDtos;
        }

        public async Task<CategoryResponseDto> GetAsync(string id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null) return null;

            var responseDto = _mapper.Map<Category, CategoryResponseDto>(category);

            return responseDto;
        }

        public async Task<CategoryResponseDto> PostAsync(CategoryRequestDto requestDto)
        {
            var category = _mapper.Map<CategoryRequestDto, Category>(requestDto);
            category.Id = Guid.NewGuid().ToString();

            _context.Categories.Add(category);

            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<Category, CategoryResponseDto>(category);

            return responseDto;
        }

        public async Task<CategoryResponseDto> PutAsync(string id, CategoryRequestDto requestDto)
        {
            var categoryFromDb = await _context.Categories.FindAsync(id);

            if (categoryFromDb == null) return null;

            _mapper.Map(requestDto, categoryFromDb);

            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<Category, CategoryResponseDto>(categoryFromDb);

            return responseDto;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null) return false;

            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
