using System.Collections.Generic;

namespace ShopApi.Dtos
{
    public class CartDto
    {
        public IEnumerable<CartItemResponseDto> Items { get; set; }

        public double Total { get; set; }
    }
}