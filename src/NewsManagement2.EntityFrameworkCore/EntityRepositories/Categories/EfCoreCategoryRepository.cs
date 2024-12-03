using NewsManagement2.Entities.Categories;
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

namespace NewsManagement2.EntityRepositories.Categories
{
    public class EfCoreCategoryRepository : EfCoreRepository<NewsManagement2DbContext, Category, int>, ICategoryRepository
    {
        public EfCoreCategoryRepository(IDbContextProvider<NewsManagement2DbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<Category>> GetListAsync(int skipCount, int maxResultCount, string sorting, string filter = null)
        {
            var dbSet = await GetDbSetAsync();

            return await dbSet.WhereIf(!filter.IsNullOrWhiteSpace(), c => c.CategoryName.Contains(filter))
              .OrderBy(sorting).Skip(skipCount).Take(maxResultCount).ToListAsync();
        }
    }
}
