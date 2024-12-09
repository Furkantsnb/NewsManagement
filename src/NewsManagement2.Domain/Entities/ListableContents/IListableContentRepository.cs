using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace NewsManagement2.Entities.ListableContents
{
    public interface IListableContentRepository : IRepository<ListableContent, int>
    {
        Task<List<ListableContent>> GetListFilterAsync(int skipCount, int maxResultCount, string sorting, string filter = null);
    }
}
