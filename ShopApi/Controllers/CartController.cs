using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Logic.Dtos.Responses;
using ShopApi.Logic.Interfaces;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Customer, Admin")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public Guid UserId => User == null ? Guid.Empty : Guid.Parse(User.Identity.Name);

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<ActionResult<CartResponseDto>> GetCartAsync()
        {
            var cartDto = await _cartService.GetCartAsync(UserId);

            return cartDto;
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCartAsync()
        {
            await _cartService.DeleteCartAsync(UserId);

            return Ok();
        }

        [HttpPut("add/{productId}")]
        public async Task<IActionResult> AddToCartAsync([FromRoute] Guid productId)
        {
            var added = await _cartService.AddToCartAsync(productId, UserId);
            if (!added)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPut("remove/{productId}")]
        public async Task<IActionResult> RemoveFromCartAsync([FromRoute] Guid productId)
        {
            var removed = await _cartService.RemoveFromCartAsync(productId, UserId);
            if (!removed)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}