using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace NewsManagement2.Entities.Tags
{
    public interface ITagRepository : IRepository<Tag, int>
    {
        Task<List<Tag>> GetListAsync(int skipCount, int maxResultCount, string sorting, string filter = null);

    }
}
