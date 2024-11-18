using NewsManagement2.EntityConsts.ListableContentConsts;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace NewsManagement2.EntityDtos.ListableContentDtos
{
    public class ListableContentDto : FullAuditedEntityDto<int>
    {
        public string Title { get; set; }
        public string Spot { get; set; }
        public Guid? ImageId { get; set; }
        public int ViewsCount { get; set; }
        public StatusType Status { get; set; }
        public DateTime? PublishTime { get; set; }
        public ListableContentType ListableContentType { get; set; }
       
    }
}
