using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.Models;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Customer, Admin")]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CartController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetCartAsync()
        {
            var userId = User.Identity.Name;

            var items = await _context.CartItems
                .Where(i => i.UserId == userId)
                .Include(i => i.Product)
                .ThenInclude(p => p.Category)
                .ToListAsync();

            var total = items.Select(i => i.Product.Cost * i.Count).Sum();

            var itemDtos = _mapper.Map<IEnumerable<CartItem>, IEnumerable<CartItemResponseDto>>(items);

            return Ok(new { Items = itemDtos, Total = Math.Round(total, 2) });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCartAsync()
        {
            var userId = User.Identity.Name;

            var items = await _context.CartItems.Where(i => i.UserId == userId).ToListAsync();

            _context.RemoveRange(items);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("add")]
        public async Task<IActionResult> AddToCartAsync(string productId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var item = _context.CartItems.SingleOrDefault(i => i.ProductId == productId);

            if (item == null)
            {
                item = new CartItem
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = User.Identity.Name,
                    ProductId = productId
                };

                _context.CartItems.Add(item);
            }

            item.Count++;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("remove")]
        public async Task<IActionResult> RemoveFromCartAsync(string productId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var item = _context.CartItems.SingleOrDefault(i => i.ProductId == productId);

            if (item == null) return NotFound();

            item.Count--;

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}