using System.Threading.Tasks;
using ShopApi.Logic.Dtos.Requests;
using ShopApi.Logic.Dtos.Responses;

namespace ShopApi.Logic.Interfaces
{
    public interface IUsersService
    {
        Task<string> LoginAsync(LoginRequestDto loginRequestDto);
        Task<Result<string>> RegisterAsync(RegisterRequestDto registerRequestDto);
    }
}