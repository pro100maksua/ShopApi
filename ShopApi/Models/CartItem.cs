﻿using System;

namespace ShopApi.Models
{
    public class CartItem
    {
        public Guid Id { get; set; }

        public int Count { get; set; }

        public Guid UserId { get; set; }

        public Guid ProductId { get; set; }

        public Product Product { get; set; }
    }
}