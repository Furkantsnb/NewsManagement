using NewsManagement2.Entities.ListableContents;
using NewsManagement2.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace NewsManagement2.EntityRepositories.ListableContents
{
    public class EfCoreListableContentRepository : EfCoreRepository<NewsManagement2DbContext, ListableContent, int>, IListableContentRepository
    {
        public EfCoreListableContentRepository(IDbContextProvider<NewsManagement2DbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
        public async Task<List<ListableContent>> GetListFilterAsync(int skipCount, int maxResultCount, string sorting, string filter = null)
        {
            var dbSet = await GetDbSetAsync();

            return await dbSet.WhereIf(!filter.IsNullOrWhiteSpace(), c => c.Title.Contains(filter))
              .OrderBy(sorting).Skip(skipCount).Take(maxResultCount).ToListAsync();
        }


    }
}
