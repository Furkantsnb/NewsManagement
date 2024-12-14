using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace NewsManagement2.EntityDtos.CategoryDtos
{
    public class ReturnCategoryDto : IEntityDto<int>
    {
        public int Id { get; set; }
        public bool IsPrimary { get; set; }
        public string CategoryName { get; set; }
        public string ColorCode { get; set; }
        public int? ParentCategoryId { get; set; }

    }
}
