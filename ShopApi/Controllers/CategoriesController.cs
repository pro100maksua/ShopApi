using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.Dtos;
using ShopApi.Models;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CategoriesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAsync([FromQuery]int skip = 0, [FromQuery] int take = 20)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var categories = await Task.Run(() => _context.Categories.Skip(skip).Take(take).ToList());

            var categoryDtos = _mapper.Map<IEnumerable<Category>, IEnumerable<CategoryResponseDto>>(categories);

            return Ok(categoryDtos);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CategoryResponseDto>> GetAsync([FromRoute] string id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var category = await _context.Categories.FindAsync(id);

            if (category == null) return NotFound();

            var responseDto = _mapper.Map<Category, CategoryResponseDto>(category);

            return responseDto;
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutAsync([FromRoute] string id, [FromBody] CategoryRequestDto requestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var categoryFromDb = await _context.Categories.FindAsync(id);

            if (categoryFromDb == null) return NotFound();

            _mapper.Map(requestDto, categoryFromDb);

            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<Category, CategoryResponseDto>(categoryFromDb);

            return Ok(responseDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostAsync([FromBody] CategoryRequestDto requestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var category = _mapper.Map<CategoryRequestDto, Category>(requestDto);
            category.Id = Guid.NewGuid().ToString();

            _context.Categories.Add(category);

            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<Category, CategoryResponseDto>(category);

            return Ok(responseDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var category = await _context.Categories.FindAsync(id);

            if (category == null) return NotFound();

            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            return Ok(id);
        }
    }
}