using System.ComponentModel.DataAnnotations;

namespace ShopApi.Dtos
{
    public class ProductRequestDto
    {
        [Required]
        public string Name { get; set; }

        [Range(0.01, double.MaxValue)]
        public double Cost { get; set; }

        [Required]
        public string CategoryId { get; set; }
    }
}