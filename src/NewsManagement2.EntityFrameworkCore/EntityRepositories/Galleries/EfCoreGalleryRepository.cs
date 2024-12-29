using NewsManagement2.Entities.Galleries;
using NewsManagement2.Entities.ListableContents;
using NewsManagement2.EntityFrameworkCore;
using NewsManagement2.EntityRepositories.ListableContents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;

namespace NewsManagement2.EntityRepositories.Galleries
{
    public class EfCoreGalleryRepository : EfCoreListableContentGenericRepository<Gallery>, IGalleryRepository, IListableContentGenericRepository<Gallery>
    {
        public EfCoreGalleryRepository(IDbContextProvider<NewsManagement2DbContext> dbContextProvider) : base(dbContextProvider)
        {
        }


    }
}
