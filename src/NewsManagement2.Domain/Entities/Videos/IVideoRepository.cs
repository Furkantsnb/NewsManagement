using NewsManagement2.Entities.ListableContents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsManagement2.Entities.Videos
{
    public interface IVideoRepository : IListableContentGenericRepository<Video>
    {
    }
}
