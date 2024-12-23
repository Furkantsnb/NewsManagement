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
                var createTagDto = new CreateTagDto { TagName = "Doğa" };

                // Act
                var result = await _tagAppService.CreateAsync(createTagDto);

                // Assert
                result.ShouldNotBeNull();
                result.TagName.ShouldBe("Doğa");
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

        // Performans Testi: Etiket oluşturma süresi
        [Fact]
        public async Task CreateAsync_Performance_ShouldCompleteInReasonableTime()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                var createTagDto = new CreateTagDto { TagName = "Hız Testi" };

                // Act
                var startTime = DateTime.UtcNow;
                await _tagAppService.CreateAsync(createTagDto);
                var endTime = DateTime.UtcNow;

                // Assert
                (endTime - startTime).TotalMilliseconds.ShouldBeLessThan(1000); // Maksimum 1 saniye
            }
        }

        //[Fact]
        //public async Task UpdateAsync_TagNameAlreadyExists_ShouldThrowAlreadyExistException()
        //{
        //    var updateTagDto = new UpdateTagDto { TagName = "Eğitim" };

        //    await Assert.ThrowsAsync<AlreadyExistException>(async () =>
        //    {
        //        await _tagAppService.UpdateAsync(1, updateTagDto);
        //    });
        //}

        //[Fact]
        //public async Task UpdateAsync_ValidTagName_ShouldUpdateSuccessfully()
        //{
        //    var updateTagDto = new UpdateTagDto { TagName = "GüncellenmişEtiket" };

        //    var result = await _tagAppService.UpdateAsync(1, updateTagDto);

        //    result.ShouldNotBeNull();
        //    result.TagName.ShouldBe("GüncellenmişEtiket");
        //}

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
