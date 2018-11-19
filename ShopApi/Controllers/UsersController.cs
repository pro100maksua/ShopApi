using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Logic.Dtos.Requests;
using ShopApi.Logic.Interfaces;

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
        public async Task<ActionResult<string>> LoginAsync([FromBody] LoginRequestDto loginRequestDto)
        {
            var token = await _usersService.LoginAsync(loginRequestDto);
            if (string.IsNullOrWhiteSpace(token))
            {
                return NotFound();
            }

            return token;
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> RegisterAsync([FromBody]RegisterRequestDto registerRequestDto)
        {
            var result = await _usersService.RegisterAsync(registerRequestDto);
            if (string.IsNullOrWhiteSpace(result.Data))
            {
                return BadRequest(result.Errors);
            }

            return result.Data;
        }
    }
}