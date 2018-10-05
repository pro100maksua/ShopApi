using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Dtos;
using ShopApi.Services;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;

        public ProductsController(IProductsService productsService)
        {
            _productsService = productsService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAsync([FromQuery] FetchRequestDto requestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var productDtos = await _productsService.GetAllAsync(requestDto);

            return Ok(productDtos);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductResponseDto>> GetAsync([FromRoute] Guid id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var responseDto = await _productsService.GetAsync(id);
            
            if (responseDto == null) return NotFound();
            
            return responseDto;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostAsync([FromBody] ProductRequestDto requestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var responseDto = await _productsService.PostAsync(requestDto);
            
            return Ok(responseDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutProductAsync([FromRoute] Guid id, [FromBody] ProductRequestDto requestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var responseDto = await _productsService.PutProductAsync(id, requestDto);

            if (responseDto == null) return NotFound();
            
            return Ok(responseDto);
        }
        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var deleted = await _productsService.DeleteAsync(id);

            if (!deleted) return NotFound();
            
            return Ok();
        }
    }
}