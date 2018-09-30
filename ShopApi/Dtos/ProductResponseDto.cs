namespace ShopApi.Dtos
{
    public class ProductResponseDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public double Cost { get; set; }

        public CategoryResponseDto Category { get; set; }
    }
}