using NewsManagement2.Entities.Cities;
using NewsManagement2.EntityDtos.CityDtos;
using NewsManagement2.EntityDtos.PagedAndSortedDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public Task DeleteHardAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
