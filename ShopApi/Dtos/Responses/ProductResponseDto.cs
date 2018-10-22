﻿using System;

namespace ShopApi.Dtos.Responses
{
    public class ProductResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public double Cost { get; set; }

        public CategoryResponseDto Category { get; set; }
    }
}