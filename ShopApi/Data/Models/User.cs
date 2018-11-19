using System;

namespace ShopApi.Data.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string NormalizedUserName { get; set; }

        public string PasswordHash { get; set; }

        public string Role { get; set; }
    }
}