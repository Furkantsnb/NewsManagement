using NewsManagement2.AppService.Videos;
using NewsManagement2.EntityConsts.ListableContentConsts;
using NewsManagement2.EntityConsts.VideoConsts;
using NewsManagement2.EntityDtos.ListableContentDtos;
using NewsManagement2.EntityDtos.PagedAndSortedDto;
using NewsManagement2.EntityDtos.VideoDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using Volo.Abp;
using Xunit;
using Volo.Abp.ObjectMapping;

namespace NewsManagement2.Video
{
    public class VideoAppService_Test : NewsManagement2ApplicationTestBase
    {

        private readonly VideoAppService _videoAppService;
        private readonly IObjectMapper _objectMapper;
        private readonly IDataFilter<IMultiTenant> _dataFilter;
        private readonly Guid _filesImageId;
        private readonly Guid _uploadImageId;
        private readonly CreateVideoDto _createVideoDto;

        public VideoAppService_Test()
        {
            _videoAppService = GetRequiredService<VideoAppService>();
            _objectMapper = GetRequiredService<IObjectMapper>();
            _dataFilter = GetRequiredService<IDataFilter<IMultiTenant>>();
            _filesImageId = NewsManagement2Consts.YoungTenanFilesImageId;
            _uploadImageId = NewsManagement2Consts.YoungTenanUploadImageId;

            _createVideoDto = new CreateVideoDto
            {
                Title = "Test Video",
                Spot = "Test Spot",
                ImageId = _filesImageId,
                TagIds = new List<int> { 5 },
                CityIds = new List<int> { 8 },
                RelatedListableContentIds = new List<int> { 12 },
                ListableContentCategoryDtos = new List<ListableContentCategoryDto>
                {
                    new() { CategoryId = 11, IsPrimary = true }
                },
                PublishTime = DateTime.Now,
                Status = StatusType.Draft,
                VideoType = VideoType.Video,
                VideoId = _uploadImageId,
            };
        }

        [Fact]
        public async Task CreateAsync_Should_Create_Video()
        {
            using (_dataFilter.Disable())
            {
                var createVideoDto = new CreateVideoDto
                {
                    Title = "Video Test 1",
                    Spot = "Video Spot 1",
                    ImageId = _filesImageId,
                    TagIds = new List<int> { 1 }, // Seed data'ya uygun bir Tag ID
                    CityIds = new List<int> { 2 }, // Seed data'ya uygun bir City ID
                    ListableContentCategoryDtos = new List<ListableContentCategoryDto>
                    {
                        new() { CategoryId = 1, IsPrimary = true } // Seed data'ya uygun bir Category ID
                    },
                    Status = StatusType.Draft,
                    VideoType = VideoType.Video,
                    VideoId = _uploadImageId
                };

                var result = await _videoAppService.CreateAsync(createVideoDto);

                Assert.NotNull(result);
                Assert.Equal(createVideoDto.Title, result.Title);
                Assert.Equal(createVideoDto.VideoType, result.VideoType);
            }
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Video()
        {
            using (_dataFilter.Disable())
            {
                var updateDto = new UpdateVideoDto
                {
                    Title = "Updated Video Title",
                    Spot = "Updated Video Spot",
                    ImageId = _filesImageId,
                    TagIds = new List<int> { 1 }, // Seed data'ya uygun bir Tag ID
                    CityIds = new List<int> { 2 }, // Seed data'ya uygun bir City ID
                    ListableContentCategoryDtos = new List<ListableContentCategoryDto>
                    {
                        new() { CategoryId = 1, IsPrimary = true } // Seed data'ya uygun bir Category ID
                    },
                    Status = StatusType.Published,
                    VideoType = VideoType.Link,
                    Url = "http://updated-link.com" // VideoType Link olduğunda URL gerekli
                };

                var id = 6; // Seed data'ya göre mevcut bir Video ID
                var result = await _videoAppService.UpdateAsync(id, updateDto);

                Assert.NotNull(result);
                Assert.Equal(updateDto.Title, result.Title);
                Assert.Equal(updateDto.Url, result.Url);
            }
        }

        [Fact]
        public async Task DeleteAsync_Should_Throw_EntityNotFoundException_When_IdInvalid()
        {
            using (_dataFilter.Disable())
            {
                int id = 99; // Geçersiz ID
                await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _videoAppService.DeleteAsync(id);
                });
            }
        }

        [Fact]
        public async Task DeleteHardAsync_Should_Delete_Video_Permanently()
        {
            using (_dataFilter.Disable())
            {
                var id = 16; // Mevcut bir video ID'si

                await _videoAppService.DeleteHardAsync(id);

                await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _videoAppService.GetAsync(id);
                });
            }
        }

        [Fact]
        public async Task GetListAsync_Should_Return_Filtered_Videos()
        {
            using (_dataFilter.Disable())
            {
                var input = new GetListPagedAndSortedDto
                {
                    SkipCount = 0,
                    MaxResultCount = 10,
                    Sorting = "Title",
                    Filter = "Video" // Seed data'ya uygun bir filtre
                };

                var result = await _videoAppService.GetListAsync(input);

                Assert.NotNull(result);
                Assert.True(result.Items.Count > 0);
                Assert.All(result.Items, video => Assert.Contains("Video", video.Title));
            }
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_BusinessException_When_Category_Invalid()
        {
            using (_dataFilter.Disable())
            {
                _createVideoDto.ListableContentCategoryDtos = new List<ListableContentCategoryDto>
                {
                    new() { CategoryId = 17, IsPrimary = true },
                    new() { CategoryId = 18, IsPrimary = false }
                };

                var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
                {
                    await _videoAppService.CreateAsync(_createVideoDto);
                });

                Assert.Equal(NewsManagement2DomainErrorCodes.ParentCategoryRequired, exception.Code);
                Assert.Equal("17, 18", exception.Data["categoryId"]);
            }
        }
    }
}
