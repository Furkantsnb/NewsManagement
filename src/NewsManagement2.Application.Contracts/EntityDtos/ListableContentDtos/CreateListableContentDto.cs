﻿using NewsManagement2.EntityConsts.ListableContentConsts;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace NewsManagement2.EntityDtos.ListableContentDtos
{
    public class CreateListableContentDto : EntityDto
    {
        public string Title { get; set; }
        public string Spot { get; set; }
        public Guid? ImageId { get; set; }
        public List<int> TagIds { get; set; }
        public List<int> CityIds { get; set; }
        public List<int>? RelatedListableContentIds { get; set; }
        public List<ListableContentCategoryDto> ListableContentCategoryDtos { get; set; }
        public DateTime? PublishTime { get; set; }
        public StatusType Status { get; set; }
    }
}