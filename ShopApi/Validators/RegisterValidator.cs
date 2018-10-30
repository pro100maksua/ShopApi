using FluentValidation;
using ShopApi.Logic.Dtos.Requests;

namespace ShopApi.Validators
{
    public class RegisterValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterValidator()
        {
            RuleFor(r => r.UserName).NotEmpty();
            RuleFor(r => r.Password).NotEmpty();
            RuleFor(r => r.ConfirmPassword).NotEmpty().Equal(r => r.Password);
        }
    }
}
