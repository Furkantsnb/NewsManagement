using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace NewsManagement2.EntityDtos.CityDtos
{
    public class CityDto : FullAuditedEntityDto<int>
    {
        public string CityName { get; set; }
        public int CityCode { get; set; }
    }
   
}
