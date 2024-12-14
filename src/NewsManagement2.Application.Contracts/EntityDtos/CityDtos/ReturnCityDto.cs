using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace NewsManagement2.EntityDtos.CityDtos
{
    public class ReturnCityDto : IEntityDto<int>
    {
        public int Id { get; set; }
        public string CityName { get; set; }
        public int CityCode { get; set; }
    }
}
