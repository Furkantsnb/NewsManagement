using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace NewsManagement2.Entities.Cities
{
    public interface ICityRepository : IRepository<City, int>
    {
        Task<List<City>> GetListAsync(int skipCount, int maxResultCount, string sorting, string filter = null);
    }
}
