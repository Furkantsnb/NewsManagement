using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace NewsManagement2.EntityDtos.CityDtos
{
    public interface ICityAppService : ICrudAppService<CityDto, int, CreateCityDto, UpdateCityDto>
    {
        Task DeleteHardAsync(int id);
    }
}
