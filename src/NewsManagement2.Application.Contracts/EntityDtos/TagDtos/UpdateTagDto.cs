using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace NewsManagement2.EntityDtos.TagDtos
{
    public class UpdateTagDto : EntityDto
    {
        public string TagName { get; set; }
    }
}
