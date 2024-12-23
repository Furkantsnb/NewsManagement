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
using Volo.Abp.Domain.Entities;
using Xunit;

namespace NewsManagement2.Tag
{
    public class TagAppService_Test : NewsManagement2ApplicationTestBase
    {
        private readonly TagAppService _tagAppService;

        public TagAppService_Test()
        {
            _tagAppService = GetRequiredService<TagAppService>();
        }

        [Fact]
        public async Task CreateAsync_TagNameAlreadyExists_ShouldThrowAlreadyExistException()
        {
            var createTagDto = new CreateTagDto { TagName = "Tatil" };

            await Assert.ThrowsAsync<AlreadyExistException>(async () =>
            {
                await _tagAppService.CreateAsync(createTagDto);
            });
        }

        [Fact]
        public async Task CreateAsync_ValidTagName_ShouldCreateSuccessfully()
        {
            var createTagDto = new CreateTagDto { TagName = "YeniEtiket" };

            var result = await _tagAppService.CreateAsync(createTagDto);

            result.ShouldNotBeNull();
            result.TagName.ShouldBe("YeniEtiket");
        }

        [Fact]
        public async Task UpdateAsync_TagNameAlreadyExists_ShouldThrowAlreadyExistException()
        {
            var updateTagDto = new UpdateTagDto { TagName = "Eğitim" };

            await Assert.ThrowsAsync<AlreadyExistException>(async () =>
            {
                await _tagAppService.UpdateAsync(1, updateTagDto);
            });
        }

        [Fact]
        public async Task UpdateAsync_ValidTagName_ShouldUpdateSuccessfully()
        {
            var updateTagDto = new UpdateTagDto { TagName = "GüncellenmişEtiket" };

            var result = await _tagAppService.UpdateAsync(1, updateTagDto);

            result.ShouldNotBeNull();
            result.TagName.ShouldBe("GüncellenmişEtiket");
        }

        [Fact]
        public async Task GetListAsync_FilteredData_ShouldReturnMatchingTags()
        {
            var input = new GetListPagedAndSortedDto { Filter = "Tek" };

            var result = await _tagAppService.GetListAsync(input);

            result.Items.ShouldContain(tag => tag.TagName == "Teknoloji");
        }

        [Fact]
        public async Task GetListAsync_InvalidFilter_ShouldThrowNotFoundException()
        {
            var input = new GetListPagedAndSortedDto { Filter = "Geçersiz" };

            await Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await _tagAppService.GetListAsync(input);
            });
        }

        [Fact]
        public async Task DeleteAsync_InvalidId_ShouldThrowEntityNotFoundException()
        {
            await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            {
                await _tagAppService.DeleteAsync(9999);
            });
        }

        [Fact]
        public async Task DeleteAsync_ValidId_ShouldDeleteSuccessfully()
        {
            await _tagAppService.DeleteAsync(1);

            var tag = await _tagAppService.GetListAsync(new GetListPagedAndSortedDto { Filter = "Tatil" });
            tag.Items.ShouldNotContain(t => t.TagName == "Tatil");
        }

        [Fact]
        public async Task DeleteHardAsync_InvalidId_ShouldThrowEntityNotFoundException()
        {
            await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            {
                await _tagAppService.DeleteHardAsync(9999);
            });
        }

        [Fact]
        public async Task DeleteHardAsync_ValidId_ShouldDeletePermanently()
        {
            await _tagAppService.DeleteHardAsync(2);

            await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            {
                await _tagAppService.GetListAsync(new GetListPagedAndSortedDto { Filter = "Eğitim" });
            });
        }
    }
}
