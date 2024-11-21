using Microsoft.AspNetCore.Authorization;
using NewsManagement2.Entities.Cities;
using NewsManagement2.EntityDtos.CityDtos;
using NewsManagement2.EntityDtos.PagedAndSortedDto;
using NewsManagement2.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace NewsManagement2.AppService.Cities
{
    [Authorize(NewsManagement2Permissions.Cities.Default)]
    public class CityAppService : CrudAppService<City, CityDto, int, GetListPagedAndSortedDto, CreateCityDto, UpdateCityDto>, ICityAppService
    {
        private readonly CityManager _cityManager;

        public CityAppService(IRepository<City, int> repository, CityManager cityManager) : base(repository)
        {
            _cityManager = cityManager;
        }
        [Authorize(NewsManagement2Permissions.Cities.Create)]
        public override async Task<CityDto> CreateAsync(CreateCityDto createCityDto)
        {
            return await _cityManager.CreateAsync(createCityDto);
        }
        [Authorize(NewsManagement2Permissions.Cities.Edit)]
        public async override Task<CityDto> UpdateAsync(int id, UpdateCityDto updateCityDto)
        {
            return await _cityManager.UpdateAsync(id, updateCityDto);
        }
        public async override Task<PagedResultDto<CityDto>> GetListAsync(GetListPagedAndSortedDto input)
        {
            return await _cityManager.GetListAsync(input);
        }
        [Authorize(NewsManagement2Permissions.Cities.Delete)]
        public override async Task DeleteAsync(int id)
        {
            await _cityManager.DeleteAsync(id);

            await base.DeleteAsync(id);
        }

        [Authorize(NewsManagement2Permissions.Cities.Delete)]
        public async Task DeleteHardAsync(int id)
        {
            await _cityManager.DeleteHardAsync(id);
        }
    }
}
