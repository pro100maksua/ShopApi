using System;

namespace ShopApi.Logic.Dtos.Responses
{
    public class ProductWithIncludeResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public double Cost { get; set; }

        public string Description { get; set; }

        public CategoryResponseDto Category { get; set; }
    }
}