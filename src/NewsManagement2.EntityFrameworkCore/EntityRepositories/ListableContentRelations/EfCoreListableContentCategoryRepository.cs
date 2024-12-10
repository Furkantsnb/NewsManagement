using NewsManagement2.Entities.ListableContentRelations;
using NewsManagement2.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NewsManagement2.EntityRepositories.ListableContentRelations
{
    public class EfCoreListableContentCategoryRepository : EfCoreRepository<NewsManagement2DbContext, ListableContentCategory>, IListableContentCategoryRepository
    {
        public EfCoreListableContentCategoryRepository(IDbContextProvider<NewsManagement2DbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<ListableContentCategory>> GetCrossListAsync(int id)
        {
            var dbSet = await GetDbSetAsync();

            return await dbSet.Where(x => x.ListableContentId == id)
              .Include(x => x.Category).ToListAsync();
        }
    }
}
