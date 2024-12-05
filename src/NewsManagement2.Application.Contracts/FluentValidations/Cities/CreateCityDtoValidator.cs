using FluentValidation;
using NewsManagement2.EntityDtos.CityDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsManagement2.FluentValidations.Cities
{
    public class CreateCityDtoValidator : AbstractValidator<CreateCityDto>
    {
        public CreateCityDtoValidator()
        {
            // CityName doğrulamaları
            RuleFor(c => c.CityName)
                .NotEmpty().WithMessage("Şehir adı boş olamaz.")
                .Length(2, 100).WithMessage("Şehir adı 2 ile 100 karakter arasında olmalıdır.")
                .Matches("^[a-zA-ZğüşöçıİĞÜŞÖÇ ]+$").WithMessage("Şehir adı yalnızca harfler ve boşluk içerebilir.");

            // CityCode doğrulamaları (int)
            RuleFor(c => c.CityCode)
                .NotEmpty().WithMessage("Şehir kodu boş olamaz.")
                .InclusiveBetween(1, 83).WithMessage("Şehir kodu 1 ile 83 arasında bir değer olmalıdır."); // Şehir kodu sınırlı bir sayı aralığında
        }
    }
}
