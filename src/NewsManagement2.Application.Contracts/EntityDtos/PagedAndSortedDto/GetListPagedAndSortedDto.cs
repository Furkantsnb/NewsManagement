using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace NewsManagement2.EntityDtos.PagedAndSortedDto
{
    public class GetListPagedAndSortedDto : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; }
    }
}
