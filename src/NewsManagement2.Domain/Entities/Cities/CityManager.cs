using NewsManagement2.Entities.Exceptions;
using NewsManagement2.EntityDtos.CityDtos;
using NewsManagement2.EntityDtos.PagedAndSortedDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
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

        public async Task<CityDto>UpdateAsync(int id , UpdateCityDto updateCityDto)
        {
            var existingCity = await _cityRepository.GetAsync(id);

            var cityAlreadyExists = await _cityRepository.AnyAsync(c =>
                                         c.CityName == updateCityDto.CityName);
            if (cityAlreadyExists)
                throw new AlreadyExistException(typeof(City), updateCityDto.CityName.ToString());
            var cityCodeAlreadyExists = await _cityRepository.AnyAsync(c => c.CityCode == updateCityDto.CityCode);

            if (cityCodeAlreadyExists)
                throw new BusinessException(NewsManagement2DomainErrorCodes.DuplicateCityCode);

            _objectMapper.Map(updateCityDto, existingCity);
            var city = await _cityRepository.UpdateAsync(existingCity,autoSave:true);
            var cityDto = _objectMapper.Map<City, CityDto>(city);

            return cityDto;



        }
        public async Task<PagedResultDto<CityDto>> GetListAsync(GetListPagedAndSortedDto input)
        {
            var totalCount = input.Filter == null
       ? await _cityRepository.CountAsync()
       : await _cityRepository.CountAsync(c => c.CityName.Contains(input.Filter));

            if (totalCount == 0)
                throw new NotFoundException(typeof(City), input.Filter ?? string.Empty);

            if (input.SkipCount >= totalCount)
                throw new BusinessException(NewsManagement2DomainErrorCodes.InvalidFilterCriteria);

            if (input.Sorting.IsNullOrWhiteSpace())
                input.Sorting = nameof(City.CityName);

            var cityList = await _cityRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter);

            var cityDtoList = _objectMapper.Map<List<City>, List<CityDto>>(cityList);

            return new PagedResultDto<CityDto>(totalCount, cityDtoList);
        }
        public async Task DeleteAsync(int id)
        {
            var isCityExist = await _cityRepository.AnyAsync(c => c.Id == id);
            if (!isCityExist)
                throw new EntityNotFoundException(typeof(City), id);
        }
        public async Task DeleteHardAsync(int id)
        {
            var city = await _cityRepository.GetAsync(id);

            await _cityRepository.HardDeleteAsync(city);
        }
    }
}
