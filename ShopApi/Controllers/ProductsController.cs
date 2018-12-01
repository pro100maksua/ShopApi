using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Logic.Dtos.Requests;
using ShopApi.Logic.Dtos.Responses;
using ShopApi.Logic.Interfaces;

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
        public async Task<ActionResult<FetchResult<ProductResponseDto>>> GetAllAsync([FromQuery] FetchRequestDto request)
        {
            var productDtos = await _productsService.GetAllAsync(request);

            return productDtos;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductWithIncludeResponseDto>> GetAsync([FromRoute] Guid id)
        {
            var responseDto = await _productsService.GetAsync(id);
            if (responseDto == null)
            {
                return NotFound();
            }

            return responseDto;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductWithIncludeResponseDto>> PostAsync([FromBody] ProductRequestDto requestDto)
        {
            var response = await _productsService.PostAsync(requestDto);
            if (response.Data == null)
            {
                return BadRequest(response.Errors);
            }

            return response.Data;
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductWithIncludeResponseDto>> PutAsync([FromRoute] Guid id, [FromBody] ProductRequestDto requestDto)
        {
            var response = await _productsService.PutAsync(id, requestDto);
            if (response == null)
            {
                return NotFound();
            }
            if (response.Data == null)
            {
                return BadRequest(response.Errors);
            }

            return response.Data;
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            var deleted = await _productsService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}