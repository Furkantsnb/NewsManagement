using NewsManagement2.AppService.Galleries;
using NewsManagement2.Entities.Exceptions;
using NewsManagement2.EntityConsts.ListableContentConsts;
using NewsManagement2.EntityDtos.GalleryDtos;
using NewsManagement2.EntityDtos.ListableContentDtos;
using NewsManagement2.EntityDtos.PagedAndSortedDto;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Xunit;

namespace NewsManagement2.Gallery
{
    public class GalleryAppService_Test : NewsManagement2ApplicationTestBase
    {

        private readonly GalleryAppService _galleryAppService;
        private readonly IObjectMapper _objectMapper;
        private readonly Guid _filesImageId;
        private readonly Guid _uploadImageId;
        private readonly CreateGalleryDto _createGalleryDto;
        private readonly ICurrentTenant _currentTenant;
        private readonly Guid? _tenantId;

        public GalleryAppService_Test()
        {
            _galleryAppService = GetRequiredService<GalleryAppService>();
            _objectMapper = GetRequiredService<IObjectMapper>();
            _currentTenant = GetRequiredService<ICurrentTenant>();

            // Initialize constants from Seed Data
            _filesImageId = NewsManagement2Consts.ChildTenanFilesImageId;
            _uploadImageId = NewsManagement2Consts.ChildTenanUploadImageId;
            _tenantId = _currentTenant.Id;

            _createGalleryDto = new CreateGalleryDto
            {
                Title = "Gallery Test",
                Spot = "Test Spot",
                ImageId = _filesImageId,
                TagIds = new List<int> { 1 },
                CityIds = new List<int> { 1 },
                RelatedListableContentIds = new List<int> { 1 },
                ListableContentCategoryDtos = new List<ListableContentCategoryDto>
                {
                    new ListableContentCategoryDto { CategoryId = 1, IsPrimary = true }
                },
                PublishTime = DateTime.UtcNow,
                Status = StatusType.Draft,
                GalleryImages = new List<GalleryImageDto>
                {
                    new GalleryImageDto { ImageId = _filesImageId, NewsContent = "Image 1", Order = 1 },
                    new GalleryImageDto { ImageId = _uploadImageId, NewsContent = "Image 2", Order = 2 }
                }
            };
        }

        [Fact]
        public async Task CreateAsync_ValidGallery_ShouldCreateSuccessfully()
        {
            using (_currentTenant.Change(_tenantId))
            {
                // Act
                var result = await _galleryAppService.CreateAsync(_createGalleryDto);

                // Assert
                result.ShouldNotBeNull();
                result.Title.ShouldBe(_createGalleryDto.Title);
                result.Status.ShouldBe(StatusType.Draft);
            }
        }

        [Fact]
        public async Task CreateAsync_DuplicateTitle_ShouldThrowAlreadyExistException()
        {
            using (_currentTenant.Change(_tenantId))
            {
                // Act & Assert
                await Assert.ThrowsAsync<AlreadyExistException>(async () =>
                {
                    await _galleryAppService.CreateAsync(_createGalleryDto);
                });
            }
        }

        [Fact]
        public async Task GetAsync_ValidId_ShouldReturnGallery()
        {
            using (_currentTenant.Change(_tenantId))
            {
                // Arrange
                var galleryId = (await _galleryAppService.GetListAsync(new GetListPagedAndSortedDto { Filter = "Gallery Haber 1" })).Items.First().Id;

                // Act
                var result = await _galleryAppService.GetAsync(galleryId);

                // Assert
                result.ShouldNotBeNull();
                result.Title.ShouldBe("Gallery Haber 1");
            }
        }

        [Fact]
        public async Task UpdateAsync_ValidUpdate_ShouldUpdateSuccessfully()
        {
            using (_currentTenant.Change(_tenantId))
            {
                // Arrange
                var galleryId = (await _galleryAppService.GetListAsync(new GetListPagedAndSortedDto { Filter = "Gallery Haber 1" })).Items.First().Id;
                var updateGalleryDto = _objectMapper.Map<CreateGalleryDto, UpdateGalleryDto>(_createGalleryDto);
                updateGalleryDto.Title = "Updated Title";

                // Act
                var result = await _galleryAppService.UpdateAsync(galleryId, updateGalleryDto);

                // Assert
                result.ShouldNotBeNull();
                result.Title.ShouldBe("Updated Title");
            }
        }

        [Fact]
        public async Task DeleteHardAsync_ValidId_ShouldDeleteSuccessfully()
        {
            using (_currentTenant.Change(_tenantId))
            {
                // Arrange
                var galleryId = (await _galleryAppService.GetListAsync(new GetListPagedAndSortedDto { Filter = "Gallery Haber 3" })).Items.First().Id;

                // Act
                await _galleryAppService.DeleteHardAsync(galleryId);

                // Assert
                await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _galleryAppService.GetAsync(galleryId);
                });
            }
        }

        [Fact]
        public async Task GetListAsync_ShouldReturnPaginatedResults()
        {
            using (_currentTenant.Change(_tenantId))
            {
                // Act
                var result = await _galleryAppService.GetListAsync(new GetListPagedAndSortedDto { SkipCount = 0, MaxResultCount = 10 });

                // Assert
                result.ShouldNotBeNull();
                result.Items.Count.ShouldBeGreaterThan(0);
                result.TotalCount.ShouldBeGreaterThan(0);
            }
        }
    }
}
