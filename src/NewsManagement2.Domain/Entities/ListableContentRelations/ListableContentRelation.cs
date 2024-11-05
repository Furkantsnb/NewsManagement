using NewsManagement2.Entities.ListableContents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace NewsManagement2.Entities.ListableContentRelations
{
    public class ListableContentRelation : Entity, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public int ListableContentId { get; set; }
        public ListableContent ListableContent { get; set; }

        public int RelatedListableContentId { get; set; }
        public ListableContent RelatedListableContent { get; set; }

        internal ListableContentRelation() { }


        public override object[] GetKeys()
        {
            return new object[] { ListableContentId, RelatedListableContentId };
        }
    }
}
