using System.ComponentModel.DataAnnotations;

namespace ShopApi.Dtos
{
    public class CategoryRequestDto
    {
        [Required]
        public string Name { get; set; }
    }
}