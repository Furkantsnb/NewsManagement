using NewsManagement2.Entities.ListableContents;
using NewsManagement2.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NewsManagement2.EntityRepositories.ListableContents
{
    public class EfCoreListableContentGenericRepository<T> : EfCoreRepository<NewsManagement2DbContext, T, int>, IListableContentGenericRepository<T>
       where T : ListableContent, new()
    {

        public EfCoreListableContentGenericRepository(IDbContextProvider<NewsManagement2DbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<T>> GetListAsync(int skipCount, int maxResultCount, string sorting, string filter)
        {

            var dbSet = await GetDbSetAsync();

            return await dbSet.WhereIf(!filter.IsNullOrWhiteSpace(), c => c.Title.Contains(filter))
                .OrderBy(sorting).Skip(skipCount).Take(maxResultCount).ToListAsync();

        }
    }
    }
