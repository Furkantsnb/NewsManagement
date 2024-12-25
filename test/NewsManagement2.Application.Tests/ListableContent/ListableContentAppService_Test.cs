using NewsManagement2.AppService.ListableContents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;

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
    }
}
