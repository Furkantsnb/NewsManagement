using NewsManagement2.AppService.Newses;
using NewsManagement2.EntityConsts.ListableContentConsts;
using NewsManagement2.EntityDtos.ListableContentDtos;
using NewsManagement2.EntityDtos.Newses;
using NewsManagement2.EntityDtos.PagedAndSortedDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using Volo.Abp;
using Xunit;
using Volo.Abp.ObjectMapping;
using Shouldly;

namespace NewsManagement2.News
{
    public class NewsAppService_Test : NewsManagement2ApplicationTestBase
    {
        private readonly NewsAppService _newsAppService;
        private readonly IObjectMapper _objectMapper;
        private readonly Guid _filesImageId;
        private readonly Guid _uploadImageId;
        private readonly CreateNewsDto _createNewsDto;
        private readonly ICurrentTenant _currentTenant;
        private readonly ITenantRepository _tenantRepository;
        private readonly Guid? _tenantId;

        public NewsAppService_Test()
        {
            _newsAppService = GetRequiredService<NewsAppService>();
            _objectMapper = GetRequiredService<IObjectMapper>();
            _currentTenant = GetRequiredService<ICurrentTenant>();
            _tenantRepository = GetRequiredService<ITenantRepository>();

            _tenantId = _tenantRepository.FindByName(NewsManagement2Consts.ChildTenanName)?.Id;
            _filesImageId = NewsManagement2Consts.ChildTenanFilesImageId;
            _uploadImageId = NewsManagement2Consts.ChildTenanUploadImageId;

            _createNewsDto = new CreateNewsDto()
            {
                Title = "News Haber 1",
                Spot = "string",
                ImageId = _filesImageId,
                TagIds = new List<int>() { 1 },
                CityIds = new List<int>() { 1 },
                RelatedListableContentIds = new List<int>() { 1 },
                ListableContentCategoryDtos = new List<ListableContentCategoryDto>()
                {
                    new()
                    {
                        CategoryId = 3, IsPrimary = true
                    },
                    new()
                    {
                        CategoryId = 4, IsPrimary = false
                    }
                },
                PublishTime = null,
                Status = StatusType.Draft,
                DetailImageIds = new List<NewsDetailImageDto>()
                {
                    new()
                    {
                        DetailImageId = _uploadImageId
                    }
                }
            };
        }

        [Fact]
        public async Task CreateAsync_CheckTagDuplicateInput_BusinessException()
        {
            using (_currentTenant.Change(_tenantId))
            {
                _createNewsDto.Title = "News Haber 0";
                _createNewsDto.TagIds = new List<int>() { 1, 1 };

                var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
                {
                    await _newsAppService.CreateAsync(_createNewsDto);
                });

                exception.Code.ShouldBe(NewsManagement2DomainErrorCodes.RepeatedDataError);
                exception.Data["0"].ShouldBe("tagIds");
                exception.Data["1"].ShouldBe("1");
            }
        }

        [Fact]
        public async Task UpdateAsync_ReturnValue_NewsDto()
        {
            using (_currentTenant.Change(_tenantId))
            {
                var updateNews = _objectMapper.Map<CreateNewsDto, UpdateNewsDto>(_createNewsDto);
                updateNews.Title = "Updated News Title";
                var id = 1;

                var result = await _newsAppService.UpdateAsync(id, updateNews);

                result.ShouldNotBeNull();
                result.Title.ShouldBe("Updated News Title");
            }
        }

        [Fact]
        public async Task GetListAsync_FilterValid_NewsDto()
        {
            using (_currentTenant.Change(_tenantId))
            {
                var result = await _newsAppService.GetListAsync(new GetListPagedAndSortedDto() { Filter = "News Haber 1" });

                result.Items.ShouldNotBeNull();
                result.Items.ShouldContain(x => x.Title == "News Haber 1");
            }
        }

        [Fact]
        public async Task GetAsync_IdValid_ViewsCountIncrease()
        {
            using (_currentTenant.Change(_tenantId))
            {
                var id = 1;

                var viewsCount1 = (await _newsAppService.GetAsync(id)).ViewsCount;
                var viewsCount2 = (await _newsAppService.GetAsync(id)).ViewsCount;

                viewsCount2.ShouldBe(viewsCount1 + 1);
            }
        }

        [Fact]
        public async Task CreateAsync_CheckPrimaryCategory_BusinessException()
        {
            using (_currentTenant.Change(_tenantId))
            {
                _createNewsDto.ListableContentCategoryDtos = new List<ListableContentCategoryDto>()
                {
                    new()
                    {
                        CategoryId = 3, IsPrimary = true
                    },
                    new()
                    {
                        CategoryId = 4, IsPrimary = true
                    }
                };

                var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
                {
                    await _newsAppService.CreateAsync(_createNewsDto);
                });

                exception.Code.ShouldBe(NewsManagement2DomainErrorCodes.ActiveCategoryLimitExceeded);
            }
        }
    }
}
