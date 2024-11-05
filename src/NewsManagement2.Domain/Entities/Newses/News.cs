using NewsManagement2.Entities.ListableContents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsManagement2.Entities.Newses
{
    public class News : ListableContent
    {
        public List<NewsDetailImage> DetailImageIds { get; set; }

        public News() { }
    }
}
