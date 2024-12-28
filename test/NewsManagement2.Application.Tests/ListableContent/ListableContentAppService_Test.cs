using NewsManagement2.AppService.ListableContents;
using NewsManagement2.Entities.Newses;
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
        public async Task GetByIdAsync_ReturnValue_NewsDto()
        {
            using (_dataFilter.Disable())
            {
                var id = 1;

                var listableContent = await _listableContentAppService.GetByIdAsync(id);

                Assert.NotNull(listableContent);
                Assert.IsType<NewsDto>(listableContent);
            }
        }
    }
}
