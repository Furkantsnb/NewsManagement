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

        //Pozitif Test: Geçerli bir şehir adı ile güncelleme
        [Fact]
        public async Task UpdateAsync_ValidCityName_ShouldUpdateSuccessfully()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var validCityId = 2; // SeedData'dan var olan bir şehir ID'si
                var updateCityDto = new UpdateCityDto { CityName = "Updated City", CityCode = 35 };

                // Act
                var result = await _cityAppService.UpdateAsync(validCityId, updateCityDto);

                // Assert
                result.ShouldNotBeNull();
                result.CityName.ShouldBe("Updated City");
            }
        }

        //Negatif Test: Güncellenecek şehir mevcut değilse EntityNotFoundException fırlatılması
        [Fact]
        public async Task UpdateAsync_NonExistentCity_ShouldThrowEntityNotFoundException()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var invalidCityId = 999; // SeedData'da olmayan bir ID
                var updateCityDto = new UpdateCityDto { CityName = "Updated City", CityCode = 34 };

                // Act & Assert
                await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _cityAppService.UpdateAsync(invalidCityId, updateCityDto);
                });
            }
        }

        //Negatif Test: Aynı isimi başka bir şehirde kullanmak
        [Fact]
        public async Task UpdateAsync_CityNameAlreadyExists_ShouldThrowAlreadyExistException()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var validCityId = 2; // SeedData'dan var olan bir ID
                var updateCityDto = new UpdateCityDto { CityName = "İstanbul", CityCode =34 }; // SeedData'da mevcut olan bir şehir adı

                // Act & Assert
                await Assert.ThrowsAsync<AlreadyExistException>(async () =>
                {
                    await _cityAppService.UpdateAsync(validCityId, updateCityDto);
                });
            }
        }

        //Negatif Test: Boş şehir adı ile güncelleme
        [Fact]
        public async Task UpdateAsync_EmptyCityName_ShouldThrowValidationException()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var validCityId = 2; // SeedData'dan var olan bir ID
                var updateCityDto = new UpdateCityDto { CityName = string.Empty };

                // Act & Assert
                await Assert.ThrowsAsync<AbpValidationException>(async () =>
                {
                    await _cityAppService.UpdateAsync(validCityId, updateCityDto);
                });
            }
        }
        //Negatif Test: Çok kısa bir şehir adı ile güncelleme
        [Fact]
        public async Task UpdateAsync_CityNameTooShort_ShouldThrowValidationException()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var validCityId = 2; // SeedData'dan var olan bir ID
                var updateCityDto = new UpdateCityDto { CityName = "A" }; // Geçersiz kısa bir şehir adı

                // Act & Assert
                await Assert.ThrowsAsync<AbpValidationException>(async () =>
                {
                    await _cityAppService.UpdateAsync(validCityId, updateCityDto);
                });
            }
        }

        //Negatif Test: Çok uzun bir şehir adı ile güncelleme
        [Fact]
        public async Task UpdateAsync_CityNameTooLong_ShouldThrowValidationException()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var validCityId = 2; // SeedData'dan var olan bir ID
                var updateCityDto = new UpdateCityDto { CityName = new string('A', 300) }; // Geçersiz uzun bir şehir adı

                // Act & Assert
                await Assert.ThrowsAsync<AbpValidationException>(async () =>
                {
                    await _cityAppService.UpdateAsync(validCityId, updateCityDto);
                });
            }
        }

        //Performans Testi: Şehir adı güncelleme süresi
        [Fact]
        
        public async Task UpdateAsync_Performance_ShouldCompleteInReasonableTime()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var validCityId = 1; // SeedData'dan var olan bir ID
                var updateCityDto = new UpdateCityDto
                {
                    CityName = "NewCity",
                    CityCode = 27 // Geçerli bir şehir kodu
                };

                // Act
                var startTime = DateTime.UtcNow;
                var result = await _cityAppService.UpdateAsync(validCityId, updateCityDto);
                var endTime = DateTime.UtcNow;

                // Assert
                result.ShouldNotBeNull();
                result.CityName.ShouldBe("NewCity");
                (endTime - startTime).TotalMilliseconds.ShouldBeLessThan(2000); // Maksimum 2 saniye
            }
        }


        //Veritabanında güncellenen şehir adının doğru olduğunun doğrulanması
        [Fact]
        public async Task UpdateAsync_ShouldPersistCityNameInDatabase()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var validCityId = 2; // SeedData'dan var olan bir ID
                var updateCityDto = new UpdateCityDto
                {
                    CityName = "Updated City",
                    CityCode = 45 // Geçerli bir şehir kodu
                };

                // Act
                await _cityAppService.UpdateAsync(validCityId, updateCityDto);

                // Assert
                var updatedCity = await _cityAppService.GetAsync(validCityId);
                updatedCity.CityName.ShouldBe("Updated City");
            }
        }
        // Geçerli bir şehir ID'siyle şehri başarıyla siler.
        [Fact]
        public async Task DeleteAsync_ValidCityId_ShouldDeleteSuccessfully()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                int validCityId = 1; // SeedData'da mevcut bir CityId

                // Act
                await _cityAppService.DeleteAsync(validCityId);

                // Assert
                var allCities = await _cityAppService.GetListAsync(new GetListPagedAndSortedDto());
                allCities.Items.ShouldNotContain(c => c.Id == validCityId); // Silinen ID listeye dahil olmamalı
            }
        }
        //Geçersiz bir şehir ID'siyle silme işlemi yapıldığında EntityNotFoundException fırlatılır.
        [Fact]
        public async Task DeleteAsync_InvalidCityId_ShouldThrowEntityNotFoundException()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                int invalidCityId = 9999; // SeedData'da bulunmayan bir CityId

                // Act & Assert
                await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _cityAppService.DeleteAsync(invalidCityId);
                });
            }
        }
        //Daha önce silinmiş bir şehir ID'siyle tekrar silme işlemi yapıldığında EntityNotFoundException fırlatılır.
        [Fact]
        public async Task DeleteAsync_DeletedCityId_ShouldThrowEntityNotFoundException()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                int cityId = 1; // SeedData'dan geçerli bir CityId
                await _cityAppService.DeleteAsync(cityId); // İlk silme işlemi

                // Act & Assert
                await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _cityAppService.DeleteAsync(cityId); // Tekrar silme işlemi
                });
            }
        }

        //Silinen bir şehrin GetListAsync çağrısında dönen listede yer almadığını doğrular.
        [Fact]
        public async Task DeleteAsync_ValidCityId_ShouldNotExistInList()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                int validCityId = 1; // SeedData'dan bir CityId

                // Act
                await _cityAppService.DeleteAsync(validCityId);

                // Assert
                var allCities = await _cityAppService.GetListAsync(new GetListPagedAndSortedDto());
                allCities.Items.Any(c => c.Id == validCityId).ShouldBeFalse();
            }
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

        //Geçerli bir istekle tüm sonuçların döndüğünü doğrular.
        [Fact]
        public async Task GetListAsync_ValidRequest_ShouldReturnAllCities()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var input = new GetListPagedAndSortedDto
                {
                    SkipCount = 0,
                    MaxResultCount = 10,
                    Sorting = nameof(CityDto.CityName)
                };

                // Act
                var result = await _cityAppService.GetListAsync(input);

                // Assert
                result.ShouldNotBeNull();
                result.Items.Count.ShouldBeGreaterThan(0);
                result.TotalCount.ShouldBeGreaterThan(0);
            }
        }


        //Hiçbir sonuç dönmeyen bir filtre kullanıldığında NotFoundException fırlatıldığını doğrular.
        [Fact]
        public async Task GetListAsync_InvalidFilter_ShouldThrowNotFoundException()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var input = new GetListPagedAndSortedDto
                {
                    Filter = "InvalidCityName", // Geçersiz bir filtre
                    SkipCount = 0,
                    MaxResultCount = 10,
                    Sorting = nameof(CityDto.CityName)
                };

                // Act & Assert
                await Assert.ThrowsAsync<NotFoundException>(async () =>
                {
                    await _cityAppService.GetListAsync(input);
                });
            }
        }

        //SkipCount toplam sonuç sayısından büyük olduğunda BusinessException fırlatıldığını kontrol eder.

        [Fact]
        public async Task GetListAsync_SkipCountExceedsTotal_ShouldThrowBusinessException()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var input = new GetListPagedAndSortedDto
                {
                    SkipCount = 100, // Daha büyük bir değer vererek hata tetiklenir
                    MaxResultCount = 10,
                    Sorting = nameof(CityDto.CityName)
                };

                // Act & Assert
                await Assert.ThrowsAsync<BusinessException>(async () =>
                {
                    await _cityAppService.GetListAsync(input);
                });
            }
        }


        //Filtre boş olduğunda tüm sonuçların döndüğünü doğrular.
        [Fact]
        public async Task GetListAsync_EmptyFilter_ShouldReturnAllCities()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var input = new GetListPagedAndSortedDto
                {
                    Filter = string.Empty, // Filtre boş
                    SkipCount = 0,
                    MaxResultCount = 10,
                    Sorting = nameof(CityDto.CityName)
                };

                // Act
                var result = await _cityAppService.GetListAsync(input);

                // Assert
                result.ShouldNotBeNull();
                result.Items.Count.ShouldBeGreaterThan(0);
                result.TotalCount.ShouldBeGreaterThan(0);
            }
        }
    }
}
