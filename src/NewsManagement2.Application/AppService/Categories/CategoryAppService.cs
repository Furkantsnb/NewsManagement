using Microsoft.AspNetCore.Authorization;
using NewsManagement2.Entities.Categories;
using NewsManagement2.EntityDtos.CategoryDtos;
using NewsManagement2.EntityDtos.PagedAndSortedDto;
using NewsManagement2.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace NewsManagement2.AppService.Categories
{
    [Authorize(NewsManagement2Permissions.Categories.Default)]
    public class CategoryAppService : CrudAppService<Category, CategoryDto, int, GetListPagedAndSortedDto, CreateCategoryDto, UpdateCategoryDto>, ICategoryAppService
    {
        private readonly CategoryManager _categoryManager;

        public CategoryAppService(IRepository<Category, int> repository, CategoryManager categoryManager) : base(repository)
        {
            _categoryManager = categoryManager;
        }



    }
}
