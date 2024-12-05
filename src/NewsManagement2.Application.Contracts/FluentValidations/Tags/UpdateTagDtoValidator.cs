using FluentValidation;
using NewsManagement2.EntityDtos.TagDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsManagement2.FluentValidations.Tags
{
    public class UpdateTagDtoValidator : AbstractValidator<UpdateTagDto>
    {
        public UpdateTagDtoValidator()
        {
            // TagName boş olamaz
            RuleFor(t => t.TagName)
                .NotEmpty().WithMessage("Tag adı boş olamaz.")
                .Length(3, 50).WithMessage("Tag adı 3 ile 50 karakter arasında olmalıdır.")
                .Matches("^[a-zA-Z0-9 ]+$").WithMessage("Tag adı yalnızca harf, rakam ve boşluk içerebilir.");

           
        }
    }
}
