﻿using NewsManagement2.EntityDtos.ListableContentDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsManagement2.EntityDtos.GalleryDtos
{
    public class GalleryDto : ListableContentDto
    {
        public List<GalleryImageDto> GalleryImages { get; set; }
    }
}
