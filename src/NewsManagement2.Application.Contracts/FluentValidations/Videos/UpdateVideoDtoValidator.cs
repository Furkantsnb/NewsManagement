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
    public class UpdateVideoDtoValidator : AbstractValidator<UpdateVideoDto>
    {
        public UpdateVideoDtoValidator(IStringLocalizer<NewsManagement2Resource> localizer)
        {
            Include(new UpdateListableContentDtoValidator(localizer));

            RuleFor(v => v.VideoType).IsInEnum().WithMessage(localizer[NewsManagement2DomainErrorCodes.VideoEnumValidationFailed]);
        }
    }
}
