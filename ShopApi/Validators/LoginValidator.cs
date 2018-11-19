using FluentValidation;
using ShopApi.Logic.Dtos.Requests;

namespace ShopApi.Validators
{
    public class LoginValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginValidator()
        {
            RuleFor(l => l.UserName).NotEmpty();
            RuleFor(l => l.Password).NotEmpty();
        }
    }
}