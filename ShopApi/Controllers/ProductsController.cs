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
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProductsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAsync([FromQuery]int skip = 0, [FromQuery] int take = 20)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var products = await Task.Run(() =>
                _context.Products.Skip(skip).Take(take).Include(p => p.Category).ToList());

            var productDtos = _mapper.Map<IEnumerable<Product>,IEnumerable<ProductResponseDto>>(products);

            return Ok(productDtos);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductResponseDto>> GetAsync([FromRoute] string id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var product = await _context.Products.Include(p => p.Category).SingleOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            var responseDto = _mapper.Map<Product, ProductResponseDto>(product);

            return responseDto;
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutProduct([FromRoute] string id, [FromBody] ProductRequestDto requestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var productFromDb = await _context.Products.FindAsync(id);

            if (productFromDb == null) return NotFound();

            _mapper.Map(requestDto, productFromDb);

            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<Product, ProductResponseDto>(productFromDb);

            return Ok(responseDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostAsync([FromBody] ProductRequestDto requestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var product = _mapper.Map<ProductRequestDto, Product>(requestDto);
            product.Id = Guid.NewGuid().ToString();

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<Product, ProductResponseDto>(product);

            return Ok(responseDto);
        }
        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var product = await _context.Products.FindAsync(id);

            if (product == null) return NotFound();

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return Ok(id);
        }
    }
}