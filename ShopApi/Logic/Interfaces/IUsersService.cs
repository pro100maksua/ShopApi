using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ShopApi.Logic.Dtos.Requests;

namespace ShopApi.Logic.Interfaces
{
    public interface IUsersService
    {
        Task<string> LoginAsync(LoginRequest loginRequest);
        Task<IdentityResult> RegisterAsync(RegisterRequest registerRequest);
    }
}