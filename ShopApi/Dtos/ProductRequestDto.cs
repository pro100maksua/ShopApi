namespace ShopApi.Dtos
{
    public class ProductRequestDto
    {
        public string Name { get; set; }

        public double Cost { get; set; }

        public string CategoryId { get; set; }
    }
}