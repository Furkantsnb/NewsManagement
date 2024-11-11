using NewsManagement2.Entities.ListableContents;
using NewsManagement2.EntityConsts.VideoConsts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsManagement2.Entities.Videos
{
    public class Video : ListableContent
    {
        public VideoType VideoType { get; set; }
        public string? Url { get; set; }
        public Guid? VideoId { get; set; }


        public Video() { }

    }
}
