using FluentValidation;
using Microsoft.Extensions.Localization;
using NewsManagement2.EntityDtos.ListableContentDtos;
using NewsManagement2.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace NewsManagement2.FluentValidations.ListableContent
{
    public class UpdateListableContentDtoValidator : AbstractValidator<UpdateListableContentDto>
    {
        public UpdateListableContentDtoValidator(IStringLocalizer<NewsManagement2Resource> localizer)
        {
            RuleFor(l => l.Title).NotEmpty();
            RuleFor(l => l.Spot).NotEmpty();
            RuleFor(l => l.TagIds).NotEmpty();
            RuleFor(l => l.CityIds).NotEmpty();
            RuleFor(l => l.Status).IsInEnum().WithMessage(localizer[NewsManagement2DomainErrorCodes.StatusEnumValidationFailed]);

            RuleFor(l => l.ListableContentCategoryDtos)
              .Must(cat => cat == null || cat.Count(c => c.IsPrimary) == 1)
              .WithMessage(x => string.Format(
                localizer[NewsManagement2DomainErrorCodes.ActiveCategoryLimitExceeded],
                x.ListableContentCategoryDtos?.Count(c => c.IsPrimary) ?? 0)
              );

        }
    }
}
