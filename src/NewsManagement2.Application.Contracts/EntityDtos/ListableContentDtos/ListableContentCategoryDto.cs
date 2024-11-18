using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace NewsManagement2.EntityDtos.ListableContentDtos
{
    public class ListableContentCategoryDto : IEntityDto
    {
        public int CategoryId { get; set; }
        public bool IsPrimary { get; set; }
    }
}
