using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace NewsManagement2
{
    internal class NewsManagementDataSeederContributor : IDataSeedContributor, ITransientDependency
    {
        public Task SeedAsync(DataSeedContext context)
        {
            throw new NotImplementedException();
        }

        private async Task SeedTenantAsync()
        { 
        
        }

        }
}
