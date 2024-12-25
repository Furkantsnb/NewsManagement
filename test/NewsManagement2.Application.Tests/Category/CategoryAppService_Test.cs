using Microsoft.Extensions.DependencyInjection;
using Moq;
using NewsManagement2.AppService.Categories;
using NewsManagement2.Entities.Categories;
using NewsManagement2.Entities.Exceptions;
using NewsManagement2.EntityConsts.ListableContentConsts;
using NewsManagement2.EntityDtos.CategoryDtos;
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
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using Volo.Abp.Validation;
using Xunit;

namespace NewsManagement2.Category
{
    public class CategoryAppService_Test : NewsManagement2ApplicationTestBase
    {
        private readonly CategoryAppService _categoryAppService;
        private readonly IDataFilter<IMultiTenant> _dataFilter;

        public CategoryAppService_Test()
        {
            _categoryAppService = GetRequiredService<CategoryAppService>();
            _dataFilter = GetRequiredService<IDataFilter<IMultiTenant>>();
        }

        [Fact]
        public async Task CreateAsync_ValidCategory_ShouldCreateSuccessfully()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var createCategoryDto = new CreateCategoryDto
                {
                    CategoryName = "Matematik",
                    ColorCode = "#123456",
                    IsActive = true,
                    ParentCategoryId = null,
                    listableContentType = ListableContentType.News
                };

                // Act
                var result = await _categoryAppService.CreateAsync(createCategoryDto);

