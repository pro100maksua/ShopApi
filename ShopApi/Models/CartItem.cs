namespace ShopApi.Models
{
    public class CartItem
    {
        public string Id { get; set; }

        public int Count { get; set; }

        public string UserId { get; set; }

        public string ProductId { get; set; }

        public Product Product { get; set; }
    }
}