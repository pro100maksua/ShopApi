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
        public async Task<IActionResult> GetAllAsync([FromQuery] FetchRequest request)
        {
            var responseDtos = await _categoriesService.GetAllAsync(request);

            return Ok(responseDtos);
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
        public async Task<IActionResult> PostAsync([FromBody] CategoryRequestDto requestDto)
        {
            var responseDto = await _categoriesService.PostAsync(requestDto);

            return Ok(responseDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutAsync([FromRoute] Guid id, [FromBody] CategoryRequestDto requestDto)
        {
            var responseDto = await _categoriesService.PutAsync(id, requestDto);

            if (responseDto == null)
            {
                return NotFound();
            }

            return Ok(responseDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            await _categoriesService.DeleteAsync(id);

            return Ok();
        }
    }
}