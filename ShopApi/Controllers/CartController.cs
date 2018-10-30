using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Logic.Interfaces;

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
            var userId = Guid.Parse(User.Identity.Name);

            var cartDto = await _cartService.GetCartAsync(userId);

            return Ok(cartDto);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCartAsync()
        {
            var userId = Guid.Parse(User.Identity.Name);


            await _cartService.DeleteCartAsync(userId);

            return Ok();
        }

        [HttpPut("add")]
        public async Task<IActionResult> AddToCartAsync(Guid productId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = Guid.Parse(User.Identity.Name);

            await _cartService.AddToCartAsync(productId, userId);

            return Ok();
        }

        [HttpPut("remove")]
        public async Task<IActionResult> RemoveFromCartAsync(Guid productId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = Guid.Parse(User.Identity.Name);

            await _cartService.RemoveFromCartAsync(productId, userId);

            return Ok();
        }
    }
}