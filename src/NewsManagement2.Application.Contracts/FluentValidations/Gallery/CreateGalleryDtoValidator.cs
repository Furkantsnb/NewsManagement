using FluentValidation;
using Microsoft.Extensions.Localization;
using NewsManagement2.EntityDtos.GalleryDtos;
using NewsManagement2.FluentValidations.ListableContent;
using NewsManagement2.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsManagement2.FluentValidations.Gallery
{
    public class CreateGalleryDtoValidator : AbstractValidator<CreateGalleryDto>
    {
        public CreateGalleryDtoValidator(IStringLocalizer<NewsManagement2Resource> localizer)
        {
            Include(new CreateListableContentDtoValidator(localizer));

            RuleFor(x => x.GalleryImages).ForEach(x => x.SetValidator(new GalleryImageDtoValidator()));

        }
    }
}
