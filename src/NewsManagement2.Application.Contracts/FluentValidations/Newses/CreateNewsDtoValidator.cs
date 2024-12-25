using FluentValidation;
using Microsoft.Extensions.Localization;
using NewsManagement2.EntityDtos.Newses;
using NewsManagement2.FluentValidations.ListableContent;
using NewsManagement2.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsManagement2.FluentValidations.Newses
{
    public class CreateNewsDtoValidator : AbstractValidator<CreateNewsDto>
    {
        public CreateNewsDtoValidator(IStringLocalizer<NewsManagement2Resource> localizer)
        {
            Include(new CreateListableContentDtoValidator(localizer));

            RuleFor(x => x.DetailImageIds).ForEach(x => x.SetValidator(new NewsDetailImageDtoValidator()));
        }
    }
}
