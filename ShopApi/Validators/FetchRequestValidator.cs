using FluentValidation;
using ShopApi.Dtos.Requests;

namespace ShopApi.Validators
{
    public class FetchRequestValidator : AbstractValidator<FetchRequest>
    {
        public FetchRequestValidator()
        {
            RuleFor(f => f.Take).NotEmpty();
        }
    }
}