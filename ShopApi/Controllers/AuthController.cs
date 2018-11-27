using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Logic.Dtos.Requests;
using ShopApi.Logic.Interfaces;

namespace ShopApi.Controllers
{
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("api/login")]
        public async Task<ActionResult<string>> LoginAsync([FromBody] LoginRequestDto loginRequestDto)
        {
            var token = await _authService.LoginAsync(loginRequestDto);
            if (string.IsNullOrWhiteSpace(token))
            {
                return NotFound();
            }

            return token;
        }

        [HttpPost("api/register")]
        public async Task<ActionResult<string>> RegisterAsync([FromBody]RegisterRequestDto registerRequestDto)
        {
            var result = await _authService.RegisterAsync(registerRequestDto);
            if (string.IsNullOrWhiteSpace(result.Data))
            {
                return BadRequest(result.Errors);
            }

            return result.Data;
        }
    }
}