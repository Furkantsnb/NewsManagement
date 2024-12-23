﻿using NewsManagement2.EntityConsts.VideoConsts;
using NewsManagement2.EntityDtos.ListableContentDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsManagement2.EntityDtos.Videos
{
    public class VideoDto : ListableContentDto
    {
        public VideoType VideoType { get; set; }
        public string? Url { get; set; }
        public Guid? VideoId { get; set; }
    }
}
