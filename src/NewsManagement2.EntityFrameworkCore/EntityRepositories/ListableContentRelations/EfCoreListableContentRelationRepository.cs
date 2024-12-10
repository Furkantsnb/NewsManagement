using Microsoft.EntityFrameworkCore;
using NewsManagement2.Entities.ListableContentRelations;
using NewsManagement2.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace NewsManagement2.EntityRepositories.ListableContentRelations
{
    public class EfCoreListableContentRelationRepository : EfCoreRepository<NewsManagement2DbContext, ListableContentRelation>, IListableContentRelationRepository
    {
        public EfCoreListableContentRelationRepository(IDbContextProvider<NewsManagement2DbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<ListableContentRelation>> GetCrossListAsync(int id)
        {
            var dbSet = await GetDbSetAsync();

            return await dbSet.Where(x => x.ListableContentId == id)
              .Include(x => x.RelatedListableContent).ToListAsync();
        }
    }
}
