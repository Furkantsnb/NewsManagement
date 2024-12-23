using NewsManagement2.AppService.Tags;
using NewsManagement2.Entities.Exceptions;
using NewsManagement2.EntityDtos.PagedAndSortedDto;
using NewsManagement2.EntityDtos.TagDtos;
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

namespace NewsManagement2.Tag
{
    public class TagAppService_Test : NewsManagement2ApplicationTestBase
    {
        private readonly TagAppService _tagAppService;
        private readonly IDataFilter<IMultiTenant> _dataFilter;

        public TagAppService_Test()
        {
            _tagAppService = GetRequiredService<TagAppService>();
            _dataFilter = GetRequiredService<IDataFilter<IMultiTenant>>();

        }

        // Pozitif Test: Geçerli bir etiket oluşturma
        [Fact]
        public async Task CreateAsync_ValidTagName_ShouldCreateSuccessfully()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var createTagDto = new CreateTagDto { TagName = "news" };

                // Act
                var result = await _tagAppService.CreateAsync(createTagDto);

                // Assert
                result.ShouldNotBeNull();
                result.TagName.ShouldBe("news");
            }
        }

        // Negatif Test: Aynı isimli etiketin zaten var olması
        [Fact]
        public async Task CreateAsync_TagNameAlreadyExists_ShouldThrowAlreadyExistException()
        {
            using (_dataFilter.Disable())
            {
                // Arrange (SeedData'dan gelen "Tatil" etiketi mevcut)
                var createTagDto = new CreateTagDto { TagName = "Tatil" };

                // Act & Assert
                await Assert.ThrowsAsync<AlreadyExistException>(async () =>
                {
                    await _tagAppService.CreateAsync(createTagDto);
                });
            }
        }

        // Negatif Test: Boş bir TagName ile oluşturma
        [Fact]
        public async Task CreateAsync_EmptyTagName_ShouldThrowBusinessException()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var createTagDto = new CreateTagDto { TagName = string.Empty };

                // Act & Assert
                await Assert.ThrowsAsync<AbpValidationException>(async () =>
                {
                    await _tagAppService.CreateAsync(createTagDto);
                });
            }
        }

        // Negatif Test: Maksimum uzunluğu aşan TagName
        [Fact]
        public async Task CreateAsync_TagNameExceedsMaxLength_ShouldThrowBusinessException()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var createTagDto = new CreateTagDto { TagName = new string('A', 300) };

                // Act & Assert
                await Assert.ThrowsAsync<AbpValidationException>(async () =>
                {
                    await _tagAppService.CreateAsync(createTagDto);
                });
            }
        }

        // Performans Testi: Etiket oluşturma süresi
        [Fact]
        public async Task CreateAsync_Performance_ShouldCompleteInReasonableTime()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var createTagDto = new CreateTagDto { TagName = "UniqueTestTag" }; // Benzersiz bir ad kullanın

                // Act
                var startTime = DateTime.UtcNow;
                var result = await _tagAppService.CreateAsync(createTagDto);
                var endTime = DateTime.UtcNow;

                // Assert
                result.ShouldNotBeNull(); // Sonuç doğrulama
                result.TagName.ShouldBe("UniqueTestTag");
                (endTime - startTime).TotalMilliseconds.ShouldBeLessThan(2000); // Maksimum 1 saniye
            }
        }

        /// <summary>
        /// Geçerli bir Tag ID'si ve güncelleme verisi ile başarıyla güncellenmesini test eder.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_ValidInput_ShouldUpdateSuccessfully()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var validTagId = 2; // SeedData'dan var olan bir ID
                var updateTagDto = new UpdateTagDto { TagName = "Updated Tag" };

                // Act
                var result = await _tagAppService.UpdateAsync(validTagId, updateTagDto);

                // Assert
                result.ShouldNotBeNull();
                result.TagName.ShouldBe("Updated Tag");
            }
        }

        /// <summary>
        /// Güncellenecek Tag mevcut değilse, EntityNotFoundException fırlatıldığını test eder.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_NonExistentId_ShouldThrowEntityNotFoundException()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var invalidTagId = 999; // SeedData'da bulunmayan bir ID
                var updateTagDto = new UpdateTagDto { TagName = "Updated Tag" };

                // Act & Assert
                await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _tagAppService.UpdateAsync(invalidTagId, updateTagDto);
                });
            }
        }

        /// <summary>
        /// Yeni TagName başka bir Tag tarafından kullanılıyorsa AlreadyExistException fırlatıldığını test eder.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_TagNameAlreadyExists_ShouldThrowAlreadyExistException()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var validTagId = 2; // SeedData'dan var olan bir ID
                var updateTagDto = new UpdateTagDto { TagName = "Tatil" }; // SeedData'da mevcut olan bir TagName

                // Act & Assert
                await Assert.ThrowsAsync<AlreadyExistException>(async () =>
                {
                    await _tagAppService.UpdateAsync(validTagId, updateTagDto);
                });
            }
        }

        /// <summary>
        /// Boş bir TagName ile güncelleme yapıldığında validasyon hatası fırlatıldığını test eder.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_EmptyTagName_ShouldThrowValidationException()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var validTagId = 2; // SeedData'dan var olan bir ID
                var updateTagDto = new UpdateTagDto { TagName = string.Empty };

                // Act & Assert
                await Assert.ThrowsAsync<AbpValidationException>(async () =>
                {
                    await _tagAppService.UpdateAsync(validTagId, updateTagDto);
                });
            }
        }

        /// <summary>
        /// Çok kısa bir TagName ile güncelleme yapıldığında validasyon hatası fırlatıldığını test eder.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_TagNameTooShort_ShouldThrowValidationException()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var validTagId = 2; // SeedData'dan var olan bir ID
                var updateTagDto = new UpdateTagDto { TagName = "A" }; // Geçersiz kısa bir isim

                // Act & Assert
                await Assert.ThrowsAsync<AbpValidationException>(async () =>
                {
                    await _tagAppService.UpdateAsync(validTagId, updateTagDto);
                });
            }
        }

        /// <summary>
        /// Çok uzun bir TagName ile güncelleme yapıldığında validasyon hatası fırlatıldığını test eder.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_TagNameTooLong_ShouldThrowValidationException()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var validTagId = 2; // SeedData'dan var olan bir ID
                var updateTagDto = new UpdateTagDto
                {
                    TagName = new string('A', 51) // Geçersiz uzun bir isim
                };

                // Act & Assert
                await Assert.ThrowsAsync<AbpValidationException>(async () =>
                {
                    await _tagAppService.UpdateAsync(validTagId, updateTagDto);
                });
            }
        }


        //[Fact]
        //public async Task GetListAsync_FilteredData_ShouldReturnMatchingTags()
        //{
        //    var input = new GetListPagedAndSortedDto { Filter = "Tek" };

        //    var result = await _tagAppService.GetListAsync(input);

        //    result.Items.ShouldContain(tag => tag.TagName == "Teknoloji");
        //}

        //[Fact]
        //public async Task GetListAsync_InvalidFilter_ShouldThrowNotFoundException()
        //{
        //    var input = new GetListPagedAndSortedDto { Filter = "Geçersiz" };

        //    await Assert.ThrowsAsync<NotFoundException>(async () =>
        //    {
        //        await _tagAppService.GetListAsync(input);
        //    });
        //}

        //[Fact]
        //public async Task DeleteAsync_InvalidId_ShouldThrowEntityNotFoundException()
        //{
        //    await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
        //    {
        //        await _tagAppService.DeleteAsync(9999);
        //    });
        //}

        //[Fact]
        //public async Task DeleteAsync_ValidId_ShouldDeleteSuccessfully()
        //{
        //    await _tagAppService.DeleteAsync(1);

        //    var tag = await _tagAppService.GetListAsync(new GetListPagedAndSortedDto { Filter = "Tatil" });
        //    tag.Items.ShouldNotContain(t => t.TagName == "Tatil");
        //}

        //[Fact]
        //public async Task DeleteHardAsync_InvalidId_ShouldThrowEntityNotFoundException()
        //{
        //    await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
        //    {
        //        await _tagAppService.DeleteHardAsync(9999);
        //    });
        //}

        //[Fact]
        //public async Task DeleteHardAsync_ValidId_ShouldDeletePermanently()
        //{
        //    await _tagAppService.DeleteHardAsync(2);

        //    await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
        //    {
        //        await _tagAppService.GetListAsync(new GetListPagedAndSortedDto { Filter = "Eğitim" });
        //    });
        //}
    }
}
