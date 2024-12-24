using NewsManagement2.AppService.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;

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
    }
}
