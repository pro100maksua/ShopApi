using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Dtos;
using ShopApi.Services;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var token = await _usersService.LoginAsync(loginDto);

            if (string.IsNullOrWhiteSpace(token)) return NotFound();

            return Ok(token);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody]RegisterDto registerDto)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid data.");

            var result = await _usersService.RegisterAsync(registerDto);

            return Ok(result);
        }
    }
}