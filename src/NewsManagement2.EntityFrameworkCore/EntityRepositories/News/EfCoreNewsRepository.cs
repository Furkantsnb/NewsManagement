using NewsManagement2.Entities.ListableContents;
using NewsManagement2.Entities.Newses;
using NewsManagement2.EntityFrameworkCore;
using NewsManagement2.EntityRepositories.ListableContents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;

namespace NewsManagement2.EntityRepositories.News
{
    public class EfCoreNewsRepository: EfCoreListableContentGenericRepository<NewsManagement2.Entities.Newses.News>, INewsRepository, IListableContentGenericRepository<NewsManagement2.Entities.Newses.News>
    {
        public EfCoreNewsRepository(IDbContextProvider<NewsManagement2DbContext> dbContextProvider) : base(dbContextProvider)
        {
        }


    }
}
