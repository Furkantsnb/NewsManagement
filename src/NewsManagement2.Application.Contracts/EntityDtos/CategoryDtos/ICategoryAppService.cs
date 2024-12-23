﻿using NewsManagement2.EntityDtos.CityDtos;
using NewsManagement2.EntityDtos.PagedAndSortedDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace NewsManagement2.EntityDtos.CategoryDtos
{
    public interface ICategoryAppService : ICrudAppService<CategoryDto, int, GetListPagedAndSortedDto, CreateCategoryDto, UpdateCategoryDto>
    {
        Task DeleteHardAsync(int id);
    }
}
