﻿using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace NewsManagement2.EntityDtos.TagDtos
{
    public class TagDto : FullAuditedEntityDto<int>
    {
        public string TagName { get; set; }
    }
}