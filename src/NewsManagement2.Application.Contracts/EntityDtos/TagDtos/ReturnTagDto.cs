using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace NewsManagement2.EntityDtos.TagDtos
{
    public class ReturnTagDto : IEntityDto<int>
    {
        public int Id { get; set; }

        public string TagName { get; set; }
    }
}
