using NewsManagement2.Entities.Exceptions;
using NewsManagement2.EntityDtos.CityDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.ObjectMapping;

namespace NewsManagement2.Entities.Cities
{
    public class CityManager : DomainService
    {
        private readonly ICityRepository _cityRepository;
        private readonly IObjectMapper _objectMapper;

        public CityManager(ICityRepository cityRepository, IObjectMapper objectMapper)
        {
            _cityRepository = cityRepository;
            _objectMapper = objectMapper;
        }

        public async Task<CityDto> CreateAsync(CreateCityDto createCityDto)
        {
            var cityAlreadyExists = await _cityRepository.AnyAsync(c =>
                                          c.CityName == createCityDto.CityName);
            if (cityAlreadyExists)
                throw new AlreadyExistException(typeof(City),createCityDto.CityName.ToString());
            var cityCodeAlreadyExists = await _cityRepository.AnyAsync(c=>c.CityCode==createCityDto.CityCode);

            if (cityCodeAlreadyExists)
                throw new BusinessException(NewsManagement2DomainErrorCodes.DuplicateCityCode);
            
            var createCity =_objectMapper.Map<CreateCityDto,City>(createCityDto);
            var city = await _cityRepository.InsertAsync(createCity,autoSave:true);
            var cityDto = _objectMapper.Map<City,CityDto>(city);
            return cityDto;



        }
    }
}
