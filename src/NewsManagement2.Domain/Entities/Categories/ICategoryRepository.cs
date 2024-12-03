using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace NewsManagement2.Entities.Categories
{
    public interface ICategoryRepository : IRepository<Category, int>
    {
        Task<List<Category>> GetListAsync(int skipCount, int maxResultCount, string sorting, string filter = null);
    }
}
