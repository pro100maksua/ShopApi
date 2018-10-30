using System.Collections.Generic;

namespace ShopApi.Logic.Dtos.Responses
{
    public class CartResponseDto
    {
        public IEnumerable<CartItemResponseDto> Items { get; set; }

        public double Total { get; set; }
    }
}