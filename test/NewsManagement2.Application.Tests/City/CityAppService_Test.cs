using NewsManagement2.AppService.Cities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;

namespace NewsManagement2.City
{
    public class CityAppService_Test: NewsManagement2ApplicationTestBase
    {
        private readonly CityAppService _cityAppService;
        private readonly IDataFilter<IMultiTenant> _dataFilter;
        public CityAppService_Test()
        {
            _cityAppService = GetRequiredService<CityAppService>();
            _dataFilter = GetRequiredService<IDataFilter<IMultiTenant>>();
        }
    }
}
