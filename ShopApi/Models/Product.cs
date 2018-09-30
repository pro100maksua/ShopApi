namespace ShopApi.Models
{
    public class Product
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public double Cost { get; set; }

        public string CategoryId { get; set; }

        public Category Category { get; set; }
    }
}