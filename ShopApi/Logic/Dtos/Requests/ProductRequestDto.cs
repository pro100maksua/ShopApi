using System;

namespace ShopApi.Logic.Dtos.Requests
{
    public class ProductRequestDto
    {
        public string Name { get; set; }
        
        public double Cost { get; set; }
        
        public Guid CategoryId { get; set; }
    }
}