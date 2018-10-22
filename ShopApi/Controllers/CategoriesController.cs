using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Dtos.Requests;
using ShopApi.Dtos.Responses;
using ShopApi.Services;

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
        public async Task<IActionResult> GetAllAsync([FromQuery] FetchRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var responseDtos = await _categoriesService.GetAllAsync(request);

            return Ok(responseDtos);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CategoryResponseDto>> GetAsync([FromRoute] Guid id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var responseDto = await _categoriesService.GetAsync(id);

            if (responseDto == null) return NotFound();

            return responseDto;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostAsync([FromBody] CategoryRequestDto requestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var responseDto = await _categoriesService.PostAsync(requestDto);

            return Ok(responseDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutAsync([FromRoute] Guid id, [FromBody] CategoryRequestDto requestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var responseDto = await _categoriesService.PutAsync(id, requestDto);

            if (responseDto == null) return NotFound();

            return Ok(responseDto);
        }
        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var deleted = await _categoriesService.DeleteAsync(id);

            if (!deleted) return NotFound();

            return Ok();
        }
    }
}