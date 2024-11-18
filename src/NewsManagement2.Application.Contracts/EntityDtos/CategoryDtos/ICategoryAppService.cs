using NewsManagement2.EntityDtos.CityDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace NewsManagement2.EntityDtos.CategoryDtos
{
    internal interface ICategoryAppService : ICrudAppService<CategoryDto, int, CreateCategoryDto, UpdateCategoryDto>
    {
        Task DeleteHardAsync(int id);
    }
}
