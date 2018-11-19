using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShopApi.Data.Models;
using ShopApi.Logic.Dtos.Requests;
using ShopApi.Logic.Dtos.Responses;
using ShopApi.Logic.Enums;
using ShopApi.Logic.Interfaces;

namespace ShopApi.Logic.Services
{
    public class UsersService : IUsersService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;

        public UsersService(IConfiguration config, UserManager<User> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public async Task<string> LoginAsync(LoginRequestDto loginRequestDto)
        {
            var user = await _userManager.FindByNameAsync(loginRequestDto.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginRequestDto.Password))
            {
                return GetUserToken(user);
            }

            return string.Empty;
        }

        public async Task<Result<string>> RegisterAsync(RegisterRequestDto registerRequestDto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = registerRequestDto.UserName,
                Role = Role.Customer.ToString()
            };

            var registerResult = await _userManager.CreateAsync(user, registerRequestDto.Password);

            var authResult = new Result<string>();
            if (registerResult.Succeeded)
            {
                authResult.Data = GetUserToken(user);
            }
            else
            {
                authResult.Errors = registerResult.Errors.Select(e => e.Description).ToList();
            }

            return authResult;
        }

        private string GetUserToken(User user)
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
    }
}