using NewsManagement2.Entities.ListableContentRelations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace NewsManagement2.Entities.Tags
{
    public class Tag : FullAuditedAggregateRoot<int>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public string TagName { get; set; }

        public ICollection<ListableContentTag> ListableContentTags { get; set; }


        public Tag()
        {
        }


    }
}
