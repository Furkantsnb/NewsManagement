using FluentValidation;
using NewsManagement2.EntityDtos.Newses;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsManagement2.FluentValidations.Newses
{
    public class NewsDetailImageDtoValidator : AbstractValidator<NewsDetailImageDto>
    {
        public NewsDetailImageDtoValidator()
        {
            RuleFor(n => n.DetailImageId).NotNull();
        }
    }
}
