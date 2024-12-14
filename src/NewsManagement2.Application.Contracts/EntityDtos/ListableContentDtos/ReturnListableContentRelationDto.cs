using NewsManagement2.EntityConsts.ListableContentConsts;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace NewsManagement2.EntityDtos.ListableContentDtos
{
    public class ReturnListableContentRelationDto : IEntityDto<int>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ViewsCount { get; set; }
        public StatusType Status { get; set; }

    }
}
