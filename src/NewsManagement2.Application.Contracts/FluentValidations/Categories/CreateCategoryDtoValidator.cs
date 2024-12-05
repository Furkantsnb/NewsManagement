using FluentValidation;
using Microsoft.Extensions.Localization;
using NewsManagement2.EntityDtos.CategoryDtos;
using NewsManagement2.Localization;
using System;
using System.Collections.Generic;
using System.Text;





namespace NewsManagement2.FluentValidations.Categories
{
  

    public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryDtoValidator(IStringLocalizer<NewsManagement2Resource> localizer)
        {
            // CategoryName doğrulama
            RuleFor(c => c.CategoryName)
                .NotEmpty().WithMessage(localizer["CategoryNameCannotBeEmpty"])
                .Length(3, 100).WithMessage(localizer["CategoryNameLengthError"]);

            // ColorCode doğrulama
            RuleFor(c => c.ColorCode)
                .NotEmpty().WithMessage(localizer["ColorCodeCannotBeEmpty"])
                .Matches("^#(?:[0-9a-fA-F]{3}){1,2}$").WithMessage(localizer["InvalidColorCodeFormat"]);

            // ListableContentType doğrulama
            RuleFor(c => c.listableContentType)
                .IsInEnum().WithMessage(localizer[NewsManagement2DomainErrorCodes.InvalidContentTypeSelection]);

            // ParentCategoryId doğrulama
            RuleFor(c => c.ParentCategoryId)
                .GreaterThan(0).WithMessage(localizer["ParentCategoryIdMustBePositive"])
                .When(c => c.ParentCategoryId.HasValue);

            // IsActive kontrolü - ek bir örnek olarak dahil edilebilir
            RuleFor(c => c.IsActive)
                .NotNull().WithMessage(localizer["IsActiveCannotBeNull"]);
        }
    }
}
