using NewsManagement2.AppService.Cities;
using NewsManagement2.Entities.Exceptions;
using NewsManagement2.EntityDtos.CityDtos;
using NewsManagement2.EntityDtos.PagedAndSortedDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace NewsManagement2.City
{
    public class CityAppService_Test : NewsManagement2ApplicationTestBase
    {
        private readonly CityAppService _cityAppService;
        private readonly IDataFilter<IMultiTenant> _dataFilter;
        public CityAppService_Test()
        {
            _cityAppService = GetRequiredService<CityAppService>();
            _dataFilter = GetRequiredService<IDataFilter<IMultiTenant>>();
        }

        [Fact]
        public async Task CreateAsync_ValidCity_ShouldCreateCity()
        {
            // Arrange
            var createDto = new CreateCityDto
            {
                CityName = "Ankara",
                CityCode = 6
            };

            // Act
            var result = await _cityAppService.CreateAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Ankara", result.CityName);
            Assert.Equal(6, result.CityCode);
        }



        [Fact]
        public async Task DeleteHardAsync_ValidCity_ShouldHardDeleteCity()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var existingCity = (await _cityAppService.GetListAsync(new GetListPagedAndSortedDto { Filter = "Elazığ" })).Items.First();

                // Act
                await _cityAppService.DeleteHardAsync(existingCity.Id);

                // Assert
                await Assert.ThrowsAsync<EntityNotFoundException>(() => _cityAppService.GetAsync(existingCity.Id));
            }
        }

        [Fact]
        public async Task GetAsync_ValidCity_ShouldReturnCity()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var existingCity = (await _cityAppService.GetListAsync(new GetListPagedAndSortedDto { Filter = "Malatya" })).Items.First();

                // Act
                var result = await _cityAppService.GetAsync(existingCity.Id);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Malatya", result.CityName);
                Assert.Equal(44, result.CityCode);
            }
        }


    }
}
