﻿using FluentValidation;
using ShopApi.Dtos.Requests;

namespace ShopApi.Validators
{
    public class CategoryValidator : AbstractValidator<CategoryRequestDto>
    {
        public CategoryValidator()
        {
            RuleFor(c => c.Name).NotEmpty();
        }
    }
}