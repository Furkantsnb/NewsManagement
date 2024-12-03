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
using NewsManagement2.EntityDtos.CategoryDtos;
using Volo.Abp.Application.Dtos;

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


        [Authorize(NewsManagement2Permissions.Categories.Create)]
        public override async Task<CategoryDto> CreateAsync(CreateCategoryDto createCategoryDto)
        {
            return await _categoryManager.CreateAsync(createCategoryDto);
        }

        [Authorize(NewsManagement2Permissions.Categories.Edit)]
        public async override Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto updateCategoryDto)
        {
            return await _categoryManager.UpdateAsync(id, updateCategoryDto);
        }

        public async override Task<PagedResultDto<CategoryDto>> GetListAsync(GetListPagedAndSortedDto input)
        {
            return await _categoryManager.GetListAsync(input);
        }

        [Authorize(NewsManagement2Permissions.Categories.Delete)]
        public override async Task DeleteAsync(int id)
        {
            var category = await _categoryManager.DeleteAsync(id);

            foreach (var item in category)
                await base.DeleteAsync(item.Id);
        }

        [Authorize(NewsManagement2Permissions.Categories.Delete)]
        public async Task DeleteHardAsync(int id)
        {
            await _categoryManager.DeleteHardAsync(id);
        }

        public async Task<List<Category>> GetSubCategoriesById(int id)
        {
            return await _categoryManager.GetSubCategoriesByIdAsync(id);
        }
    }
}
