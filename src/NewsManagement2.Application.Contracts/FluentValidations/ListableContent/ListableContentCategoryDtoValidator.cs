using FluentValidation;
using NewsManagement2.EntityDtos.ListableContentDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsManagement2.FluentValidations.ListableContent
{
    public class ListableContentCategoryDtoValidator : AbstractValidator<ListableContentCategoryDto>
    {
        public ListableContentCategoryDtoValidator()
        {
            RuleFor(x => x.CategoryId).NotEmpty();

        }
    }
}
