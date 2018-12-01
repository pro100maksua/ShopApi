using FluentValidation;
using ShopApi.Logic.Dtos.Requests;

namespace ShopApi.Validators
{
    public class FetchRequestValidator : AbstractValidator<FetchRequestDto>
    {
        public FetchRequestValidator()
        {
            RuleFor(f => f.Take).NotEmpty();
        }
    }
}