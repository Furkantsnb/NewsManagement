using NewsManagement2.EntityDtos.ListableContentDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsManagement2.EntityDtos.Newses
{
    public class UpdateNewsDto : UpdateListableContentDto
    {
        public List<NewsDetailImageDto> DetailImageIds { get; set; }
    }
}
