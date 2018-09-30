using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Services;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Customer, Admin")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCartAsync()
        {
            var userId = User.Identity.Name;

            var cartDto = await _cartService.GetCartAsync(userId);

            return Ok(cartDto);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCartAsync()
        {
            var userId = User.Identity.Name;

            var deleted = await _cartService.DeleteCartAsync(userId);

            if (!deleted) return NotFound();

            return Ok();
        }

        [HttpPut("add")]
        public async Task<IActionResult> AddToCartAsync(string productId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.Identity.Name;

            await _cartService.AddToCartAsync(productId, userId);

            return Ok();
        }

        [HttpPut("remove")]
        public async Task<IActionResult> RemoveFromCartAsync(string productId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.Identity.Name;

            var removed = await _cartService.RemoveFromCartAsync(productId,userId);

            if (!removed) return BadRequest();

            return Ok();
        }
    }
}