﻿namespace ShopApi.Models
{
    public class User
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string NormalizedUserName { get; set; }

        public string PasswordHash { get; set; }

        public string Role { get; set; }
    }
}