using System;

namespace ShopApi.Data.Models
{
    public class Product
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public double Cost { get; set; }

        public Guid CategoryId { get; set; }

        public Category Category { get; set; }
    }
}