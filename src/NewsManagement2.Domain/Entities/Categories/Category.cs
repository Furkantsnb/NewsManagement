using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace NewsManagement2.Entities.Categories
{
    public class Category : FullAuditedAggregateRoot<int>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
        public string ColorCode { get; set; }
        public int? ParentCategoryId { get; set; }

        public Category()
        {
        }
    }
}
