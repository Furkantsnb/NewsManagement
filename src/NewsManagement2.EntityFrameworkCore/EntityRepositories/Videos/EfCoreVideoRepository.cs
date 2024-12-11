using NewsManagement2.Entities.ListableContents;
using NewsManagement2.Entities.Videos;
using NewsManagement2.EntityFrameworkCore;
using NewsManagement2.EntityRepositories.ListableContents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;

namespace NewsManagement2.EntityRepositories.Videos
{
    public class EfCoreVideoRepository : EfCoreListableContentGenericRepository<Video>, IVideoRepository, IListableContentGenericRepository<Video>
    {
        public EfCoreVideoRepository(IDbContextProvider<NewsManagement2DbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

    }
}
