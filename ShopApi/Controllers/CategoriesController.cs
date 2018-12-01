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
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoriesService _categoriesService;

        public CategoriesController(ICategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<FetchResult<CategoryResponseDto>>> GetAllAsync([FromQuery] FetchRequestDto request)
        {
            var categoryDtos = await _categoriesService.GetAllAsync(request);

            return categoryDtos;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CategoryResponseDto>> GetAsync([FromRoute] Guid id)
        {
            var responseDto = await _categoriesService.GetAsync(id);
            if (responseDto == null)
            {
                return NotFound();
            }

            return responseDto;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryResponseDto>> PostAsync([FromBody] CategoryRequestDto requestDto)
        {
            var response = await _categoriesService.PostAsync(requestDto);
            if (response.Data == null)
            {
                return BadRequest(response.Errors);
            }

            return response.Data;
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryResponseDto>> PutAsync([FromRoute] Guid id, [FromBody] CategoryRequestDto requestDto)
        {
            var response = await _categoriesService.PutAsync(id, requestDto);
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
            var deleted = await _categoriesService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}