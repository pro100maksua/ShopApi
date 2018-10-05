using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShopApi.Dtos;
using ShopApi.Enums;
using ShopApi.Models;

namespace ShopApi.Services
{
    public interface IUsersService
    {
        Task<string> LoginAsync(LoginDto loginDto);
        Task<IdentityResult> RegisterAsync(RegisterDto registerDto);
    }

    public class UsersService : IUsersService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;

        public UsersService(IConfiguration config, UserManager<User> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                var key = Encoding.ASCII.GetBytes(_config["Secret"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, user.Role)
                    }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return tokenString;
            }

            return string.Empty;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDto registerDto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = registerDto.UserName,
                Role = Role.Admin.ToString()
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            return result;
        }
    }
}