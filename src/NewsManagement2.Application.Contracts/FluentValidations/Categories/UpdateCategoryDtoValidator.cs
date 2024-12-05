using FluentValidation;
using Microsoft.Extensions.Localization;
using NewsManagement2.EntityDtos.CategoryDtos;
using NewsManagement2.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsManagement2.FluentValidations.Categories
{
    public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
    {
        public UpdateCategoryDtoValidator(IStringLocalizer<NewsManagement2Resource> localizer)
        {
            RuleFor(c => c.CategoryName).NotEmpty();
            RuleFor(c => c.ColorCode).NotEmpty();

            RuleFor(c => c.listableContentType).IsInEnum().WithMessage(localizer[NewsManagement2DomainErrorCodes.InvalidContentTypeSelection]);

        }
    }
}
