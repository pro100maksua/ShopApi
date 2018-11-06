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
        public async Task<IActionResult> GetAllAsync([FromQuery] FetchRequest request)
        {
            var productDtos = await _productsService.GetAllAsync(request);

            return Ok(productDtos);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductResponseDto>> GetAsync([FromRoute] Guid id)
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
        public async Task<ActionResult<ProductResponseDto>> PostAsync([FromBody] ProductRequestDto requestDto)
        {
            var responseDto = await _productsService.PostAsync(requestDto);
            if (responseDto == null)
            {
                return BadRequest("Duplicate name.");
            }

            return responseDto;
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductResponseDto>> PutAsync([FromRoute] Guid id, [FromBody] ProductRequestDto requestDto)
        {
            var responseDto = await _productsService.PutAsync(id, requestDto);
            if (responseDto == null)
            {
                return NotFound();
            }

            return responseDto;
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            await _productsService.DeleteAsync(id);

            return Ok();
        }
    }
}