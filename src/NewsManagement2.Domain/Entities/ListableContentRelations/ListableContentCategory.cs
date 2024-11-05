using NewsManagement2.Entities.Categories;
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
    public class ListableContentCategory : Entity, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public bool IsPrimary { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int ListableContentId { get; set; }
        public ListableContent ListableContent { get; set; }



        internal ListableContentCategory() { }

        public override object[] GetKeys()
        {
            return new object[] { CategoryId, ListableContentId };
        }

    }
}
