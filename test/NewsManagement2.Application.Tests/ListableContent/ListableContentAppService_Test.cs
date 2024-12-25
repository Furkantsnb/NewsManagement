using NewsManagement2.AppService.ListableContents;
using NewsManagement2.EntityDtos.Newses;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace NewsManagement2.ListableContent
{
    public class ListableContentAppService_Test : NewsManagement2ApplicationTestBase
    {
        private readonly ListableContentAppService _listableContentAppService;
        private readonly IDataFilter<IMultiTenant> _dataFilter;

        public ListableContentAppService_Test()
        {
            _listableContentAppService = GetRequiredService<ListableContentAppService>();
            _dataFilter = GetRequiredService<IDataFilter<IMultiTenant>>();
        }

        [Fact]
        public async Task GetByIdAsync_ValidId_ShouldReturnListableContentDto()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                int validId = 1;  // SeedData'dan mevcut bir ID (varsa)

                // Act
                var result = await _listableContentAppService.GetByIdAsync(validId);

                // Assert
                result.ShouldNotBeNull();  // Sonuç null olmamalı
                result.ShouldBeOfType<NewsDto>();  // Dönen sonuç NewsDto türünde olmalı
            }
        }

        [Fact]
        public async Task GetByIdAsync_InvalidId_ShouldThrowEntityNotFoundException()
        {
            using (_dataFilter.Disable())
            {
                // Arrange
                int invalidId = 9999;  // SeedData'da mevcut olmayan bir ID

                // Act & Assert
                await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _listableContentAppService.GetByIdAsync(invalidId);
                });
            }
        }
    }
}