                // Assert
                result.ShouldNotBeNull();
                result.Id.ShouldBeGreaterThan(0);
                result.CategoryName.ShouldBe("Matematik");
            }
        }

        [Fact]
        public async Task CreateAsync_DuplicateCategoryName_ShouldThrowAlreadyExistException()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var createCategoryDto = new CreateCategoryDto
                {
                    CategoryName = "Yazılım", // SeedData'da mevcut
                    ColorCode = "#123456",
                    IsActive = true,
                    ParentCategoryId = null,
                    listableContentType = ListableContentType.Gallery
                };

                // Act & Assert
                await Assert.ThrowsAsync<AlreadyExistException>(async () =>
                {
                    await _categoryAppService.CreateAsync(createCategoryDto);
                });
            }
        }

        [Fact]
        public async Task CreateAsync_InvalidSubCategoryContentType_ShouldThrowBusinessException()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var parentCategory = await _categoryAppService.GetListAsync(new GetListPagedAndSortedDto
                {
                    Filter = "Yazılım"
                });

                parentCategory.Items.ShouldNotBeEmpty();
                var createCategoryDto = new CreateCategoryDto
                {
                    CategoryName = "Uygulamalar",
                    ColorCode = "#654321",
                    IsActive = true,
                    ParentCategoryId = parentCategory.Items.First().Id,
                    listableContentType = ListableContentType.Video // Farklı bir content type
                };

                // Act & Assert
                await Assert.ThrowsAsync<BusinessException>(async () =>
                {
                    await _categoryAppService.CreateAsync(createCategoryDto);
                });
            }
        }

        [Fact]
        public async Task CreateAsync_EmptyCategoryName_ShouldThrowValidationException()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var createCategoryDto = new CreateCategoryDto
                {
                    CategoryName = "",
                    ColorCode = "#000000",
                    IsActive = true,
                    ParentCategoryId = null,
                    listableContentType = ListableContentType.News
                };

                // Act & Assert
                await Assert.ThrowsAsync<Volo.Abp.Validation.AbpValidationException>(async () =>
                {
                    await _categoryAppService.CreateAsync(createCategoryDto);
                });
            }
        }

        [Fact]
        public async Task CreateAsync_Performance_ShouldCompleteInReasonableTime()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var createCategoryDto = new CreateCategoryDto
                {
                    CategoryName = "Performans Testi",
                    ColorCode = "#abcdef",
                    IsActive = true,
                    ParentCategoryId = null,
                    listableContentType = ListableContentType.News
                };

                // Act
                var startTime = DateTime.UtcNow;
                var result = await _categoryAppService.CreateAsync(createCategoryDto);
                var endTime = DateTime.UtcNow;

                // Assert
                result.ShouldNotBeNull();
                (endTime - startTime).TotalMilliseconds.ShouldBeLessThan(1000); // Maksimum 1 saniye
            }
        }


        /// <summary>
        /// Başarılı bir güncelleme durumunu test eder.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_ValidCategory_ShouldUpdateSuccessfully()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var existingCategory = (await _categoryAppService.GetListAsync(new GetListPagedAndSortedDto { Filter = "Yazılım" })).Items.First();
                var updateCategoryDto = new UpdateCategoryDto
                {
                    CategoryName = "Yazılım ve Teknoloji",
                    ColorCode = "#654321",
                    IsActive = true,
                    ParentCategoryId = null,
                    listableContentType = ListableContentType.Gallery
                };

                // Act
                var result = await _categoryAppService.UpdateAsync(existingCategory.Id, updateCategoryDto);

                // Assert
                result.ShouldNotBeNull();
                result.Id.ShouldBe(existingCategory.Id);
                result.CategoryName.ShouldBe("Yazılım ve Teknoloji");
            }
        }

        /// <summary>
        /// Alt kategori için geçersiz bir üst kategori tipi kullanıldığında BusinessException bekler.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_InvalidSubCategoryContentType_ShouldThrowBusinessException()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var parentCategory = (await _categoryAppService.GetListAsync(new GetListPagedAndSortedDto { Filter = "Yazılım" })).Items.First();
                var existingSubCategory = (await _categoryAppService.GetListAsync(new GetListPagedAndSortedDto { Filter = "Yazılım Mühendisi" })).Items.First();

                var updateCategoryDto = new UpdateCategoryDto
                {
                    CategoryName = "Mobil Uygulamalar",
                    ColorCode = "#654321",
                    IsActive = true,
                    ParentCategoryId = parentCategory.Id,
                    listableContentType = ListableContentType.Video // Çakışan içerik tipi
                };

                // Act & Assert
                await Assert.ThrowsAsync<BusinessException>(async () =>
                {
                    await _categoryAppService.UpdateAsync(existingSubCategory.Id, updateCategoryDto);
                });
            }
        }

        /// <summary>
        /// Geçersiz bir kategori ID kullanıldığında EntityNotFoundException bekler.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_InvalidCategoryId_ShouldThrowEntityNotFoundException()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var updateCategoryDto = new UpdateCategoryDto
                {
                    CategoryName = "Geçersiz Kategori",
                    ColorCode = "#654321",
                    IsActive = true,
                    ParentCategoryId = null,
                    listableContentType = ListableContentType.News
                };

                // Act & Assert
                await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _categoryAppService.UpdateAsync(9999, updateCategoryDto); // Geçersiz ID
                });
            }
        }

        /// <summary>
        /// Performans testi: Güncelleme işlemi makul bir sürede tamamlanmalıdır.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_Performance_ShouldCompleteInReasonableTime()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var existingCategory = (await _categoryAppService.GetListAsync(new GetListPagedAndSortedDto { Filter = "Yazılım" })).Items.First();
                var updateCategoryDto = new UpdateCategoryDto
                {
                    CategoryName = "Performans Testi Kategorisi",
                    ColorCode = "#abcdef",
                    IsActive = true,
                    ParentCategoryId = null,
                    listableContentType = ListableContentType.News
                };

                // Act
                var startTime = DateTime.UtcNow;
                var result = await _categoryAppService.UpdateAsync(existingCategory.Id, updateCategoryDto);
                var endTime = DateTime.UtcNow;

                // Assert
                result.ShouldNotBeNull();
                (endTime - startTime).TotalMilliseconds.ShouldBeLessThan(1000); // Maksimum 1 saniye
            }
        }

        [Fact]
        public async Task GetListAsync_ShouldReturnPagedResults_WhenCalledWithValidInput()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var input = new GetListPagedAndSortedDto
                {
                    MaxResultCount = 2,
                    SkipCount = 0,
                    Sorting = "CategoryName",
                    Filter = null
                };

                // Act
                var result = await _categoryAppService.GetListAsync(input);

                // Assert
                result.ShouldNotBeNull();
                result.Items.Count.ShouldBeLessThanOrEqualTo(2); // Sayfalama sınırı 2
                result.TotalCount.ShouldBeGreaterThan(0); // En az bir kayıt olmalı
            }
        }

        [Fact]
        public async Task GetListAsync_ShouldFilterResults_ByCategoryName()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var input = new GetListPagedAndSortedDto
                {
                    MaxResultCount = 10,
                    SkipCount = 0,
                    Sorting = "CategoryName",
                    Filter = "Yazılım" // Filtre
                };

                // Act
                var result = await _categoryAppService.GetListAsync(input);

                // Assert
                result.ShouldNotBeNull();
                result.Items.ShouldNotBeEmpty();
                result.Items.All(c => c.CategoryName.Contains("Yazılım")).ShouldBeTrue();
            }
        }

        [Fact]
        public async Task GetListAsync_ShouldSortResults_ByCategoryName()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var input = new GetListPagedAndSortedDto
                {
                    MaxResultCount = 10,
                    SkipCount = 0,
                    Sorting = "CategoryName" // İsme göre sıralama
                };

                // Act
                var result = await _categoryAppService.GetListAsync(input);

                // Assert
                result.ShouldNotBeNull();
                result.Items.ShouldNotBeEmpty();

                // İsme göre sıralı olduğunu doğrula
                var sorted = result.Items.OrderBy(c => c.CategoryName).ToList();
                result.Items.ShouldBe(sorted);
            }
        }

    

        [Fact]
        public async Task GetListAsync_Performance_ShouldCompleteInReasonableTime()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var input = new GetListPagedAndSortedDto
                {
                    MaxResultCount = 50,
                    SkipCount = 0,
                    Sorting = "CategoryName",
                    Filter = null
                };

                // Act
                var startTime = DateTime.UtcNow;
                var result = await _categoryAppService.GetListAsync(input);
                var endTime = DateTime.UtcNow;

                // Assert
                result.ShouldNotBeNull();
                (endTime - startTime).TotalMilliseconds.ShouldBeLessThan(1000); // Maksimum 1 saniye
            }
        }

        [Fact]
        public async Task GetListAsync_ShouldHandleInvalidInput_Gracefully()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var input = new GetListPagedAndSortedDto
                {
                    MaxResultCount = -1, // Geçersiz
                    SkipCount = -1, // Geçersiz
                    Sorting = null,
                    Filter = null
                };

                // Act & Assert
                await Assert.ThrowsAsync<AbpValidationException>(async () =>
                {
                    await _categoryAppService.GetListAsync(input);
                });
            }
        }

    }
}