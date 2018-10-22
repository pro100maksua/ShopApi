using FluentValidation;
using ShopApi.Dtos.Requests;

namespace ShopApi.Validators
{
    public class LoginValidator : AbstractValidator<LoginRequest>
    {
        public LoginValidator()
        {
            RuleFor(l => l.UserName).NotEmpty();
            RuleFor(l => l.Password).NotEmpty();
        }
    }
}