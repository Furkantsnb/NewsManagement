using NewsManagement2.AppService.Cities;
using NewsManagement2.Entities.Exceptions;
using NewsManagement2.EntityDtos.CityDtos;
using NewsManagement2.EntityDtos.PagedAndSortedDto;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Validation;
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

        // Pozitif Test: Geçerli bir city oluşturma
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
        public async Task CreateAsync_ValidCity_ShouldCreateSuccessfully()
        {
            // Arrange
            var createCityDto = new CreateCityDto
            {
                CityName = "Istanbul",
                CityCode = 34
            };

            // Act
            var result = await _cityAppService.CreateAsync(createCityDto);

            // Assert
            result.ShouldNotBeNull();
            result.CityName.ShouldBe("Istanbul");
            result.CityCode.ShouldBe(34);
        }



        // Negatif Test: Geçersiz şehir kodu (1 ile 83 dışında bir değer) ile oluşturma
        [Fact]
        public async Task CreateAsync_InvalidCityCode_ShouldThrowBusinessException()
        {
            // Arrange
            var createCityDto = new CreateCityDto { CityName = "TestCity", CityCode = 999 };

            // Act & Assert
            await Assert.ThrowsAsync<AbpValidationException>(async () =>
            {
                await _cityAppService.CreateAsync(createCityDto);
            });
        }


        // Negatif Test: Boş bir CityName ile oluşturma
        [Fact]
        public async Task CreateAsync_EmptyCityName_ShouldThrowBusinessException()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var createCityDto = new CreateCityDto { CityName = string.Empty };

                // Act & Assert
                await Assert.ThrowsAsync<AbpValidationException>(async () =>
                {
                    await _cityAppService.CreateAsync(createCityDto);
                });
            }
        }

        // Negatif Test: Maksimum uzunluğu aşan CityName
        [Fact]
        public async Task CreateAsync_CityNameExceedsMaxLength_ShouldThrowBusinessException()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var createCityDto = new CreateCityDto { CityName = new string('A', 300) };

                // Act & Assert
                await Assert.ThrowsAsync<AbpValidationException>(async () =>
                {
                    await _cityAppService.CreateAsync(createCityDto);
                });
            }
        }

        //Negatif Test: Şehir adı çok kısa (2 karakterden az) ile oluşturma
        [Fact]
        public async Task CreateAsync_CityNameTooShort_ShouldThrowBusinessException()
        {
            // Arrange
            var createCityDto = new CreateCityDto { CityName = "A", CityCode = 34 };

            // Act & Assert
            await Assert.ThrowsAsync<AbpValidationException>(async () =>
            {
                await _cityAppService.CreateAsync(createCityDto);
            });
        }


        // Performans Testi: City oluşturma süresi
        [Fact]
        public async Task CreateAsync_Performance_ShouldCompleteInReasonableTime()
        {
            // Arrange
            var createCityDto = new CreateCityDto { CityName = "UniqueCity", CityCode = 70 };

            // Act
            var startTime = DateTime.UtcNow;
            var result = await _cityAppService.CreateAsync(createCityDto);
            var endTime = DateTime.UtcNow;

            // Assert
            result.ShouldNotBeNull();
            result.CityName.ShouldBe("UniqueCity");
            (endTime - startTime).TotalMilliseconds.ShouldBeLessThan(2000); // Maksimum 2 saniye
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
