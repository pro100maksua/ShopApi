﻿using System.ComponentModel.DataAnnotations;

namespace ShopApi.Dtos
{
    public class RegisterDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Compare("Password")]
        [Required]
        public string ConfirmPassword { get; set; }
    }
}