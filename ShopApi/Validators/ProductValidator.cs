using FluentValidation;
using ShopApi.Dtos.Requests;

namespace ShopApi.Validators
{
    public class ProductValidator : AbstractValidator<ProductRequestDto>
    {
        public ProductValidator()
        {
            RuleFor(p => p.Name).NotEmpty();
            RuleFor(p => p.Cost).NotEmpty();
            RuleFor(p => p.CategoryId).NotEmpty();
        }
    }
}