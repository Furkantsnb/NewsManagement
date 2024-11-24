using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.ObjectMapping;

namespace NewsManagement2.Entities.Categories
{
    public class CategoryManager : DomainService
    {
        private readonly IObjectMapper _objectMapper;
        private readonly IDataFilter<ISoftDelete> _softDeleteFilter;

        public CategoryManager(IObjectMapper objectMapper, IDataFilter<ISoftDelete> softDeleteFilter)
        {
            _objectMapper = objectMapper;
            _softDeleteFilter = softDeleteFilter;
        }
    }
}
