namespace ShopApi.Dtos.Responses
{
    public class CartItemResponseDto
    {
        public int Count { get; set; }

        public ProductResponseDto Product { get; set; }
    }
}