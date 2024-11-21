using NewsManagement2.Entities.Cities;
using NewsManagement2.EntityDtos.CityDtos;
using NewsManagement2.EntityDtos.PagedAndSortedDto;
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
    public class CityAppService : CrudAppService<City, CityDto, int, GetListPagedAndSortedDto, CreateCityDto, UpdateCityDto>, ICityAppService
    {
        private readonly CityManager _cityManager;

        public CityAppService(IRepository<City, int> repository, CityManager cityManager) : base(repository)
        {
            _cityManager = cityManager;
        }
        public override async Task<CityDto> CreateAsync(CreateCityDto createCityDto)
        {
            return await _cityManager.CreateAsync(createCityDto);
        }
        public async override Task<CityDto> UpdateAsync(int id, UpdateCityDto updateCityDto)
        {
            return await _cityManager.UpdateAsync(id, updateCityDto);
        }
        public async override Task<PagedResultDto<CityDto>> GetListAsync(GetListPagedAndSortedDto input)
        {
            return await _cityManager.GetListAsync(input);
        }
        public override async Task DeleteAsync(int id)
        {
            await _cityManager.DeleteAsync(id);

            await base.DeleteAsync(id);
        }
        public async Task DeleteHardAsync(int id)
        {
            await _cityManager.DeleteHardAsync(id);
        }
    }
}
