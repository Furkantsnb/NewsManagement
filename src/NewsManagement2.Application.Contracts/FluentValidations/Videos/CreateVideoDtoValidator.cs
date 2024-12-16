using FluentValidation;
using Microsoft.Extensions.Localization;
using NewsManagement2.EntityDtos.VideoDtos;
using NewsManagement2.FluentValidations.ListableContent;
using NewsManagement2.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsManagement2.FluentValidations.Videos
{
    public class CreateVideoDtoValidator : AbstractValidator<CreateVideoDto>
    {
        public CreateVideoDtoValidator(IStringLocalizer<NewsManagement2Resource> localizer)
        {
            Include(new CreateListableContentDtoValidator(localizer));

            RuleFor(v => v.VideoType).IsInEnum().WithMessage(localizer[NewsManagement2DomainErrorCodes.VideoEnumValidationFailed]);
        }
    }
}
