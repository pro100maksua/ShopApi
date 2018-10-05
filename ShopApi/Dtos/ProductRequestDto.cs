using System;
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
        public Guid CategoryId { get; set; }
    }
}